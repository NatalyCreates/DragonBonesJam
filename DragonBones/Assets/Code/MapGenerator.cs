using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour {

	enum TileLayer {
		FLOOR,
		WALL,
		FOG,
	};
	enum TileType {
		FLOOR_NONE,
		FLOOR,
		FLOOR_BONES,
		FLOOR_ROOM,
		WALL,
		WALL_BONES,
		WALL_ROOM,
		WALL_DRAGON,
		WALL_ROCK,
		FOG,
		FOG_ROCK,
	};

	int uniqueIdCounter = 0;

	struct TileLayered {
		public int _id;
		public bool _drawn;
		public int _row;
		public int _col;
		public int _typeFloor;
		public int _typeWall;
		public GameObject _prefabFloor;
		public GameObject _prefabWall;
		public GameObject _prefabUnknown;
		public float _posX;
		public float _posY;

		public TileLayered (int id, int row, int col, float posX, float posY) {
			_id = id;
			_row = row;
			_col = col;
			_posX = posX;
			_posY = posY;
			_typeFloor = -1;
			_typeWall = -1;
			_prefabFloor = null;
			_prefabWall = null;
			_prefabUnknown = null;
			_drawn = false;
		}

		public void SetTypeAndCreate (int typeFloor, int typeWall) {
			_typeFloor = typeFloor;
			_typeWall = typeWall;
			_drawn = true;
		}
	}

	GameObject tilePrefab;
	public GameObject floorTilePrefab;
	public GameObject wallTilePrefab;
	public GameObject fogTilePrefab;

	float xpos;
	float ypos;
	Vector2 position;
	GameObject newTile;

	int sortingCount = -1;

	List<List<TileLayered>> tileMap =  new List<List<TileLayered>>();

	//Tile tileScriptRef;

	// Use this for initialization
	void Start () {
		xpos = 0;
		ypos = 0;
		position = new Vector2(xpos, ypos);
		GenerateMap(0);
	}

	public int GetNextId() {
		uniqueIdCounter++;
		return uniqueIdCounter;
	}

	void IterateTileMapAndCreate() {
		for (int i = 0; i < tileMap.Count; i++) {
			for (int j = 0; j < tileMap[i].Count; j++) {
				Debug.Log ("Tile ID = " + tileMap[i][j]._id.ToString() + "");
				Debug.Log ("[" + tileMap[i][j]._row.ToString() + "," + tileMap[i][j]._col.ToString() + "] == [" + i.ToString() + "," + j.ToString() + "]");
				int id = tileMap[i][j]._id;
				AddTile(tileMap[i][j]._posX, tileMap[i][j]._posY, TileLayer.FLOOR, TileType.FLOOR_ROOM, id);
				AddTile(tileMap[i][j]._posX, tileMap[i][j]._posY, TileLayer.WALL, TileType.WALL_ROOM, id);
				AddTile(tileMap[i][j]._posX, tileMap[i][j]._posY, TileLayer.FOG, TileType.FOG, id);
				GameObject floorFind = GameObject.Find ("Tile Floor " + id.ToString());
				GameObject fogFind = GameObject.Find ("Tile Fog " + id.ToString());
				floorFind.transform.parent = GameObject.Find("Tile Wall " + id.ToString()).transform;
				fogFind.transform.parent = GameObject.Find("Tile Wall " + id.ToString()).transform;
				floorFind.name = "Floor";
				fogFind.name = "Fog";
			}
		}
	}

	void MakeDiamondMap(int rowLen = 6, int totalRows = 6) {
		int curRow, curTile;

		for (curRow = 0; curRow < totalRows; curRow++) {
			List<TileLayered> tempTileList = new List<TileLayered>();
			for (curTile = 0; curTile < rowLen; curTile++) {
				xpos = xpos + (DragonGame.tileWidth / 2);
				ypos = ypos - (DragonGame.tileHeight / 4);
				tempTileList.Add(new TileLayered(GetNextId(), curRow, curTile, xpos, ypos));
				//AddTile(xpos, ypos);
			}
			tileMap.Add(tempTileList);
			tempTileList = new List<TileLayered>();
			// move pointer back to start
			xpos = xpos - ((rowLen + 1) * (DragonGame.tileWidth / 2));
			ypos = ypos + ((rowLen - 1) * (DragonGame.tileHeight / 4));
		}
	}

	void MakeHexMap(int totalRows = 8, int sideLen = 5) {
		
		int rowIncRate = 2; // can only be even (for a symmetrical map)!! (usually 2 is best)
		
		int nextRowLen = sideLen;
		int curRow, curTile;
		
		for (curRow = 0; curRow < totalRows; curRow++) {
			for (curTile = 0; curTile < nextRowLen; curTile++) {
				xpos = xpos + (DragonGame.tileWidth / 2);
				ypos = ypos - (DragonGame.tileHeight / 4);
				//AddTile(xpos, ypos);
			}
			// move pointer back to start
			xpos = xpos - ((nextRowLen + 1) * (DragonGame.tileWidth / 2));
			ypos = ypos + ((nextRowLen - 1) * (DragonGame.tileHeight / 4));
			
			// even number of rows
			if (totalRows % 2 == 0) {
				// first half
				if (curRow < totalRows/2 - 1) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (DragonGame.tileWidth / 2);
						ypos = ypos + (DragonGame.tileHeight / 4);
					}
				}
				// first row of second half
				else if (curRow == totalRows/2 - 1) {
					// don't inc xpos, ypos or nextRowLen, since we want a few rows that are the same
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (DragonGame.tileWidth / 2);
						ypos = ypos - (DragonGame.tileHeight / 4);
					}
				}
			}
			// odd number of rows, total/2 is rounded down
			else {
				// first half
				if (curRow < totalRows/2) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (DragonGame.tileWidth / 2);
						ypos = ypos + (DragonGame.tileHeight / 4);
					}
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (DragonGame.tileWidth / 2);
						ypos = ypos - (DragonGame.tileHeight / 4);
					}
				}
			}
		}
	}

	void MakeOctMap(int totalRows = 15, int sideLen = 4) {
		
		int rowIncRate = 2; // can only be even (for a symmetrical map)!! (usually 2 is best)
		
		int nextRowLen = sideLen;
		int curRow, curTile;
		
		for (curRow = 0; curRow < totalRows; curRow++) {
			for (curTile = 0; curTile < nextRowLen; curTile++) {
				xpos = xpos + (DragonGame.tileWidth / 2);
				ypos = ypos - (DragonGame.tileHeight / 4);
				//AddTile(xpos, ypos);
			}
			// move pointer back to start
			xpos = xpos - ((nextRowLen + 1) * (DragonGame.tileWidth / 2));
			ypos = ypos + ((nextRowLen - 1) * (DragonGame.tileHeight / 4));
			
			// even number of rows
			if (totalRows % 2 == 0) {
				// first half
				if (curRow < totalRows/2 - sideLen/2) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (DragonGame.tileWidth / 2);
						ypos = ypos + (DragonGame.tileHeight / 4);
					}
				}
				// first row of second half
				else if ((curRow >= totalRows/2 - sideLen/2) && (curRow < totalRows/2 + sideLen/2 - 1)) {
					// don't inc xpos, ypos or nextRowLen, since we want a few rows that are the same
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (DragonGame.tileWidth / 2);
						ypos = ypos - (DragonGame.tileHeight / 4);
					}
				}
			}
			// odd number of rows, total/2 is rounded down
			else {
				// first half
				if (curRow < totalRows/2 - sideLen/2 + 1) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (DragonGame.tileWidth / 2);
						ypos = ypos + (DragonGame.tileHeight / 4);
					}
				}
				// first row of second half
				else if ((curRow >= totalRows/2 - sideLen/2 + 1) && (curRow < totalRows/2 + sideLen/2 - 1)) {
					// don't inc xpos, ypos or nextRowLen, since we want a few rows that are the same
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (DragonGame.tileWidth / 2);
						ypos = ypos - (DragonGame.tileHeight / 4);
					}
				}
			}
		}
	}


	[PunRPC]
	void GenerateMap (int seed) {

		if (seed != 0) {
			Debug.Log("Running GenerateMap with seed from network " + seed.ToString());
			Random.seed = seed;
		}
		else {
			Debug.Log("Running GenerateMap with random seed " + Random.seed.ToString());
		}
		// Make sure to preserve the order of random generation
		// usually best to use half of number of rows as side len

		//MakeHexMap(18,9);
		MakeDiamondMap();
		IterateTileMapAndCreate();
	}
	/*
	void PickRandomTile () {
		int tile = Random.Range(4,9);
		switch (tile) {
		case 1:
			tilePrefab = tilePrefab1;
			break;
		case 2:
			tilePrefab = tilePrefab2;
			break;
		case 3:
			tilePrefab = tilePrefab3;
			break;
		case 4:
			tilePrefab = tilePrefab4;
			break;
		case 5:
			tilePrefab = tilePrefab5;
			break;
		case 6:
			tilePrefab = tilePrefab6;
			break;
		case 7:
			tilePrefab = tilePrefab7;
			break;
		case 8:
			tilePrefab = tilePrefab8;
			break;
		default:
			tilePrefab = tilePrefab1;
			break;
		}
	}*/

	void AddTile (float x, float y, TileLayer tileLayer, TileType tileType, int tileId) {
		position.x = x;
		position.y = y;

		string newName = "Tile JustInCase" + sortingCount.ToString();

		switch (tileLayer) {
		case TileLayer.FLOOR:
			//GameObject.Find ("Tile 28/Floor");
			newName = "Tile Floor " + tileId.ToString();
			break;
		case TileLayer.WALL:
			newName = "Tile Wall " + tileId.ToString();
			break;
		case TileLayer.FOG:
			newName = "Tile Fog " + tileId.ToString();
			break;
		}

		switch (tileType) {
		case TileType.FLOOR:
			tilePrefab = floorTilePrefab;
			break;
		case TileType.FLOOR_ROOM:
			tilePrefab = floorTilePrefab;
			break;
		case TileType.WALL_ROOM:
			tilePrefab = floorTilePrefab;
			break;
		}

		//newName = "Tile " + sortingCount.ToString();
		Basics.assert(GameObject.Find(newName) == null);
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.name = newName;
		newTile.transform.parent = gameObject.transform;
		newTile.GetComponent<SpriteRenderer>().sortingOrder = sortingCount;
		if (tileLayer == TileLayer.FLOOR) {

		}

		sortingCount++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
