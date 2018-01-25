using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public TileList ceiling;
    public TileList floor;
    public TileList leftWall;
    public TileList rightWall;

	public Room()
	{
	}

    public void appendCeiling(Tile tile){
        ceiling.append(tile);
    }

    public void appendLeftwall(Tile tile){
        leftWall.append(tile);
    }
}
