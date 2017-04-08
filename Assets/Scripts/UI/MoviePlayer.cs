using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public struct cutScene {
	public MovieTexture tex;
	public GameObject go;
	public bool closeAfter;
	public UnityEvent endAction;
	public AudioClip audio;
}

[RequireComponent(typeof(AudioSource))]
public class MoviePlayer : MonoBehaviour {
	
	public bool playingMovie = false;
	public int movieSelected;
	public GameObject videoParent;
	public cutScene[] movTexture;
	private int vsyncprevious;
	AudioSource AS;

	void Start () {
		AS = GetComponent<AudioSource>();
	}

	void Update () {
		if (playingMovie) {

			if (Input.anyKeyDown)
				StopMovie();

			if (!movTexture[movieSelected].tex.isPlaying) {
				movTexture[movieSelected].endAction.Invoke();
				StopMovie();
			}
		}
	}

	public void PlayMovie (int index = -1) {
		if (index != -1)
			movieSelected = index;

		vsyncprevious = QualitySettings.vSyncCount;
		QualitySettings.vSyncCount = 0;
		//Time.captureFramerate = 60;
		movTexture[movieSelected].tex.Play();
		movTexture[movieSelected].go.SetActive(true);
		videoParent.SetActive(true);
		if (movTexture[movieSelected].audio != null) {
			playingMovie = true;
			AS.outputAudioMixerGroup = null;
			AS.clip = movTexture[movieSelected].audio;
			AS.PlayDelayed(7f);
		}
	}

	public void StopMovie (int index = -1) {
		if (index != -1)
			movieSelected = index;

		movTexture[movieSelected].tex.Stop();
		QualitySettings.vSyncCount = vsyncprevious;
		Time.captureFramerate = 0;
		movTexture[movieSelected].go.SetActive(false);
		if (movTexture[movieSelected].closeAfter) {
			videoParent.SetActive(false);
		}
		playingMovie = false;
	}

}
