using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour {

	public int offsetX = 2; //Offset to avoid errors

	//Used to check buddy existance
	public bool hasARightBuddy = false; 
	public bool hasALeftBuddy = false;

	//Used if object is not tilable
	public bool reverseScale = false;

	//Width of texture
	private float spriteWidth = 0f;
	private Camera cam;
	private Transform myTransform;

	void Awake(){
		cam = Camera.main;
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		SpriteRenderer sRenderer = GetComponent<SpriteRenderer> ();
		spriteWidth = sRenderer.sprite.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
		//buddy check
		if (!hasALeftBuddy || !hasARightBuddy) {
			//calculate camera extend (half width) of what camera can see in world coordinates
			float camHorizontalExtend = (cam.orthographicSize * Screen.width) / Screen.height;

			//calculate x position where camera can see edge of sprite
			float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth/2) - camHorizontalExtend;
			float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;

			if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasARightBuddy) {
				makeNewBuddy (1);
				hasARightBuddy = true;
			} else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasALeftBuddy) {
				makeNewBuddy (-1);
				hasALeftBuddy = true;
			}
		}
	}

	//Function to create new buddy
	void makeNewBuddy (int rightOrLeft){
		//Calculating new buddy position
		int space = Random.Range(1,6);

		Vector3 newPosition = new Vector3 (myTransform.position.x + spriteWidth * space * rightOrLeft, myTransform.position.y, myTransform.position.z);
		//instantiating new buddy
		Transform newBuddy = (Transform) Instantiate (myTransform, newPosition, myTransform.rotation);

		//if not tilable reverse x size of object to make it tile correctly
		if (reverseScale) {
			newBuddy.localScale = new Vector3 (newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
		}

		newBuddy.parent = myTransform.parent;
		if (rightOrLeft > 0) {
			newBuddy.GetComponent<Tiling> ().hasALeftBuddy = true;
		} else {
			newBuddy.GetComponent<Tiling> ().hasARightBuddy = true;
		}
	}
}
