using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Car : MonoBehaviour {

	public float throttlePos = 0f;
	public float brakePos = 0f;
	public bool handbrakePos = false;
	public float steerTarget = 0f;
	private float steerPos = 0f;

	public bool allWheelDrive = true;

	[SerializeField] WheelCollider[] steeringWheels;
	[SerializeField] WheelCollider[] staticWheels;
	[SerializeField] AnimationCurve driveTorque;
	[SerializeField] float brakeTorque = 5000f;
	[SerializeField] float handbrakeTorque = 10000f;
	[SerializeField] float maxSteerSpeed = 5f;
	[SerializeField] float maxSteerAngle = 30;

	[HideInInspector] public Rigidbody RB;
	[HideInInspector] public float speed;

	void Start () {
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
		}
		foreach (WheelCollider wheel in staticWheels) {
			wheel.motorTorque = 0f;
			wheel.brakeTorque = 0f;
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
		Physics.gravity = transform.position.normalized * -9.8f;

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
			wheel.motorTorque = Mathf.Lerp(0f, driveTorque.Evaluate(wheel.rpm * wheel.radius), throttlePos);
		}

		foreach (WheelCollider wheel in staticWheels) {
			//Make wheels touch the ground
			PlaceWheelChildren(wheel, wheel.transform.GetChild(0));

			// Applies normal analog brakes if appropriate.  This is like the brake pedal on a normal car.
			wheel.brakeTorque = Mathf.Lerp(0f, brakeTorque, brakePos);

			// Applies the handbrake if appropriate
			if (handbrakePos)
				wheel.brakeTorque = handbrakeTorque;

			// If using 4-wheel drive, the back wheels need torque as well
			if (allWheelDrive)
				wheel.motorTorque = 0.1f * Mathf.Lerp(0f, driveTorque.Evaluate(wheel.rpm * wheel.radius), throttlePos);
			else
				wheel.motorTorque = 0f;
		}
	}

	void PlaceWheelChildren (WheelCollider wheel, Transform model) {
		RaycastHit hit;

		// Draw a line form the wheel down.  If it hits the ground, put the wheel model there.
		if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out hit, wheel.suspensionDistance + wheel.radius))
			wheel.transform.GetChild(0).position = hit.point + wheel.transform.up * wheel.radius;
		else // Wheel too far from ground.  Place objects at the end of the suspension range instead.
			wheel.transform.GetChild(0).position = wheel.transform.position - (wheel.transform.up * wheel.suspensionDistance);
	}
}
