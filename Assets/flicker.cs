using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flicker : MonoBehaviour {

	// Use this for initialization
	[SerializeField]
	float threshold = 0.0f;
	[SerializeField]
	float amplitude = 1.0f;

	[SerializeField]
	float phase = 0.0f;

	[SerializeField]
	float frequency = 0.5f;


	float intensity;
	Light light;
	void Start () {
		light = GetComponent<Light>();
		intensity = light.intensity;
	}
	
	// Update is called once per frame
	void Update () {
		light.intensity = intensity*(sinWave());
	}

	float sinWave(){
		float x = (Time.time + phase) * frequency;
		float y;

		x = x-Mathf.Floor(x);
		y = Mathf.Sin(x * 2 * Mathf.PI);

		float newColor = Mathf.Clamp((y*amplitude), threshold, 1f);

		return newColor;
	}
}
