using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

	public Transform[] subMenuParents;
	public float scrollAmount = 150f;
	public float lerpConst = 0.1f;
	public int currentMenu;

	void Start () {
		foreach (Transform menu in subMenuParents) {
			menu.localPosition = Vector3.down * 2000f;
		}
		
	}

	void Update () {
		for (int i = 0; i < subMenuParents.Length; i++) {
			Transform pane = subMenuParents[i];
			if (i == currentMenu) {
				pane.localPosition = Vector3.Lerp(pane.localPosition, Vector3.zero, lerpConst);
			} else {
				pane.localPosition = Vector3.Lerp(pane.localPosition, Vector3.down * 2000f, lerpConst);
			}
		}
	}

	public void StartGame () {
		SceneManager.LoadScene(1);
	}

	public void SwitchSubMenu (int newMenu) {
		currentMenu = newMenu;
		subMenuParents[currentMenu].localPosition = Vector3.right * 2000f;
	}

	public void GraphicsToggle () {
		Debug.Log("Graphics toggled");
	}

	public void AudioToggle () {
		Debug.Log("Audio toggled");
	}

	public void LogoClick () {
		Debug.Log("Logo clicked");
	}

	public void PeopleClick () {
		Debug.Log("People clicked");
	}

	public void Quit () {
		Application.Quit();
	}
}
