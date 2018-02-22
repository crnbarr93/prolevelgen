using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grapplingHook : MonoBehaviour {

	DistanceJoint2D joint2D;
	Vector3 targetPos;
	RaycastHit2D cast;

	[SerializeField]
	float distance = 10f;

	[SerializeField]
	LayerMask mask;
	float jointDistance = 0;

	[SerializeField]
	LineRenderer hook;
	void Start () {
		joint2D = GetComponent<DistanceJoint2D>();
		joint2D.enabled = false;
	}
	void Update () {

		if(joint2D.distance > 1f && Input.GetKey(KeyCode.W)) joint2D.distance -= 8.0f*Time.deltaTime;
		if(joint2D.distance < jointDistance && Input.GetKey(KeyCode.S)) joint2D.distance += 8.0f*Time.deltaTime;

		if(Input.GetButtonDown("Fire1") && !joint2D.enabled){
			targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			targetPos.z = 0;

			cast = Physics2D.Raycast(transform.position, targetPos - transform.position, distance, mask);
			if(cast.collider != null && cast.collider.gameObject.GetComponent<BoxCollider2D>() != null && cast.point.y > transform.position.y){
				joint2D.enabled = true;
				joint2D.connectedBody = cast.collider.gameObject.GetComponent<Rigidbody2D>();
				joint2D.connectedAnchor = cast.point - new Vector2(cast.collider.transform.position.x, cast.collider.transform.position.y);
				jointDistance = Vector2.Distance(transform.position, cast.point);
				joint2D.distance = jointDistance;

				hook.SetPosition(0, transform.position);
				hook.SetPosition(1, cast.point);
				hook.enabled=true;
			}
		} else if (Input.GetKeyDown(KeyCode.Mouse0) && joint2D.enabled) {
			joint2D.enabled=false;
			hook.enabled = false;
		}
		
		if(joint2D.enabled){ 
			hook.SetPosition(0, transform.position);
		}
		
	}
}
