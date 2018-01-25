using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public short parity;
    private short parityFlag = 0;
    public Vector2 tilePosition;
    public Transform tileTransform;

    private Tile[] neighbours = new Tile[8];
    private short neighbourCount = 0;

	public Tile()
	{
	}

    public short getIsWall(){
        return this.parity;
    }

    public void setIsWall(short isWall){
        this.parity = isWall;
    }

    public void setTilePosition(Vector2 position){
        this.tilePosition = position;
    }

    public void instantiateTile(Transform[] tiles, int i, int j){
        this.tileTransform = Instantiate (tiles [Random.Range (0, 7)], this.tilePosition, tiles [0].rotation);
		this.tileTransform.parent = tiles [0].parent;
        this.tileTransform.name = "Tile (" + i + ", " + j +")";
    }

    public void drawTile(){
        if (this.parity == 1) {
            this.tileTransform.GetComponent<SpriteRenderer>().enabled = true;
            this.tileTransform.GetComponent<BoxCollider2D>().enabled = true;
        } else if (this.parity == 0){
            this.tileTransform.GetComponent<SpriteRenderer>().enabled = false;
            this.tileTransform.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void addNeighbour(Tile neighbour){
        this.neighbours[this.neighbourCount] = neighbour;
        this.neighbourCount++;
    }

    public void updateFlag(){
        for(int i = 0; i < 8; i++){
            this.parityFlag += this.neighbours[i].getIsWall();
        }
    }

    public void deathRule(int deathRate){
        if(this.parityFlag < deathRate) this.parity = 0;
        else this.parity = 1;
    }

    public void birthRule(int birthRate){
        if(this.parityFlag > birthRate) this.parity = 1;
        else this.parity = 0;
    }

    public void updateParity(int birthRate, int deathRate){
        if(this.parity == 1) deathRule(deathRate);
        if(this.parity == 0) birthRule(birthRate);
        this.parityFlag = 0;
    }
}
