using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public short parity;
    public short parityFlag = 0;
    public Vector2 tilePosition;
    public Transform tileTransform;
    private bool isTreasure = false;

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

    public Vector2 getTilePosition(){
        return this.tilePosition;
    }

    public void instantiateTile(Transform[] tiles, int i, int j){
        int rnd = Random.Range (1, 5);
        this.tileTransform = Instantiate (tiles [randomTile()], this.tilePosition, tiles [0].rotation);
        this.tileTransform.Rotate(new Vector3(0, 0, 90*rnd));
		this.tileTransform.parent = GameObject.FindGameObjectWithTag ("TileContainer").GetComponent<Transform>();
        this.tileTransform.name = "Tile (" + i + ", " + j +")";
        this.tileTransform.tag = "tile";
    }

    public void addEdge(Transform edge, int rot){
        Vector3 position = new Vector3(0,0,0);
        Vector3 rotate = new Vector3(0,0, 90*rot);
        float edgeInc = - edge.GetComponent<SpriteRenderer>().bounds.size.y/2;
        switch(rot){
            case 0: position = new Vector3(this.tileTransform.position.x, this.tileTransform.position.y + this.tileTransform.GetComponent<SpriteRenderer>().bounds.size.y/2 + edgeInc, -1); break;
            case 1: position = new Vector3(this.tileTransform.position.x + this.tileTransform.GetComponent<SpriteRenderer>().bounds.size.x/2 + edgeInc, this.tileTransform.position.y, -1); break;
            case 2: position = new Vector3(this.tileTransform.position.x, this.tileTransform.position.y - this.tileTransform.GetComponent<SpriteRenderer>().bounds.size.y/2 + edgeInc, -1); break;
            case 3: position = new Vector3(this.tileTransform.position.x - this.tileTransform.GetComponent<SpriteRenderer>().bounds.size.x/2 + edgeInc, this.tileTransform.position.y, -1); break;
        }
        Transform edgeT = Instantiate(edge, position, Quaternion.identity);
        edgeT.Rotate(rotate);
        edgeT.name = "Edge @ " + position;
        edgeT.GetComponent<SpriteRenderer>().enabled = true;
        edgeT.parent = this.tileTransform;
    }

    private int randomTile(){
        int rnd = Random.Range (1, 42);
        if(rnd <= 10) return 0;
        else if (rnd <= 20) return 1;
        else if (rnd <= 30) return 2;
        else if (rnd <= 31) return 3;
        else if (rnd <= 41) return 4;
        return -1;
    }

    public void drawTile(){
        if (this.parity == 1) {
            this.tileTransform.GetComponent<SpriteRenderer>().enabled = true;
            this.tileTransform.GetComponent<BoxCollider2D>().enabled = true;
        } else if (this.parity == 0 && !this.isTreasure){
            this.tileTransform.GetComponent<SpriteRenderer>().enabled = false;
            this.tileTransform.GetComponent<BoxCollider2D>().enabled = false;
        } else if (this.isTreasure){
            this.tileTransform.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public short getFlag(){
        return this.parityFlag;
    }

    public void deathRule(int deathRate){
        if(this.parityFlag <= deathRate) this.parity = 0;
        else this.parity = 1;
    }

    public void birthRule(int birthRate){
        if(this.parityFlag >= birthRate) this.parity = 1;
        else this.parity = 0;
    }

    public void updateParity(int birthRate, int deathRate){
        if(this.parity == 1) deathRule(deathRate);
        else if(this.parity == 0) birthRule(birthRate);
        this.parityFlag = 0;
    }

    public void instantiateTreasure(Transform treasure, int i, int j){
        this.tileTransform = Instantiate (treasure, this.tilePosition, treasure.rotation);
        this.tileTransform.name = "Treasure @ (" + i + ", " + j +")";
        this.tileTransform.parent = GameObject.FindGameObjectWithTag ("treasureContainer").GetComponent<Transform>();
        this.tileTransform.tag = "treasure";
        isTreasure = true;
    }

    public bool treasure(){
        return this.isTreasure;
    }
}
