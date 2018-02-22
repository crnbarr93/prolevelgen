using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ropeTiling : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float scale = Vector3.Distance(GetComponent<LineRenderer>().GetPosition(0), GetComponent<LineRenderer>().GetPosition(1));
		GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(scale, 1f);
	}
}
