using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Flicker : MonoBehaviour {

	float min = 0f, max;
	Light lightEmitter;

	void Start () {
		lightEmitter = GetComponent<Light>();
		max = lightEmitter.intensity;
	}
	
	void Update () {
		lightEmitter.intensity = Random.Range(min, max);
	}
}
