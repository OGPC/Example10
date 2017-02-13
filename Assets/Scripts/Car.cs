using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Car : MonoBehaviour {

	public float gravity = 20f;

	public float positionSaveDelay = 5f;
	float positionSaveTimer;
	Vector3[] savedPosition = {Vector3.zero, Vector3.zero};
	
	public float throttlePos = 0f;
	public float brakePos = 0f;
	public bool handbrakePos = false;
	public float steerTarget = 0f;
	private float steerPos = 0f;

	public bool allWheelDrive = true;

	public AnimationCurve downForce;
	public float maxDownForce = 50f;
	public float maxDownForceSpeed = 100f;
	public AnimationCurve driveTorque;
	public float maxTorque = 10000f;
	public float maxSpeed = 50f;
	public WheelCollider[] steeringWheels;
	public WheelCollider[] staticWheels;
	public float brakeTorque = 5000f;
	public float handbrakeTorque = 10000f;
	public float maxSteerSpeed = 5f;
	public float maxSteerAngle = 30;
	public GameObject[] lights;
	bool lightsOn = false;

	[HideInInspector] public Rigidbody RB;
	[HideInInspector] public float speed;

	void Start () {
		positionSaveTimer = 0f;
		savedPosition[0] = transform.position;
		savedPosition[1] = transform.position;

		// This is safe without checking for success because of line 3.
		RB = GetComponent<Rigidbody>();

		// Sets variables to good values.  This part is mostly optional.
		steerTarget = Mathf.Clamp(steerTarget, -1f, 1f);
		throttlePos = Mathf.Clamp(throttlePos, -1f, 1f);
		brakePos = Mathf.Clamp01(brakePos);
		steerPos = steerTarget;

		//Make sure the wheels aren't screwey.  This part is also optional.
		foreach (WheelCollider wheel in steeringWheels) {
			wheel.motorTorque = 0f;
			wheel.brakeTorque = 0f;
			wheel.ConfigureVehicleSubsteps(5f, 12, 15);
		}
		foreach (WheelCollider wheel in staticWheels) {
			wheel.motorTorque = 0f;
			wheel.brakeTorque = 0f;
			wheel.ConfigureVehicleSubsteps(5f, 12, 15);
		}
	}

	void Update () {
		
		if (Vector3.Dot(UnityEngine.RenderSettings.sun.transform.forward, transform.position.normalized) > 0f) {
			lightsOn = true;
		} else {
			lightsOn = false;
		}

		foreach (GameObject o in lights) {
			o.GetComponent<Light>().intensity = Mathf.Lerp(o.GetComponent<Light>().intensity, lightsOn?3f:0f, 0.1f);
		}
	
	}

	void FixedUpdate () {

		// Makes sure input values are within acceptable ranges.
		steerTarget = Mathf.Clamp(steerTarget, -1f, 1f);
		throttlePos = Mathf.Clamp(throttlePos, -1f, 1f);
		brakePos = Mathf.Clamp01(brakePos);

		// Calculates vehicle speed.  This is useful for a spedometer or to tell Driver.cs how to behave.
		speed = Vector3.Dot(RB.velocity, transform.forward);

		// Change gravity to point towards the origin
		Physics.gravity = transform.position.normalized * -gravity;

		// Push the car down more the faster it goes. This mimics aerodynamic properties of real cars.
		RB.AddRelativeForce(Vector3.down * downForce.Evaluate(speed / maxDownForceSpeed) * maxDownForce, ForceMode.Acceleration);

		// Puts a cap on how fast you can swerve.  You can't turn a real steering wheel to an arbitrary position in 1/60th of a second.
		steerPos = Mathf.MoveTowards(steerPos, steerTarget, maxSteerSpeed * Time.fixedDeltaTime);

		foreach (WheelCollider wheel in steeringWheels) {
			// Changes steering range from [0, 1] to [-1, 1] and finds the desired angle.
			float steerAngle = Mathf.Lerp(-maxSteerAngle, maxSteerAngle, 0.5f * (steerPos + 1f));
			wheel.steerAngle = steerAngle; // Turns wheel
			wheel.transform.localEulerAngles = Vector3.up * steerAngle; // Turns children (aka wheel mesh)

			// Moves wheel mesh to the ground
			PlaceWheelChildren(wheel, wheel.transform.GetChild(0));

			// Applies normal analog brakes if appropriate.  This is like the brake pedal on a normal car.
			wheel.brakeTorque = Mathf.Lerp(0f, brakeTorque, brakePos);

			// Applies the handbrake if appropriate
			if (handbrakePos)
				wheel.brakeTorque = handbrakeTorque;

			// Applies engine torque depending on throttle
			float torque = Mathf.Sign(throttlePos);
			torque *= maxTorque * Mathf.Lerp(
				0f,
				driveTorque.Evaluate((wheel.rpm * 2f * Mathf.PI * wheel.radius / 60f)/maxSpeed),
				Mathf.Abs(throttlePos)
			);
			wheel.motorTorque = torque;

			// Control particles
			ControlParticles(wheel, wheel.transform.GetChild(1).GetComponent<ParticleSystem>());
		}

		foreach (WheelCollider wheel in staticWheels) {
			//Make wheels touch the ground
			PlaceWheelChildren(wheel, wheel.transform.GetChild(0));

			// Applies normal analog brakes if appropriate.  This is like the brake pedal on a normal car.
			//wheel.brakeTorque = Mathf.Lerp(0f, brakeTorque, brakePos);

			// Applies the handbrake if appropriate
			if (handbrakePos)
				wheel.brakeTorque = handbrakeTorque;
			else
				wheel.brakeTorque = 0f;

			// If using 4-wheel drive, the back wheels need torque as well
			if (allWheelDrive) {
				float torque = Mathf.Sign(throttlePos);
				torque *= maxTorque * Mathf.Lerp(
					0f,
					driveTorque.Evaluate((wheel.rpm * 2f * Mathf.PI * wheel.radius / 60f)/maxSpeed),
					Mathf.Abs(throttlePos)
				);
				wheel.motorTorque = torque;
			} else
				wheel.motorTorque = 0f;


			ControlParticles(wheel, wheel.transform.GetChild(1).GetComponent<ParticleSystem>());
		}

		// Check for a flipped car and unflip it
		if (Vector3.Dot(transform.up, transform.position.normalized) < 0f
		&& RB.velocity.magnitude < 0.1f && RB.angularVelocity.magnitude < .5f) {
			UnFlip();
		}
	}

	void PlaceWheelChildren (WheelCollider wheel, Transform model) {
		RaycastHit hit;

		// Draw a line form the wheel down.  If it hits the ground, put the wheel model there.
		if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out hit, wheel.suspensionDistance + wheel.radius))
			wheel.transform.GetChild(0).position = hit.point + wheel.transform.up * wheel.radius;
		else // Wheel too far from ground.  Place objects at the end of the suspension range instead.
			wheel.transform.GetChild(0).position = wheel.transform.position - (wheel.transform.up * wheel.suspensionDistance);

		// Rotate the wheel model
		model.Rotate(0f, 0f, wheel.rpm / 6f, Space.Self);
	}

	void ControlParticles (WheelCollider wheel, ParticleSystem particles) {
		if (wheel.isGrounded && Mathf.Abs(speed) > 5f) {
			particles.Play();
		} else
			particles.Stop();
	}

	void UnFlip () {
		RB.velocity = transform.position.normalized * 10f;
		RB.angularVelocity = transform.forward * 3f;
	}

	void OnTriggerEnter (Collider coll) {
		if (coll.tag == "Respawn") {
			Respawn();
		}
	}

	void OnTriggerStay (Collider coll) {
		if (coll.tag == "Ground") {
			positionSaveTimer -= Time.fixedDeltaTime;
			if (positionSaveTimer < 0f) {
				positionSaveTimer = positionSaveDelay;
				savedPosition[1] = savedPosition[0];
				savedPosition[0] = transform.position;
			}
		}
	}

	public void Respawn() {
		if (positionSaveTimer > positionSaveDelay) { // If you respawned recently already
			positionSaveTimer = positionSaveDelay * 2f;
			transform.position = savedPosition[1] + (savedPosition[1].normalized * 2f);
			transform.LookAt(-(savedPosition[0]-savedPosition[1]), savedPosition[1].normalized);
		} else {
			positionSaveTimer = positionSaveDelay * 2f;
			transform.position = savedPosition[0] + (savedPosition[0].normalized * 2f);
			transform.LookAt(savedPosition[1], savedPosition[0].normalized);
		}

		RB.velocity = Vector3.zero;
		RB.angularVelocity = Vector3.zero;
	}
}
