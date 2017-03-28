using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public struct cutScene {
	public MovieTexture tex;
	public GameObject go;
	public bool closeAfter;
	public UnityEvent endAction;
}

public class MoviePlayer : MonoBehaviour {
	
	public bool playingMovie = false;
	public int movieSelected;
	public GameObject videoParent;
	public cutScene[] movTexture;
	private int vsyncprevious;

	void Update () {
		if (playingMovie && !movTexture[movieSelected].tex.isPlaying) {
			movTexture[movieSelected].endAction.Invoke();
			StopMovie(movieSelected);
		}
	}

	public void PlayMovie (int index) {
		movieSelected = index;
		vsyncprevious = QualitySettings.vSyncCount;
		QualitySettings.vSyncCount = 0;
		Time.captureFramerate = 30;
		movTexture[index].tex.Play();
		movTexture[index].go.SetActive(true);
		videoParent.SetActive(true);
		playingMovie = true;
	}

	public void StopMovie (int index) {
		movieSelected = index;
		movTexture[index].tex.Stop();
		QualitySettings.vSyncCount = vsyncprevious;
		Time.captureFramerate = 0;
		movTexture[index].go.SetActive(false);
		if (movTexture[index].closeAfter) {
			videoParent.SetActive(false);
		}
		playingMovie = false;
	}

}
