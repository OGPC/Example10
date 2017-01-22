using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Car))]
public class Driver : MonoBehaviour {
	[HideInInspector] public Car car;

	public bool allowInput = true;
	[SerializeField] private float throttle;
	[SerializeField] private float steering;
	[SerializeField] private bool handbrake;

	public float brakesCutoffVelocity = 3f;

	void Start() {
		// This is safe without checking for success because of line 3.
		car = GetComponent<Car>();
	}

	void FixedUpdate () {
		// Allows for easy debugging/testing
		if (allowInput) {
			steering = Input.GetAxis("Horizontal");
			throttle = Input.GetAxis("Vertical");
			handbrake = Input.GetButton("Handbrake");
		}

		// Same behavior regardless of motion
		car.steerTarget = steering;
		car.handbrakePos = handbrake;
		
		// Behavior changes depending on car speed
		if (car.speed > brakesCutoffVelocity) {
			// Driving/braking forward
			if (throttle >= 0f) {
				car.throttlePos = throttle;
				car.brakePos = 0f;
			} else {
				car.brakePos = -throttle;
				car.throttlePos = 0f;
			}
		} else if (car.speed < -brakesCutoffVelocity) {
			// Driving/braking backward
			if (throttle <= 0f) {
				car.throttlePos = throttle;
				car.brakePos = 0f;
			} else {
				car.brakePos = throttle;
				car.throttlePos = 0f;
			}
		} else {
			// Slow driving with no brakes
			car.throttlePos = throttle;
			car.brakePos = 0f;
		}

		if (Input.GetKeyUp(KeyCode.Escape))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
