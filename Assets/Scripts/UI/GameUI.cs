using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

	public Car car;
	public Text speedometer;
	GameManager gameManager;
	public Text timer;
	
	void Start () {
		gameManager = GetComponent<GameManager>();
	}

	void Update () {
		string speedometerText = Mathf.Round(Mathf.Abs(car.speed)).ToString();
		speedometer.text = speedometerText;

		float timeRemaining = Mathf.Floor(gameManager.timeRemaining);
		float seconds = timeRemaining % 60f;
		float minutes = Mathf.Floor(timeRemaining / 60f);
		timer.text = string.Format("{0}:{1}", minutes, seconds.ToString().PadLeft(2,'0'));
	}
}
