using UnityEngine;

public class CamFollow : MonoBehaviour {

	public Transform follow;
	public float strength = 0.1f;

	void Start () {
		transform.position = follow.position;
		transform.rotation = follow.rotation;
	}
	
	void FixedUpdate () {
		transform.position = Vector3.Lerp(transform.position, follow.position, strength);
		transform.rotation = Quaternion.Lerp(transform.rotation, follow.rotation, strength);
	}
}
