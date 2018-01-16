using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardMaster : MonoBehaviour {

	public static boardMaster bm;
	public int boardSize = 10;
	public int wallSeedPerc = 45;

	private int step;
	public int stepTotal = 7;

	public Transform[] tiles;
	public Transform bg; //background Transform
	public Tile[,] board;

	private float spriteWidth;
	private float spriteHeight;
	
	void Start () {
		board = new Tile[boardSize, boardSize];
		spriteWidth = tiles[0].GetComponent<SpriteRenderer> ().sprite.bounds.size.x;
		spriteHeight  = tiles [0].GetComponent<SpriteRenderer> ().sprite.bounds.size.y;

		if (bm == null) bm = GameObject.FindGameObjectWithTag ("BM").GetComponent<boardMaster>();
		
		step = 0;

		initialiseAssets(new Vector3 (0, 0, -30));
		createBoard ();
	}

	private	void Update() {
		if(step < stepTotal && Input.GetKeyDown("s")){
			caStep();
			step++;
		}
	}

	public void createBoard(){
		SpriteRenderer sRenderer;
		int rndWall = 0;
		Vector3 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2, 0);

		for (int i = 0; i < boardSize; i++) {
			for (int j = 0; j < boardSize; j++) {
				rndWall = Random.Range(0,100);
				
				board[i, j] = new Tile();
				board[i,j].setTilePosition(new Vector2(defaultPosition.x + (spriteWidth*i), defaultPosition.y + (spriteHeight*j)));
				
				if(i == 0 || i == boardSize-1 || j == 0 || j == boardSize-1) board[i,j].setIsWall(1);
				else if (rndWall < wallSeedPerc){
					board[i,j].setIsWall(1);
				}
				
				board[i,j].instantiateTile(tiles, i, j);
				board[i,j].drawTile();

				//Temporary spawn fix (REMOVE LATER)
				if(i == 1 && j == 1){
					initialisePlayer(defaultPosition);
				}
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
				addNeighbours(i,j);
				
				board[i,j].updateFlag();

				board[i,j].drawTile();
			}
		}
		for(int i = 1; i < boardSize - 1; i++){
			for (int j = 1; j < boardSize - 1; j++){
				board[i,j].updateParity(step);
			}
		}
	}

	public void initialiseAssets(Vector3 position){
		initialiseCamera(position);
		initialiseBackGround(position);
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
		bg.localScale = new Vector3(2.0f, 2.0f, 1.0f);
	}
}
