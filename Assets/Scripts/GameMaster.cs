using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;
	public Transform spawnPoint;
	private GameObject player;
	private GameObject camera;
	private int cameraFlag = 1;

	void Start() {
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();
		}
		if (player == null){
			player = GameObject.FindGameObjectWithTag("Player");
		}

		camera = GameObject.FindGameObjectWithTag("MainCamera");
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
			restartGame();

		if (Input.GetKeyDown ("q"))
			exitGame ();

		if(Input.GetKeyDown ("k")){
			Destroy(player);
			gm.StartCoroutine(gm.Respawn());
		}

		if(Input.GetKeyDown("l")) 
			swapCamera();

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

}
