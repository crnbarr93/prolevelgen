using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardMaster : MonoBehaviour {

	public static boardMaster bm;
	public int boardSize = 10;

	public int stepTotal = 7;

	public Transform[] tiles;
	public Transform bg; //background Transform
	public Tile[,] board;

	private bool[,] floodBoard;
	private int floodCount;
	private int maxFloodCount;

	private Vector3 spawnPosition;

	private float spriteWidth;
	private float spriteHeight;

	public int birthRate = 4;
	public int deathRate = 3;
	
	void Start () {
		initialiseAssets();
		for(int i = 6; i < 8; i++){
			for(int j = 6; j < 8; j++){
				birthRate = i;
				deathRate = j;
				print("Rates: (" + birthRate + ", " + deathRate + ")");
				generateLevel();
			}
		}
	}

	public void generateLevel(){
		createBoard ();

		for(int step = 0; step < stepTotal; step++){
			caStep();
		}

		floodAndSpawn();
		drawTiles();
	}
	
	public void floodAndSpawn(){
		Vector3 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2, 0);

		for(int i = 0; i < boardSize; i++){
			for (int j = 0; j < boardSize; j++)
			if(board[i,j].getIsWall() == 0 && !floodBoard[i,j]){
				flood(i,j);

				if(floodCount > maxFloodCount){
					maxFloodCount = floodCount;
					
					spawnPosition = new Vector3(defaultPosition.x + (i*spriteWidth), defaultPosition.y + (j*spriteHeight), 0);
				}

				floodCount = 0;
			}
		}

		print("Spawn position: " + spawnPosition + " Room size: " + maxFloodCount);
		initialisePlayer(spawnPosition);
	}

	public void createBoard(){
		int rndWall = 0;
		Vector3 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2, 0);
		board = new Tile[boardSize, boardSize];

		for (int i = 0; i < boardSize; i++) {
			for (int j = 0; j < boardSize; j++) {
				bool border = false;
				rndWall = Random.Range(0,100);
				
				board[i, j] = new Tile();
				floodBoard[i,j] = false;

				board[i,j].setTilePosition(new Vector2(defaultPosition.x + (spriteWidth*i), defaultPosition.y + (spriteHeight*j)));
				
				if(i == 0 || i == boardSize-1 || j == 0 || j == boardSize-1) border = true;
				
				if (rndWall < 40 || border){
					board[i,j].setIsWall(1);
				}
			
				board[i,j].instantiateTile(tiles, i, j);
			}
		}

		for(int i = 1; i < boardSize-1; i++){
			for(int j = 1; j < boardSize-1; j++){
				addNeighbours(i,j);
			}
		}

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
				board[i,j].drawTile();
			}
		}
	}
	public void flood(int i, int j){
		if(floodBoard[i,j]) return;
		if(board[i,j].getIsWall() == 1) return;

		floodBoard[i,j] = true;
		floodCount++;

		flood(i-1, j);
		flood(i+1, j);
		flood(i, j-1);
		flood(i, j+1);

		return;
	}

	public void initialiseAssets(){
		initialiseCamera(new Vector3 (0, 0, -30));
		initialiseBackGround(new Vector3 (0, 0, -30));
		
		floodBoard = new bool[boardSize, boardSize];
		floodCount = 0;
		maxFloodCount = 0;

		spriteWidth = tiles[0].GetComponent<SpriteRenderer> ().sprite.bounds.size.x;
		spriteHeight  = tiles [0].GetComponent<SpriteRenderer> ().sprite.bounds.size.y;

		if (bm == null) bm = GameObject.FindGameObjectWithTag ("BM").GetComponent<boardMaster>();
	}

	public void initialisePlayer(Vector3 position){
		GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position = position;
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
}
