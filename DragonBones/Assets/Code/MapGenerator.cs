using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour {

	public enum TileLayer {
		FLOOR,
		WALL,
		FOG,
	};
	
	public enum TileType {
		FLOOR_NONE,
		FLOOR,
		FLOOR_BONES,
		FLOOR_ROOM,
		WALL,
		WALL_BONES,
		WALL_ROOM,
		WALL_DRAGON,
		WALL_DRAGON_ENTER,
		WALL_ROCK,
		FOG,
		FOG_ROCK,
		UNSET,
	};
	
	public class TileLayered {
		public int _id;
		public bool _drawn;
		public bool _set;
		public int _row;
		public int _col;
		public TileType _typeFloor;
		public TileType _typeWall;
		public TileType _typeFog;
		public GameObject _prefabFloor;
		public GameObject _prefabWall;
		public GameObject _prefabFog;
		public float _posX;
		public float _posY;
		
		public TileLayered (int id, int row, int col, float posX, float posY) {
			_id = id;
			_row = row;
			_col = col;
			_posX = posX;
			_posY = posY;
			_typeFloor = TileType.UNSET;
			_typeWall = TileType.UNSET;
			_typeFog = TileType.UNSET;
			_prefabFloor = null;
			_prefabWall = null;
			_prefabFog = null;
			_drawn = false;
			_set = false;
		}
		
		public void SetType (TileType typeFloor, TileType typeWall, TileType typeFog) {
			
			if (_set) {
				Debug.Log ("Warning! Tile " + _id.ToString() + " was already given a type, setting again anyway.");
			}
			Debug.Log ("Setting tile " + _id.ToString());
			
			_typeFloor = typeFloor;
			_typeWall = typeWall;
			_typeFog = typeFog;
			
			switch (_typeFloor) {
			case TileType.FLOOR_NONE:
				_prefabFloor = floorNoneTilePrefab;
				break;
			case TileType.FLOOR:
				_prefabFloor = floorTilePrefab;
				break;
			case TileType.FLOOR_BONES:
				_prefabFloor = floorBonesTilePrefab;
				break;
			case TileType.FLOOR_ROOM:
				_prefabFloor = floorRoomTilePrefab;
				break;
			}
			
			switch (_typeWall) {
			case TileType.WALL:
				_prefabWall = wallTilePrefab;
				break;
			case TileType.WALL_BONES:
				_prefabWall = wallBonesTilePrefab;
				break;
			case TileType.WALL_ROOM:
				_prefabWall = wallRoomTilePrefab;
				break;
			case TileType.WALL_DRAGON:
				_prefabWall = wallDragonTilePrefab;
				break;
			case TileType.WALL_DRAGON_ENTER:
				_prefabWall = wallDragonEnterTilePrefab;
				break;
			case TileType.WALL_ROCK:
				_prefabWall = wallRockTilePrefab;
				break;
			}
			
			switch (_typeFog) {
			case TileType.FOG:
				_prefabFog = fogTilePrefab;
				break;
			case TileType.FOG_ROCK:
				_prefabFog = fogRockTilePrefab;
				break;
			}
			
			_set = true;
			Debug.Log ("_set = " + _set.ToString());
		}
		public void CreateDraw () {
			
			Debug.Log ("_set = " + _set.ToString());
			
			if (!_set) {
				Debug.Log ("Trying to create tile " + _id.ToString() + " before its type was set.");
			}
			else {
				AddTile(_posX, _posY, TileLayer.FLOOR, _prefabFloor, _id);
				AddTile(_posX, _posY, TileLayer.WALL, _prefabWall, _id);
				AddTile(_posX, _posY, TileLayer.FOG, _prefabFog, _id);
				
				GameObject floorFind = GameObject.Find ("Tile Floor " + _id.ToString());
				GameObject fogFind = GameObject.Find ("Tile Fog " + _id.ToString());
				floorFind.transform.parent = GameObject.Find("Tile Wall " + _id.ToString()).transform;
				fogFind.transform.parent = GameObject.Find("Tile Wall " + _id.ToString()).transform;
				floorFind.name = "Floor";
				fogFind.name = "Fog";
				
				_drawn = true;
			}
		}
	}

	int uniqueIdCounter = 0;

	public static GameObject floorNoneTilePrefab;
	public static GameObject floorTilePrefab;
	public static GameObject floorBonesTilePrefab;
	public static GameObject floorRoomTilePrefab;
	public static GameObject wallTilePrefab;
	public static GameObject wallBonesTilePrefab;
	public static GameObject wallRoomTilePrefab;
	public static GameObject wallDragonTilePrefab;
	public static GameObject wallDragonEnterTilePrefab;
	public static GameObject wallRockTilePrefab;
	public static GameObject fogTilePrefab;
	public static GameObject fogRockTilePrefab;

	static int sortingCount = -1;

	List<List<TileLayered>> tileMap =  new List<List<TileLayered>>();

	// Use this for initialization
	void Start () {

		// Init prefabs

		floorNoneTilePrefab = Resources.Load("FloorNoneTilePrefab") as GameObject;
		floorTilePrefab = Resources.Load("FloorTilePrefab") as GameObject;
		floorBonesTilePrefab = Resources.Load("FloorBonesTilePrefab") as GameObject;
		floorRoomTilePrefab = Resources.Load("FloorRoomTilePrefab") as GameObject;
		wallTilePrefab = Resources.Load("WallTilePrefab") as GameObject;
		wallBonesTilePrefab = Resources.Load("WallBonesTilePrefab") as GameObject;
		wallRoomTilePrefab = Resources.Load("WallRoomTilePrefab") as GameObject;
		wallDragonTilePrefab = Resources.Load("WallDragonTilePrefab") as GameObject;
		wallDragonEnterTilePrefab = Resources.Load("WallDragonEnterTilePrefab") as GameObject;
		wallRockTilePrefab = Resources.Load("WallRockTilePrefab") as GameObject;
		fogTilePrefab = Resources.Load("FogTilePrefab") as GameObject;
		fogRockTilePrefab = Resources.Load("FogRockTilePrefab") as GameObject;

		GenerateMap(0);
	}

	public int GetNextId() {
		uniqueIdCounter++;
		return uniqueIdCounter;
	}

	void IterateTileMapAndCreate() {
		for (int i = 0; i < tileMap.Count; i++) {
			for (int j = 0; j < tileMap[i].Count; j++) {
				// figure out which tile type we want!
				// options are - room, rock, dirt, bones, dragon
				tileMap[i][j].SetType(TileType.FLOOR_ROOM, TileType.WALL_ROOM, TileType.FOG);
				tileMap[i][j].CreateDraw();
			}
		}
	}

	void MakeDiamondMap(int rowLen = 6, int totalRows = 6) {
		int curRow, curTile;
		float xpos = 0, ypos = 0;

		for (curRow = 0; curRow < totalRows; curRow++) {
			List<TileLayered> tempTileList = new List<TileLayered>();
			for (curTile = 0; curTile < rowLen; curTile++) {
				xpos = xpos + (DragonGame.tileWidth / 2);
				ypos = ypos - (DragonGame.tileHeight / 4);
				tempTileList.Add(new TileLayered(GetNextId(), curRow, curTile, xpos, ypos));
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
		float xpos = 0, ypos = 0;
		
		for (curRow = 0; curRow < totalRows; curRow++) {
			List<TileLayered> tempTileList = new List<TileLayered>();
			for (curTile = 0; curTile < nextRowLen; curTile++) {
				xpos = xpos + (DragonGame.tileWidth / 2);
				ypos = ypos - (DragonGame.tileHeight / 4);
				tempTileList.Add(new TileLayered(GetNextId(), curRow, curTile, xpos, ypos));
			}
			tileMap.Add(tempTileList);
			tempTileList = new List<TileLayered>();
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
		float xpos = 0, ypos = 0;
		
		for (curRow = 0; curRow < totalRows; curRow++) {
			List<TileLayered> tempTileList = new List<TileLayered>();
			for (curTile = 0; curTile < nextRowLen; curTile++) {
				xpos = xpos + (DragonGame.tileWidth / 2);
				ypos = ypos - (DragonGame.tileHeight / 4);
				tempTileList.Add(new TileLayered(GetNextId(), curRow, curTile, xpos, ypos));
			}
			tileMap.Add(tempTileList);
			tempTileList = new List<TileLayered>();
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

		// DO NOT generate more than one map in each game - they will overlap and make a mess

		MakeHexMap(18,9);
		//MakeDiamondMap(10,10);

		IterateTileMapAndCreate();
	}

	static void AddTile (float x, float y, TileLayer tileLayer, GameObject tilePrefab, int tileId) {
		GameObject newTile;
		Vector2 position;
		position.x = x;
		position.y = y;

		string newName = "Tile JustInCase " + sortingCount.ToString();

		switch (tileLayer) {
		case TileLayer.FLOOR:
			newName = "Tile Floor " + tileId.ToString();
			break;
		case TileLayer.WALL:
			newName = "Tile Wall " + tileId.ToString();
			break;
		case TileLayer.FOG:
			newName = "Tile Fog " + tileId.ToString();
			break;
		}

		Basics.assert(GameObject.Find(newName) == null);
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.name = newName;
		//newTile.transform.parent = gameObject.transform;
		newTile.GetComponent<SpriteRenderer>().sortingOrder = sortingCount;
		sortingCount++;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
