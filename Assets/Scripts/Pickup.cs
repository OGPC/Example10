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
	AudioSource AS;

	void Start () {
		AS = GetComponent<AudioSource>();
		initScale = transform.GetChild(2).localScale;
		pointLight = transform.GetChild(1).GetComponent<Light>();
		initLum = pointLight.intensity;
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			PickUp();
		}
	}

	void FixedUpdate( ) {
		if (pickedUp) {
			transform.GetChild(1).localScale = initScale * scaleAnim.Evaluate(animTime);
			animTime += Time.fixedDeltaTime/animationLength;
			pointLight.intensity = initLum * (1f - animTime);
		}

		if (animTime > 1f && !AS.isPlaying)
			Destroy(this.gameObject);
	}

	public void PickUp () {
		if (pickedUp) return;

		AS.PlayOneShot(AS.clip);
		pickedUp = true;
		if (manager != null)
			manager.GetPickup(this.gameObject);
		else
			Debug.LogWarning(name + " was not properly initialized!");
		Destroy(transform.GetChild(0).gameObject);	// Beacon
	}
}
