using UnityEngine;

public class CamFollow : MonoBehaviour {

	public Transform follow;
	public float transStrength = 20f;
	public float rotStrength = 2f;

	void Start () {
		transform.position = follow.position;
		transform.rotation = Quaternion.LookRotation(follow.forward, transform.position);
	}
	
	void FixedUpdate () {
		// Move the camera to the position of the car, but actually only move it a certain percentage (transStrength) of the way there.
		transform.position = Vector3.Lerp(transform.position, follow.position, transStrength * Time.fixedDeltaTime);

		// Define a vector that you want the camera to roughly point along. It's made by combining the
		// forward direction and the way it's traveling, but velocity should only matter to a point.
		Vector3 camForward = follow.forward + (Vector3.ClampMagnitude(follow.GetComponent<Rigidbody>().velocity, 10f));

		// Do a ton of vector and matrix math to pick the direction to look in. If you really want details, contact OGPC I guess.
		Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.Cross(follow.position, camForward), follow.position), follow.position);

		// Rotate the camera to the desired rotation by a certain percentage (rotStrength).
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotStrength * Time.fixedDeltaTime);
	}
}
