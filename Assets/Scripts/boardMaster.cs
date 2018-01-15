using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardMaster : MonoBehaviour {

	public static boardMaster bm;
	public int boardSize = 10;

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
		
		initialiseAssets(new Vector3 (0, 0, -30));
		createBoard ();
	}

	public void createBoard(){
		SpriteRenderer sRenderer;
		Vector3 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2, 0);

		for (int i = 0; i < boardSize; i++) {
			for (int j = 0; j < boardSize; j++) {
				board[i, j] = new Tile();

				board[i,j].setTilePosition(new Vector2(defaultPosition.x + (spriteWidth*i), defaultPosition.y + (spriteHeight*j)));
				board[i,j].instantiateTile(tiles);

				//Temporary spawn fix (REMOVE LATER)
				if(i == 1 && j == 1){
					initialisePlayer(defaultPosition);
				}
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
	}

	public void initialiseBackGround(Vector3 position){
		position.z = 20;
		bg.position = position;
		bg.localScale = new Vector3(2.0f, 2.0f, 1.0f);
	}
}
