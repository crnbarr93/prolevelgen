using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boardMaster : MonoBehaviour {

	public static boardMaster bm;
	public static GameMaster gm;
	private GameObject player;
	public int boardSize = 10;

	public int stepTotal = 7;

	private List<Vector2> roomCoords;

	public Transform[] tiles;
	public Transform bg; //background Transform
	public Transform border;

	public Tile[,] board;
	private bool[,] floodBoard;
	private int maxFloodCount;

	private Vector3 spawnPosition;

	private float spriteWidth;
	private float spriteHeight;

	public int birthRate = 4;
	public int deathRate = 3;
	
	private bool generating = true;

	void Start () {
		Stopwatch stopWatch = new Stopwatch();

		Destroy(player);
		initialiseVariables();
		stopWatch.Start();
		while(generating) generateLevel();
		drawTiles();
		stopWatch.Stop();
		gm.Respawn();
		initialiseObjects();
		print("Total time: " + stopWatch.Elapsed);
	}

	public void generateLevel(){
		createBoard ();

		for(int step = 0; step < stepTotal; step++){
			caStep();
		}

		floodAndSpawn();
		if(maxFloodCount < 0.45*(boardSize*boardSize) || maxFloodCount > 0.55*(boardSize*boardSize)){
			print("INVALID LEVEL! Regenerating");
			generateLevel();
		} 
		
		generating = false;
	}
	
	public void floodAndSpawn(){
		Vector2 currCoord;
		Vector2 maxCoord = new Vector2(0, 0);
		Vector3 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2, 0);

		for(int i = 0; i < boardSize; i++){
			for (int j = 0; j < boardSize; j++)
			if(board[i,j].getIsWall() == 0 && !floodBoard[i,j]){
				int floodCount = flood(i,j);
				currCoord = new Vector2(i,j);
				roomCoords.Add(currCoord);
				if(floodCount > maxFloodCount){
					maxFloodCount = floodCount;
					maxCoord = currCoord;
					
				}
			}
		}
		roomCoords.Remove(maxCoord);
		spawnPosition = new Vector3(defaultPosition.x + (maxCoord.x*spriteWidth), defaultPosition.y + (maxCoord.y*spriteHeight), 0);
		foreach (var coord in roomCoords) blockFill((int) coord.x, (int) coord.y);
	}

	public void createBoard(){
		int rndWall = 0;
		Vector3 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2, 0);

		board = new Tile[boardSize, boardSize];
		floodBoard = new bool[boardSize, boardSize];
		maxFloodCount = 0;
	
		roomCoords = new List<Vector2>();

		Stopwatch stopWatch = new Stopwatch();

		
		for (int i = 0; i < boardSize; i++) {
			for (int j = 0; j < boardSize; j++) {
				
				bool border = false;
				rndWall = Random.Range(0,100);
				

				stopWatch.Start();
				board[i, j] = gameObject.AddComponent<Tile>();
				stopWatch.Stop();
				floodBoard[i,j] = false;
				
				
				board[i,j].setTilePosition(new Vector2(defaultPosition.x + (spriteWidth*i), defaultPosition.y + (spriteHeight*j)));
				
				if(i == 0 || i == boardSize-1 || j == 0 || j == boardSize-1) border = true;
				
				if (rndWall < 40 || border){
					board[i,j].setIsWall(1);
				}
			}
		}
		

		for(int i = 1; i < boardSize-1; i++){
			for(int j = 1; j < boardSize-1; j++){
				addNeighbours(i,j);
			}
		}
		

		print("Board creation time: " + stopWatch.Elapsed);
	}

	public void addNeighbours(int i, int j){
		for(int k = -1; k < 2; k++){
			for (int l = -1; l < 2; l ++){
				if (!(l == 0 && k == 0)) board[i, j].addNeighbour(board[i+k, j+l]);
			}
		}
	}

	public void caStep(){
		for(int i = 1; i < boardSize - 1; i++){
			for (int j = 1; j < boardSize - 1; j++){
				board[i,j].updateFlag();
			}
		}
		for(int i = 1; i < boardSize - 1; i++){
			for (int j = 1; j < boardSize - 1; j++){
				board[i,j].updateParity(birthRate, deathRate);
			}
		}
	}

	public void drawTiles(){
		for(int i = 0; i < boardSize; i++){
			for(int j = 0; j< boardSize; j++){
				board[i,j].instantiateTile(tiles, i, j);
				board[i,j].drawTile();
			}
		}
	}
	public int flood(int i, int j){
		int floodC = 1;
		if(floodBoard[i,j]) return 0;
		if(board[i,j].getIsWall() == 1) return 0;

		floodBoard[i,j] = true;

		if(board[i-1,j].getIsWall() == 0) floodC += flood(i-1, j);
		
		if(board[i+1,j].getIsWall() == 0) floodC += flood(i+1, j);

		if(board[i,j-1].getIsWall() == 0) floodC += flood(i, j-1);

		if(board[i,j+1].getIsWall() == 0) floodC += flood(i, j+1);

		return floodC;
	}

	public void blockFill(int i, int j){
		if(board[i,j].getIsWall() == 1) return;

		board[i,j].setIsWall(1);

		blockFill(i-1, j);
		blockFill(i+1, j);
		blockFill(i, j-1);
		blockFill(i, j+1);

		return;
	}

	public void initialiseVariables(){
		if (gm == null)
			gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();
		if (player == null)
			player = GameObject.FindGameObjectWithTag("Player");
		if (bm == null)
			bm = GameObject.FindGameObjectWithTag ("BM").GetComponent<boardMaster>();

		spriteWidth = tiles[0].GetComponent<SpriteRenderer> ().sprite.bounds.size.x;
		spriteHeight  = tiles [0].GetComponent<SpriteRenderer> ().sprite.bounds.size.y;

		
	}

	public void initialiseObjects(){
		initialiseCamera(new Vector3 (0, 0, -30));
		initialiseBackGround(new Vector3 (0, 0, -30));
		print("Spawn position: " + spawnPosition + " Room size: " + maxFloodCount);
		initialisePlayer(spawnPosition);
	}

	public void initialisePlayer(Vector3 position){
		gm.player.GetComponent<Transform>().position = position;
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>().position = position;
	}

	public void initialiseCamera(Vector3 position){
		GameObject.FindGameObjectWithTag("overviewCamera").GetComponent<Transform>().position = position;
		GameObject.FindGameObjectWithTag("overviewCamera").GetComponent<Camera>().orthographicSize = boardSize;
	}

	public void initialiseBackGround(Vector3 position){
		position.z = 20;
		bg.position = position;
		bg.localScale = new Vector3((float) boardSize/20.0f, (float) boardSize/20.0f, 1.0f);
	}

	public void restartGame() {
		SceneManager.LoadScene(2);
	}
}
