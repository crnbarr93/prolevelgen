using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {

	public Transform[] backgrounds;	 // Array of all the back/foregrounds to be parallaxed
	private float[] parallaxScales; //Proportion of camera's movement for backgrounds
	public float smoothing = 1;		 //Parallax smoothing (>0)

	private Transform cam;			//References main camera
	private Vector3 previousCamPos;	//Position of camera in previous frame

	//Called before Start
	void Awake () {
		//Camera reference init
		cam = Camera.main.transform;
	}

	void Start () {
		//Previous frame stores current frame's camera position
		previousCamPos = cam.position;

		//Assigning correspending parallax scales
		parallaxScales = new float[backgrounds.Length];
		for (int i = 0; i < backgrounds.Length; i++) {
			parallaxScales [i] = backgrounds[i].position.z * -1;

		}
	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < backgrounds.Length; i++) {
			//Parallax is opposite of camera movement due to previous frame multiplied by scale
			float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
			float parallaxy = (previousCamPos.y - cam.position.y) * parallaxScales [i];

			//set target x position which is current pos + parallax
			float backgroundTargetPosX = backgrounds[i].position.x + parallax;
			float backgroundTargetPosY = backgrounds [i].position.y + parallaxy;

			//Create target position which is backgrounds current position with target x position
			Vector3 backgroundTargetPos = new Vector3 (backgroundTargetPosX, backgroundTargetPosY, backgrounds[i].position.z);

			// fade between current position and target position

			backgrounds [i].position = Vector3.Lerp (backgrounds [i].position, backgroundTargetPos, smoothing * Time.deltaTime);
		}

		// set previous cam pos to camera's position at end of frame
		previousCamPos = cam.position;
	}
}
