using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

	public Car car;
	public Text speedometer;
	
	void Update () {
		string speedometerText = Mathf.Round(car.speed).ToString();
		speedometer.text = speedometerText;

		if (Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene(0);
	}
}
