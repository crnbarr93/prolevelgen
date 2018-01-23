using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int parity;
    private int parityFlag;

    public int tileSprite;
    public Vector2 tilePosition;
    public Transform tileTransform;

    private Tile[] neighbours;
    private int neighbourCount = 0;

    private bool[] borders;

	public Tile()
	{
        this.parity = 0;
        this.tileSprite = Random.Range (0, 7);
        this.tilePosition = new Vector2(0, 0);
        this.parityFlag = 0;
        this.neighbours = new Tile[8];
        this.borders = new bool[4];
        for(int i = 0; i < 4; i++) borders[i] = false;
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

    public void instantiateTile(Transform[] tiles, int i, int j){
        this.tileTransform = Instantiate (tiles [this.tileSprite], this.tilePosition, tiles [0].rotation);
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

    public void setRoof(){
        borders[0] = true;
    }

    public bool isRoof(){
        return borders[0];
    }

    public void setLeftWall(){
        borders[1] = true;
    }

    public bool isLeftWall(){
        return borders[1];
    }

    public void setFloor(){
        borders[2] = true;
    }

    public bool isFloor(){
        return borders[2];
    }

    public void setRightWall(){
        borders[3] = true;
    }

    public bool isRightWall(){
        return borders[3];
    }
}
