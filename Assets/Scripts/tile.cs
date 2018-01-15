using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public bool isWall;
    public int tileSprite;
    public Vector2 tilePosition;
    public Transform tileTransform;

	public Tile()
	{
        this.isWall = true;
        this.tileSprite = Random.Range (0, 7);
        this.tilePosition = new Vector2(0, 0);
	}

	public int getTileSprite(){
        return this.tileSprite;
    }

    public bool getIsWall(){
        return this.isWall;
    }

    public void setTileSprite(int tileSprite){
        this.tileSprite = tileSprite;
    }

    public void setIsWall(bool isWall){
        this.isWall = isWall;
    }

    public void printTileInfo(){
        print("Sprite no.: " + this.tileSprite + " Is wall?: " + this.isWall);
    }

    public void setTilePosition(Vector2 position){
        this.tilePosition = position;
    }

    public Vector2 getTilePositon(){
        return this.tilePosition;
    }

    public void instantiateTile(Transform[] tiles){
        if (this.isWall) {
            this.tileTransform = Instantiate (tiles [this.tileSprite], this.tilePosition, tiles [0].rotation);
		    this.tileTransform.parent = tiles [0].parent;
            this.tileTransform.GetComponent<SpriteRenderer>().enabled = true;
        } else print("Cannot instantiate non-wall tile at:" + this.tilePosition);
    }

    public SpriteRenderer getSRenderer(){
        if (this.tileTransform != null) return this.tileTransform.GetComponent<SpriteRenderer>();
        else return null;
    }
}
