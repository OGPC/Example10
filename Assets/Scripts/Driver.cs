using UnityEngine;

[RequireComponent(typeof(Car))]
public class Driver : MonoBehaviour {
	[HideInInspector] public Car car;

	public bool allowInput = true;
	[SerializeField] private float throttle;
	[SerializeField] private float steering;
	[SerializeField] private bool handbrake;

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
//		if (car.speed > 0f) { // driving forward
			if (throttle >= 0f) {
				car.throttlePos = throttle;
				car.brakePos = 0f;
			} else {
				car.brakePos = Mathf.Abs(throttle);
				car.throttlePos = 0f;
			}
/*		} else { // driving backward
			if (throttle <= 0f) {
				car.throttlePos = throttle;
				car.brakePos = 0f;
			} else {
				car.brakePos = Mathf.Abs(throttle);
				car.throttlePos = 0f;
			}
		}*/
	}
}
