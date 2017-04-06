using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioToggleHelper : MonoBehaviour {

	public AudioMixer mixer;
	public string controlledParam;
	public float onValue = 0f, offValue = -80f;

	public Image background;
	public Color offColor;
	Color onColor;

	void Start () {
		onColor = background.color;

		float loadedVal = PlayerPrefs.GetFloat("audio"+controlledParam, 0f);
		
		if (!mixer.SetFloat(controlledParam, loadedVal))
			Debug.LogError("No \"" + controlledParam + "\" exposed parameter on mixer " + mixer.name);
	}
	
	void Update () {
		float currentVal;
		if (mixer.GetFloat(controlledParam, out currentVal)) {
			if (currentVal == onValue)
				background.color = onColor;
			else if (currentVal == offValue)
				background.color = offColor;
		} else
			Debug.LogError("No \"" + controlledParam + "\" exposed parameter on mixer " + mixer.name);
	}
}
