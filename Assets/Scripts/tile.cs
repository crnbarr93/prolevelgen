using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int parity;
    public int tileSprite;
    public Vector2 tilePosition;
    public Transform tileTransform;
    private int parityFlag;
    private Tile[] neighbours;
    private int neighbourCount = 0;

	public Tile()
	{
        this.parity = 1;
        this.tileSprite = Random.Range (0, 7);
        this.tilePosition = new Vector2(0, 0);
        this.parityFlag = 0;
        this.neighbours = new Tile[8];
	}

	public int getTileSprite(){
        return this.tileSprite;
    }

    public int getIsWall(){
        return this.parity;
    }

    public void setTileSprite(int tileSprite){
        this.tileSprite = tileSprite;
    }

    public void setIsWall(int isWall){
        this.parity = isWall;
    }

    public void printTileInfo(){
        print("Sprite no.: " + this.tileSprite + " Is wall?: " + this.parity);
    }

    public void setTilePosition(Vector2 position){
        this.tilePosition = position;
    }

    public Vector2 getTilePositon(){
        return this.tilePosition;
    }

    public void instantiateTile(Transform[] tiles){
        if (this.parity == 1) {
            this.tileTransform = Instantiate (tiles [this.tileSprite], this.tilePosition, tiles [0].rotation);
		    this.tileTransform.parent = tiles [0].parent;
            this.tileTransform.GetComponent<SpriteRenderer>().enabled = true;
        } else print("Cannot instantiate non-wall tile at:" + this.tilePosition);
    }

    public SpriteRenderer getSRenderer(){
        if (this.tileTransform != null) return this.tileTransform.GetComponent<SpriteRenderer>();
        else return null;
    }

    public void addNeighbour(Tile neighbour){
        this.neighbours[this.neighbourCount] = neighbour;
        this.neighbourCount++;
    }

    public void updateFlag(){
        for(int i = 0; i < 8; i++){
            if(neighbours[i] != null){
                this.parityFlag += neighbours[i].getIsWall();
            }    
        }
    }

    public void updateParity(int step){
        if (this.parityFlag > 4 || (this.parityFlag <= 1 && step <= 4)) this.parity = 1;
        else this.parity = 0;
        this.neighbourCount = 0;
        this.parityFlag = 0;
    }
}
