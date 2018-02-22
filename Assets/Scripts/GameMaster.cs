using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;
	private static boardMaster bm;
	public Transform spawnPoint;
	public GameObject player;
	public GameObject camera;
	private int cameraFlag = 1;

	public Text scoreText;
	private GameObject[] treasures;
	private int treasureCount;
	public short treasureScore;

	void Start() {
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();
		}
		if (player == null){
			player = GameObject.FindGameObjectWithTag("Player");
		}
		if (bm == null) {
			bm = GameObject.FindGameObjectWithTag ("BM").GetComponent<boardMaster>();
		}

		camera = GameObject.FindGameObjectWithTag("overviewCamera");

		treasureCount = 0;
		treasureScore = 0;
	}

	public Transform playerPrefab;
	public int spawnDelay = 2;

	void Update(){
		if (player != null){
			updateSpawnPosition();
		}else{
			player = GameObject.FindGameObjectWithTag("Player");
		}

		if (Input.GetKeyDown("r"))
			SceneManager.LoadScene(1);

		if (Input.GetKeyDown ("q"))
			SceneManager.LoadScene(0);

		if(Input.GetKeyDown ("k")){
			Destroy(player);
			gm.StartCoroutine(gm.Respawn());
		}

		if(Input.GetKeyDown("l")) 
			swapCamera();

		updateScore();

		collectTreasure();


	}

	public IEnumerator  Respawn (){
		yield return new WaitForSeconds (spawnDelay);

		Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);
	}

	public static void KillPlayer(Player player_class) {
		Destroy (player_class.gameObject);
		gm.StartCoroutine(gm.Respawn ());
	}

	private void updateSpawnPosition(){
		Transform playerPoint = player.GetComponent<Transform>();
		Transform spawnPoint = GameObject.FindGameObjectWithTag("SP").GetComponent<Transform>();

		if (player.GetComponent<Rigidbody2D>().velocity.y == 0 && spawnPoint.position != playerPoint.position){
			spawnPoint.position = playerPoint.position;
		}
	}

	public void restartGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
	}

	public void exitGame() {
		print("Qutting game. Bye!");
		Application.Quit ();
	}

	public void swapCamera() {
		if (cameraFlag == 1){
			camera.GetComponent<Camera>().enabled = false;
			camera = GameObject.FindGameObjectWithTag("overviewCamera");
			camera.GetComponent<Camera>().enabled = true;
			cameraFlag = 0;
		}else{
			camera.GetComponent<Camera>().enabled = false;
			camera = GameObject.FindGameObjectWithTag("MainCamera");
			camera.GetComponent<Camera>().enabled = true;
			cameraFlag = 1;
		}
	}

	public void collectTreasure(){
		foreach(GameObject treasure in treasures){
			if(!treasure.Equals(null)){
				float treasureHeight = treasure.GetComponent<SpriteRenderer>().bounds.size.x/2;
				float treasureWidth = treasure.GetComponent<SpriteRenderer>().bounds.size.y/2;
				if((player.transform.position.x >= treasure.transform.position.x - treasureHeight && player.transform.position.x <= treasure.transform.position.x + treasureHeight) && (player.transform.position.y >= treasure.transform.position.y - treasureWidth && player.transform.position.y <= treasure.transform.position.y + treasureWidth)){
					treasureScore += 1;
					Destroy(treasure);
					print("Treasure collected @ :" + player.transform.position);
				} 
			}
		}

		//if(treasureScore == treasureCount) SceneManager.LoadScene(1);
	}

	public void updateScore(){
		scoreText.text = treasureScore + "/" + treasureCount;
	}

	public void countTreasure(){
		treasures = GameObject.FindGameObjectsWithTag("treasure");
		
		treasureCount = bm.treasureCoords.Count;
	}

}
