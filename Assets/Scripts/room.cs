using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public List<boardMaster.coordinate> edgeTiles;
	public Room()
	{
        this.edgeTiles = new List<boardMaster.coordinate>();
	}

    public void addEdgeTile(boardMaster.coordinate edge){
        if(!edgeTiles.Contains(edge)) edgeTiles.Add(edge);
    }

    public bool Equals(Room roomB){
        foreach(boardMaster.coordinate coord1 in edgeTiles){
            foreach(boardMaster.coordinate coord2 in roomB.edgeTiles){
                if(coord1.Equals(coord2)) return true;
            }
        }
        return false;
    }
}
