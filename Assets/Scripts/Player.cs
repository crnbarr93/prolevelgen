using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[System.Serializable]
	public class PlayerStats
	{
		public float Health = 100f;
	}

	public PlayerStats playerStats = new PlayerStats();

	void Update(){
		if (transform.position.y <= -10000) {
			GameMaster.KillPlayer (this);
		}
	}

	public void damagePlayer(int damage){
		if(playerStats.Health > damage){
			playerStats.Health -= damage;
		} else {
			GameMaster.KillPlayer (this);
		}
	}
}
