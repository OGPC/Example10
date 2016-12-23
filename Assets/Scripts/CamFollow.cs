using UnityEngine;

public class CamFollow : MonoBehaviour {

	public Transform follow;
	public float strength = 0.1f;

	void Start () {
		transform.position = follow.position;
		transform.rotation = Quaternion.LookRotation(follow.forward, transform.position);
	}
	
	void FixedUpdate () {
		transform.position = Vector3.Lerp(transform.position, follow.position, strength);
		Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.Cross(follow.position, follow.forward), follow.position), follow.position);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, strength);
	}
}
