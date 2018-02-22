using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileList : MonoBehaviour {

    private TileElement head;

	public TileList(Tile head)
	{
        this.head = new TileElement(head);
	}

    public TileElement getTail(){
        TileElement iter = head;

        while(iter.next != null){
            iter = iter.next;
        }
        
        return iter;
    }

    public void append(Tile tile){
        TileElement tail = this.getTail();

        TileElement newTile = new TileElement(tile);
        newTile.setPrev(tail);
        tail.setNext(newTile);
    }
}
