using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Car))]
public class Driver : MonoBehaviour {
	[HideInInspector] public Car car;

	public bool allowInput = true;
	public float throttle;
	public float steering;
	public bool boost;

	public float brakesCutoffVelocity = 3f;

	void Start() {
		// This is safe without checking for success because of line 3.
		car = GetComponent<Car>();
	}

	void FixedUpdate () {
		// Allows for easy debugging/testing
		if (allowInput) {
			steering = Input.GetAxis("Steer");
			throttle = Input.GetAxis("Throttle");
			boost = Input.GetButton("Boost");
		}

		// Same behavior regardless of motion
		car.steerTarget = steering;
		car.boostPos = boost;
		
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
	}
}
