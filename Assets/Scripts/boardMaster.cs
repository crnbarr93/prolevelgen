using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boardMaster : MonoBehaviour {
	[SerializeField]
	bool drawTunnelLines;

	public static GameMaster gm;
	public int boardSize = 10;

	[SerializeField]
	int stepTotal = 7;
	public List<coordinate> treasureCoords;
	List<Room> allRooms;

	[SerializeField]
	Transform edge;
	public Transform[] tiles;

	[SerializeField]
	Transform bg; //background Transform

	[SerializeField]
	Transform treasure;
	[SerializeField]
	Transform lighting;

	Tile[,] board;
	bool[,] floodBoard;
	coordinate spawnCoord;

	Vector3 spawnPosition;

	public float spriteWidth;
	public float spriteHeight;

	[SerializeField]
	int birthRate = 4;

	[SerializeField]
	int deathRate = 3;
	
	bool generating = true;

	Room currRoom;


	public struct coordinate {
        public int i;
		public int j;

		 public coordinate(int i, int j){
			 this.i = i;
			 this.j = j;
		 }

		 public string toString(){
			 return "(" + this.i + ", " + this.j + ")";
		 }

		 public bool Equals(coordinate comp){
			 return (this.i == comp.i && this.j == comp.j);
		 }
     }   

	void Start () {
		Stopwatch stopWatch = new Stopwatch();

		initialiseVariables();

		stopWatch.Start();
		generateLevel(); //while(generating) 
		drawTiles();
		gm.countTreasure();
		initialiseObjects();
		gm.Respawn();
		stopWatch.Stop();

		UnityEngine.Debug.Log("Total time: " + stopWatch.Elapsed);
	}

	void generateLevel(){
		createBoard ();

		for(int step = 0; step < stepTotal; step++){
			caStep();
		}

		floodRooms();
		spawnTreasure();
		cleanUp();
		connectRooms();
		spawnLighting();
		

		if(treasureCoords.Count < Mathf.Floor(boardSize/10)){
			string reason = "";
			if(treasureCoords.Count < Mathf.Floor(boardSize/10)) reason = "Not enough treasure";

			print("INVALID LEVEL! " + reason);
			foreach(GameObject treasure in GameObject.FindGameObjectsWithTag("treasure")) if(treasure != null) Destroy(treasure);
			foreach(GameObject tile in GameObject.FindGameObjectsWithTag("tile")) if(tile != null) Destroy(tile);
			return;
		} 

		generating = false;
	}
	
	void floodRooms(){
		coordinate currCoord;
		int maxFloodCount = 0;
		for(int i = 0; i < boardSize; i++){
			for (int j = 0; j < boardSize; j++)
			{
				if(board[i,j].getIsWall() == 0 && !floodBoard[i,j]){
					currRoom = new Room();
					int floodCount = flood(i,j);
					currCoord = new coordinate(i, j);

					if(floodCount < 50) {
						floodFill(i, j);
						continue;
					} else {
						allRooms.Add(currRoom);
					}

					if(floodCount > maxFloodCount) {
						maxFloodCount = floodCount;
						spawnCoord = currCoord;
					}
				}
			}
		}
		spawnPosition = coordToWorldPoint3D(spawnCoord.i, spawnCoord.j);
		
	}

	void connectRooms(){
		List<Room> connectedRooms = new List<Room>();

		foreach(Room roomA in allRooms){
			if(containsRoom(connectedRooms, roomA)){
				continue;
			} else {
				Room closestRoom = new Room();
				double closestDistance = Mathf.Pow(boardSize, 2);
				bool foundConnection = false;

				List<coordinate> closestCoordinates = new List<coordinate>();
				List<Room> explored = new List<Room>();
				explored.Add(roomA);

				foreach(Room roomB in allRooms){
					if(roomA.Equals(roomB) || (!containsRoom(connectedRooms, roomB) && connectedRooms.Count != 0)){
						continue;
					} else {
						List<coordinate> closestRoomCoordinates = findClosestCoords(roomA.edgeTiles, roomB.edgeTiles);

						double roomDistance = euclideanDistance(closestRoomCoordinates[0], closestRoomCoordinates[1]);
						if(roomDistance < closestDistance){
							closestDistance = roomDistance;
							closestRoom = roomB;
							closestCoordinates = closestRoomCoordinates;
							foundConnection = true;
						}
					}
				}

				if(foundConnection){
						connectedRooms.Add(roomA);
						connectedRooms.Add(closestRoom);
						tunnel(closestCoordinates[0], closestCoordinates[1]);
				}
			}
		}
		
	}

	List<coordinate> findClosestCoords(List<coordinate> coordsA, List<coordinate> coordsB){
		double closestDistance = Mathf.Pow(boardSize, 2);
		List<coordinate> closestCoordinates = new List<coordinate>();

		foreach(coordinate coord1 in coordsA){
			foreach(coordinate coord2 in coordsB){
				double distance = euclideanDistance(coord1, coord2);
				if(distance < closestDistance){
					closestDistance = distance; 
					closestCoordinates = new List<coordinate>();
					closestCoordinates.Add(coord1);
					closestCoordinates.Add(coord2);
				}
			}
		}
		return closestCoordinates;
	}

	void tunnel(coordinate coordA, coordinate coordB){
		if(drawTunnelLines) UnityEngine.Debug.DrawLine(coordToWorldPoint3D(coordA.i, coordA.j), coordToWorldPoint3D(coordB.i, coordB.j), Color.green, 10000.0f, false);
		print("TUNNELING " + coordA.toString() + " , " + coordB.toString());
		int xDistance = coordB.i - coordA.i;
		int yDistance = coordB.j - coordA.j;

		int xShift = 0;
		int yShift = 0;

		int xInc = (int) Mathf.Sign(xDistance);
		int yInc = (int) Mathf.Sign(yDistance);

		while(Mathf.Abs(xShift) < Mathf.Abs(xDistance) || Mathf.Abs(yShift) < Mathf.Abs(yDistance)){
			if(Mathf.Abs(xShift) < Mathf.Abs(xDistance)){
				xShift += xInc;
				board[coordA.i + (xShift), coordA.j + (yShift)].setIsWall(0);
			}
			if(Mathf.Abs(yShift) < Mathf.Abs(yDistance)){
				yShift += yInc;
				board[coordA.i + (xShift), coordA.j + (yShift)].setIsWall(0);
			}
		}
	}

	void createBoard(){
		int rndWall = 0;
		spawnCoord = new coordinate(0, 0);
		board = new Tile[boardSize, boardSize];
		floodBoard = new bool[boardSize, boardSize];
	

		treasureCoords = new List<coordinate>();
		allRooms = new List<Room>();

		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Reset();
		stopWatch.Start();
		for (int i = 0; i < boardSize; i++) {
			for (int j = 0; j < boardSize; j++) {
				
				bool border = false;
				rndWall = Random.Range(0,100);
				
				board[i, j] = new Tile();
				stopWatch.Stop();
				floodBoard[i,j] = false;
				
				
				board[i,j].setTilePosition(coordToWorldPoint2D(i, j));
				
				if(i == 0 || i == boardSize-1 || j == 0 || j == boardSize-1) border = true;
				
				if (rndWall < 38 || border){
					board[i,j].setIsWall(1);
				}
			}
		}

		print("Board creation time: " + stopWatch.Elapsed);
	}

	void updateParities(int i, int j){
		for(int k = -1; k < 2; k++){
			for (int l = -1; l < 2; l ++){
				if (!(l == 0 && k == 0)) board[i, j].parityFlag += board[i+k, j+l].parity;
			}
		}
	}

	void caStep(){
		for(int i = 1; i < boardSize - 1; i++){
			for (int j = 1; j < boardSize - 1; j++){
				updateParities(i,j);
			}
		}
		for(int i = 1; i < boardSize - 1; i++){
			for (int j = 1; j < boardSize - 1; j++){
				board[i,j].updateParity(birthRate, deathRate);
			}
		}
	}

	void drawTiles(){
		for(int i = 0; i < boardSize; i++){
			for(int j = 0; j< boardSize; j++){
				if(board[i,j].treasure() || board[i,j].parity != 0) {
					if(!board[i,j].treasure()) board[i,j].instantiateTile(tiles, i, j);
					board[i,j].drawTile();
				}
			}
		}
		drawEdges();
	}

	void drawEdges(){
		for(int i = 0; i < boardSize; i++){
			for(int j = 0; j < boardSize; j++){
				if(board[i,j].parity == 1){
					if((i+1) < boardSize) if(board[i+1, j].parity == 0) board[i,j].addEdge(edge, 1);
					if((i-1) >= 0) if(board[i-1, j].parity == 0) board[i,j].addEdge(edge, 3);
					if((j+1) < boardSize) if(board[i, j+1].parity == 0) board[i,j].addEdge(edge, 0);
					if((j-1) >= 0) if(board[i, j-1].parity == 0) board[i,j].addEdge(edge, 2);
				}
			}
		}
	}

	int flood(int i, int j){
		int floodC = 1;
		if(floodBoard[i,j]) return 0;
		if(board[i,j].getIsWall() == 1) return 0;

		floodBoard[i,j] = true;

		if(board[i-1,j].getIsWall() == 0) {
			floodC += flood(i-1, j);
		} else {
			currRoom.addEdgeTile(new coordinate(i, j));
		}
		
		if(board[i+1,j].getIsWall() == 0) {
			floodC += flood(i+1, j);
		} else {
			currRoom.addEdgeTile(new coordinate(i, j));
		}

		if(board[i,j-1].getIsWall() == 0) {
			floodC += flood(i, j-1);
		} else {
			currRoom.addEdgeTile(new coordinate(i, j));
		}

		if(board[i,j+1].getIsWall() == 0) {
			floodC += flood(i, j+1);
		} else {
			currRoom.addEdgeTile(new coordinate(i, j));
		}

		return floodC;
	}

	void floodFill(int i, int j){
		if(board[i,j].getIsWall() == 1) return;

		board[i,j].setIsWall(1);

		floodFill(i-1, j);
		floodFill(i+1, j);
		floodFill(i, j-1);
		floodFill(i, j+1);

		return;
	}

	void spawnTreasure(){
		List<coordinate> tempList = new List<coordinate>();
		for(int i = 1; i < boardSize - 1; i++){
			for (int j = 1; j < boardSize - 1; j++){

				bool proximity = true;
				coordinate coord_ = new coordinate(i,j);
				foreach(var coord in treasureCoords){
					if(euclideanDistance(coord_, coord) <= 4.0f) proximity = false;
					if(euclideanDistance(coord_, spawnCoord) <= 4.0f) proximity = false;
					if(coord_.i == spawnCoord.i && coord_.j == spawnCoord.j) proximity = false;
				} 

				if(!proximity) {
					continue;
				} else {
					int count = 0;
					int[] axis = new int[2] {-1, 1};
					
					foreach(var k in axis){
						count+= board[i+k, j].getIsWall();
						count+= board[i, j+k].getIsWall();
					}

					if(count == 3 && board[i,j].getIsWall() == 0) {
						treasureCoords.Add(new coordinate(i, j));
					}
				}
			}
		}

		foreach(var coord in treasureCoords){
			foreach(var coord2 in treasureCoords){
				if(!coord.Equals(coord2) && !tempList.Contains(coord) && !tempList.Contains(coord2) && (euclideanDistance(coord, coord2) <= Mathf.Sqrt(18)) && (treasureCoords.Count - tempList.Count) > Mathf.Floor(boardSize/10f)) {
					tempList.Add(coord2);
				}
			}
		} while(treasureCoords.Count - tempList.Count > Mathf.Floor(boardSize/10) + 5){
			int rnd = Random.Range (0, treasureCoords.Count);
			coordinate remove = treasureCoords[rnd];
			if(!tempList.Contains(remove)) tempList.Add(remove);
		}
		foreach(var coord in tempList) treasureCoords.Remove(coord);
		foreach(var coord in treasureCoords) board[coord.i, (int) coord.j].instantiateTreasure(treasure, coord.i, coord.j);
	}

	void spawnLighting(){
		int lightCount = 0;
		while(lightCount < boardSize/5){
			int i = Random.Range(1,boardSize-1);
			int j = Random.Range(1,boardSize-1);
			if(board[i,j].getIsWall() == 0) {
				Transform light = Instantiate(lighting, new Vector3(board[i,j].getTilePosition().x, board[i,j].getTilePosition().y, -4), lighting.rotation);
				light.name = "Light " + lightCount;
				lightCount += 1;
			}
		}
	}

	void cleanUp(){
		for(int i = 1; i+1 < boardSize; i++){
			for(int j = 1; j+1 < boardSize; j++){
				bool tooth = checkForTeeth(i,j);
				if(tooth) print("tooth for (" + i + ", " + j + " ): " + tooth);
				if(tooth && !board[i,j].treasure() && (i != spawnCoord.i && j != spawnCoord.j)){ //Check to see if the current cell is involved in creating the "teeth effect"  & is not a treasure cell
					int fillProbability = Random.Range(0,100);
					print(i + ", " + j + ", prob: " + fillProbability);
					if(fillProbability >= 100){
						continue;
					} else {
						board[i,j].setIsWall(1);
					}
				}
			}
		}
	}

	bool checkForTeeth(int i, int j){
		int[,] neighbours = new int[3,3]; //Moore neighbourhood
		for(int x = -1; x <= 1; x++){
			for(int y = -1; y <= 1; y++){
				neighbours[x+1,y+1] = board[i+x,j+y].getIsWall();
			}
		}

		//Case 1, roof teeth
		if(neighbours[0, 0] == 0 && neighbours[0, 1] == 1 && neighbours[0, 2] == 1 && neighbours[1, 0] == 0 && neighbours[1, 1] == 0 && neighbours[1, 2] == 1 && neighbours[2, 0] == 0 && neighbours[2, 1] == 1 && neighbours[2, 2] == 1) return true;
		//Case 2, right wall teeth
		if(neighbours[0, 0] == 0 && neighbours[0, 1] == 0 && neighbours[0, 2] == 0 && neighbours[1, 0] == 1 && neighbours[1, 1] == 0 && neighbours[1, 2] == 1 && neighbours[2, 0] == 1 && neighbours[2, 1] == 1 && neighbours[2, 2] == 1) return true;
		//Case 3, floor teeth
		if(neighbours[0, 0] == 1 && neighbours[0, 1] == 1 && neighbours[0, 2] == 0 && neighbours[1, 0] == 1 && neighbours[1, 1] == 0 && neighbours[1, 2] == 0 && neighbours[2, 0] == 1 && neighbours[2, 1] == 1 && neighbours[2, 2] == 0) return true;
		//Case 4, left wall teeth
		if(neighbours[0, 0] == 1 && neighbours[0, 1] == 1 && neighbours[0, 2] == 1 && neighbours[1, 0] == 1 && neighbours[1, 1] == 0 && neighbours[1, 2] == 1 && neighbours[2, 0] == 0 && neighbours[2, 1] == 0 && neighbours[2, 2] == 0) return true;

		return false;
	}

	double euclideanDistance(coordinate u, coordinate v){
		return Mathf.Sqrt(Mathf.Abs(Mathf.Pow(u.i - v.i, 2) + Mathf.Pow(u.j - v.j, 2)));
	}

	void initialiseVariables(){
		if (gm == null)
			gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();

		spriteWidth = tiles[0].GetComponent<SpriteRenderer> ().sprite.bounds.size.x;
		spriteHeight  = tiles [0].GetComponent<SpriteRenderer> ().sprite.bounds.size.y;

		foreach(var tile in tiles){
			tile.GetComponent<Transform> ().localScale = new Vector3(spriteWidth/tile.GetComponent<SpriteRenderer> ().sprite.bounds.size.x, spriteHeight/tile.GetComponent<SpriteRenderer> ().sprite.bounds.size.y, 1);
		}
		edge.GetComponent<Transform>().localScale = new Vector3(spriteWidth/edge.GetComponent<SpriteRenderer> ().sprite.bounds.size.x, 1, 1);
		
	}

	void initialiseObjects(){
		initialiseCamera(new Vector3 (0, 0, -30));
		initialiseBackGround(new Vector3 (0, 0, -30));
		print("Spawn position: " + spawnPosition);
		initialisePlayer(spawnPosition);
	}

	void initialisePlayer(Vector3 position){
		gm.player.GetComponent<Transform>().position = position;
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>().position = position;
	}

	void initialiseCamera(Vector3 position){
		//GameObject.FindGameObjectWithTag("overviewCamera").GetComponent<Transform>().position = position;
		//GameObject.FindGameObjectWithTag("overviewCamera").GetComponent<Camera>().orthographicSize = boardSize;
	}

	void initialiseBackGround(Vector3 position){
		position.z = 20;
		bg.position = position;
		bg.localScale = new Vector3((float) boardSize/5.0f, (float) boardSize/2.0f, 1.0f);
	}

	Vector3 coordToWorldPoint3D(int i, int j){
		Vector2 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2);
		Vector3 worldPoint = new Vector3(defaultPosition.x + (i*spriteWidth), defaultPosition.y + (j*spriteHeight), -2);

		return worldPoint;
	}

	Vector3 coordToWorldPoint2D(int i, int j){
		Vector3 defaultPosition = new Vector3 ((-spriteWidth*boardSize)/2, (-spriteHeight*boardSize)/2);
		Vector3 worldPoint = new Vector3(defaultPosition.x + (i*spriteWidth), defaultPosition.y + (j*spriteHeight));

		return worldPoint;
	}

	bool containsRoom(List<Room> list, Room room){
		foreach(var listRoom in list){
			if(room.Equals(listRoom)){
				return true;
			}
		}
		return false;
	}
}
