using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MenuUI : MonoBehaviour {

	public Transform[] subMenuParents;
	public AudioMixer mixer;
	public float scrollAmount = 150f;
	public float lerpConst = 5f;
	public int currentMenu;
	AudioSource sound;
	string webPage = "";
	public bool paused = false;
	public bool canPause = false;

	void Start () {
		sound = GetComponent<AudioSource>();

		if (!canPause)
			foreach (Transform menu in subMenuParents) {
				menu.localPosition = Vector3.down * 2000f;
			}
		else
			subMenuParents[0].gameObject.SetActive(false);
	}

	void Update () {
		if (canPause) { // Pause menu activation
			if (Input.GetKeyUp(KeyCode.Escape))
				if (paused) Resume();
				else Pause();

		} else { // Not pause menu
			for (int i = 0; i < subMenuParents.Length; i++) {
			Transform pane = subMenuParents[i];
			if (i == currentMenu) {
				pane.localPosition = Vector3.Lerp(pane.localPosition, Vector3.zero, lerpConst * Time.deltaTime);
			} else {
				pane.localPosition = Vector3.Lerp(pane.localPosition, Vector3.down * 2000f, lerpConst * Time.deltaTime);
			}
		}
		}
	}

	public void Pause () {
		Time.timeScale = 0f;
		paused = true;
		subMenuParents[0].gameObject.SetActive(true);
	}

	IEnumerator WaitForResume () {
		while (!Input.GetKeyUp(KeyCode.Escape)) {
			yield return null;
		}
		Resume();
	}

	public void Resume () {
		Time.timeScale = 1f;
		paused = false;
		subMenuParents[0].gameObject.SetActive(false);
	}

	public void GoToMainMenu () {
		Time.timeScale = 1f;
		SceneManager.LoadScene(0);
	}

	public void StartGame () {
		SceneManager.LoadScene(1);
	}

	public void ClickStart () {
		sound.Play();
		GetComponent<MoviePlayer>().PlayMovie(0);
	}

	public void SwitchSubMenu (int newMenu) {
		sound.Play();
		currentMenu = newMenu;
		subMenuParents[currentMenu].localPosition = Vector3.right * 2000f;
	}

	public void Graphics (int change) {
		sound.Play();

		if (change == 1)
			QualitySettings.IncreaseLevel(true);
		else if (change == -1)
			QualitySettings.DecreaseLevel(true);
	}

	public void Audio (int mixerGroup) {
		// Music is 1, SFX is 2.

		string paramName = mixerGroup.ToString();
		float currentVol;

		if (mixer.GetFloat(paramName, out currentVol)){
			
			float newVol;
			if (currentVol == 0f)
				newVol = -80f;
			else
				newVol = 0f;

			mixer.SetFloat(paramName, newVol);
			PlayerPrefs.SetFloat("audio"+paramName, newVol);
		} else
			Debug.LogError("Mixer parameter \"" + paramName + "\" not set!");
		
		sound.Play();
	}

	public void LogoClick (int dialogMenu) {
		SwitchSubMenu(dialogMenu);
		webPage = "http://www.ogpc.info";
	}

	public void PeopleClick (int dialogMenu) {
		SwitchSubMenu(dialogMenu);
		webPage = "https://tms.ogpc.info/Games/Details/4277f7ee-6620-4637-af30-5c50a2ee259a";
	}

	public void OpenWebPage (int returnMenu) {
		Application.OpenURL(webPage);
		SwitchSubMenu(returnMenu);
	}

	public void Quit () {
		sound.Play();
		Application.Quit();
	}
}
