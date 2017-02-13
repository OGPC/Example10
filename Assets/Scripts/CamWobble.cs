using UnityEngine;

public class CamWobble : MonoBehaviour {

	public float camRotConst = 0.00001f;
	public float camWobbleConst = 5f;
	public float camWobbleTimeConst = 0.25f;
	public float lerpConst = 0.1f;

	Transform cam;
	Vector3 wobbleTime;

	void Start () {
		Cursor.visible = true;
		cam = transform.GetChild(0);
		wobbleTime.x = Random.Range(0, 2 * Mathf.PI);
		wobbleTime.y = Random.Range(0, 2 * Mathf.PI);
		wobbleTime.z = Random.Range(0, 2 * Mathf.PI);
	}

	void Update () {

		//Cam wobble
		cam.localRotation = Quaternion.FromToRotation(Vector3.forward,
			(Input.mousePosition * camRotConst) + Vector3.forward) *
			Quaternion.Euler(
				Mathf.Sin(wobbleTime.x) * camWobbleConst,
				Mathf.Sin(wobbleTime.y) * camWobbleConst,
				Mathf.Sin(wobbleTime.z) * camWobbleConst);
		wobbleTime.x += Random.Range(0.75f, 1.25f) * camWobbleTimeConst * Time.deltaTime;
		wobbleTime.y += Random.Range(0.75f, 1.25f) * camWobbleTimeConst * Time.deltaTime;
		wobbleTime.z += Random.Range(0.75f, 1.25f) * camWobbleTimeConst * Time.deltaTime;
	}
}
