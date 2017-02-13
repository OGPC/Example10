using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

	public GameManager manager;
	bool pickedUp = false;
	public AnimationCurve scaleAnim;
	public float animationLength = 1f;	// Seconds
	Vector3 initScale;
	float initLum;
	Light pointLight;
	float animTime = 0f;

	void Start () {
		initScale = transform.GetChild(0).localScale;
		pointLight = transform.GetChild(0).GetChild(0).GetComponent<Light>();
		initLum = pointLight.intensity;
	}

	void OnTriggerEnter (Collider other) {
		if (!pickedUp && other.tag == "Player") {
			pickedUp = true;
			if (manager != null)
				manager.GetPickup(this.gameObject);
			else
				Debug.LogWarning(name + " was not properly initialized!");
			Destroy(transform.GetChild(1).gameObject);	// Beacon

		}
	}

	void FixedUpdate() {
		if (pickedUp) {
			transform.GetChild(0).localScale = initScale * scaleAnim.Evaluate(animTime);
			animTime += Time.fixedDeltaTime/animationLength;
			pointLight.intensity = initLum * (1f - animTime);
		}

		if (animTime > 1f)
			Destroy(this.gameObject);
	}
}
