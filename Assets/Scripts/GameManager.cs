using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject player;
	public GameObject pickupPrefab;
	public int pickupCount;
	List<GameObject> pickups = new List<GameObject>();
	float maxPlacementHeight = 300f;
	public float timeRemaining;
	bool timerDone = false;

	void Start () {
		// Place pickups
		GameObject pickupParent = new GameObject("Pickup Parent");
		RaycastHit hit;

		while (pickups.Count < pickupCount) {
			Vector3 raycastOrigin = Random.onUnitSphere;
			Physics.Raycast(raycastOrigin * maxPlacementHeight, -raycastOrigin, out hit, maxPlacementHeight);
			if (hit.collider.tag == "Ground") {
				GameObject newPickup = GameObject.Instantiate(pickupPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
				pickups.Add(newPickup);
				newPickup.transform.GetChild(1).LookAt(raycastOrigin * maxPlacementHeight);	// Child 1 is the beacon
				newPickup.name = "Pickup " + pickups.Count.ToString();
				newPickup.transform.parent = pickupParent.transform;
				newPickup.GetComponent<Pickup>().manager = this;
			}
		}

	}

	void Update () {
		if (timeRemaining == 0f) {
			if (!timerDone)	// If the time ran out and you haven't said so yet, call Lose()
				Lose();
		} else
			timeRemaining = Mathf.Max(0f, timeRemaining - Time.deltaTime);
	}

	public void GetPickup (GameObject pickup) {
		pickups.Remove(pickup);
		if (pickups.Count != 0)
			Debug.Log("Got pickup! only " + pickups.Count.ToString() + " remaining!");
		else {
			Win();
		}
	}

	public void Win () {
		Debug.Log("You win!");
	}

	public void Lose () {
		timerDone = true;
		timeRemaining = 0f;
		Debug.Log("Out of time! You lose.");
	}

}
