using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject player;
	public GameObject[] pickupPrefabs;
	public int pickupCount;
	List<GameObject> pickups = new List<GameObject>();
	float maxPlacementHeight = 300f;
	public float timeRemaining = 300;
	bool timerDone = false;
	public AudioClip loseClip;
	public AudioClip winClip;
	public AudioClip story2;
	AudioSource AS;
	bool won;
	bool lost;

	void Start () {
		AS = player.GetComponent<AudioSource>();

		// Place pickups
		GameObject pickupParent = new GameObject("Pickup Parent");
		RaycastHit hit;

		while (pickups.Count < pickupCount) {
			Vector3 raycastOrigin = Random.onUnitSphere;
			Physics.Raycast(raycastOrigin * maxPlacementHeight, -raycastOrigin, out hit, maxPlacementHeight);
			if (hit.collider.tag == "Ground") {
				int prefabID = Mathf.FloorToInt(Random.Range(0f, pickupPrefabs.Length - 0.01f));
				GameObject newPickup = GameObject.Instantiate(pickupPrefabs[prefabID], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
				pickups.Add(newPickup);
				newPickup.transform.GetChild(0).LookAt(raycastOrigin * maxPlacementHeight);	// Child 0 is the beacon
				newPickup.name = pickupPrefabs[prefabID].name + "." + pickups.Count.ToString();
				newPickup.transform.parent = pickupParent.transform;
				newPickup.GetComponent<Pickup>().manager = this;
			}
		}

		AS.clip = story2;
		AS.Play();
	}

	void Update () {

		// Debug/cheat code. Press Home and End to get a pickup.
		if ((Input.GetKey(KeyCode.Home) && Input.GetKeyDown(KeyCode.End) || (Input.GetKeyDown(KeyCode.Home) && Input.GetKey(KeyCode.End)))) {
			Debug.Log("Combo hit at t " + Time.realtimeSinceStartup);
			int rand = Mathf.FloorToInt(Random.Range(0f, pickups.Count - 0.01f));
			pickups[rand].GetComponent<Pickup>().PickUp();
		}

		if (!won && timeRemaining == 0f) {
			if (!timerDone)	{// If the time ran out and you haven't said so yet, call Lose()
				timerDone = true;
				Lose();
			}
		} else
			timeRemaining = Mathf.Max(0f, timeRemaining - Time.deltaTime);

		if ((won || lost) && !AS.isPlaying) {
			AudioClip clip = won?winClip:loseClip;
			if (AS.clip != clip) {
				AS.clip = clip;
				AS.PlayDelayed(1f);
			} else
				SceneManager.LoadScene(0);
		}
	}

	public void GetPickup (GameObject pickup) { // Don't call this yourself. This is just for Pickup to call.
		pickups.Remove(pickup);
		player.GetComponent<Car>().GotPickup(pickups.Count);
		if (pickups.Count != 0)
			Debug.Log("Got pickup! only " + pickups.Count.ToString() + " remaining!");
		else {
			Win();
		}
	}

	public void Win () {
		won = true;
	}

	public void Lose () {
		if (won)
			return;
		lost = true;
	}

}
