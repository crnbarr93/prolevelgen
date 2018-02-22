using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileElement : MonoBehaviour {

    public Tile tile;
    public TileElement next;
    public TileElement prev;

	public TileElement(Tile tile)
	{
        this.tile = tile;
        this.next = null;
        this.prev = null;
	}

    public void setPrev(TileElement prev){
        this.prev = prev;
    }

    public void setNext(TileElement next){
        this.next = next;
    }
}
