using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour {

	enum TileType {
		FLOOR_DIG,
		FLOOR_DIG_POWER,
		FLOOR_ROOM,
		WALL_DIG,
		WALL_DIG_POWER,
		WALL_ROOM,
		WALL_DRAGON,
		WALL_ROCK,
		WALL_UNKOWN,
	};
	struct TileLayered {
		private bool set;
		public int xInMap;
		public int yInMap;
		public int typeFloor;
		public int typeWall;
		public GameObject prefabFloor;
		public GameObject prefabWall;
		public GameObject prefabUnknown;
	}

	GameObject tilePrefab;
	public GameObject tilePrefab1;
	public GameObject tilePrefab2;
	public GameObject tilePrefab3;
	public GameObject tilePrefab4;
	public GameObject tilePrefab5;
	public GameObject tilePrefab6;
	public GameObject tilePrefab7;
	public GameObject tilePrefab8;

	//float tileSizeX = 256.0f;
	//float tileSizeY = 256.0f;

	float tileWidth = 256.0f;
	float tileHeight = 256.0f;

	float xpos;
	float ypos;
	Vector2 position;
	GameObject newTile;

	int sortingCount = 0;
	int rows = 0;

	List<int> tileMap =  new List<int>();

	//Tile tileScriptRef;

	// Use this for initialization
	void Start () {
		xpos = 0;
		ypos = 0;
		position = new Vector2(xpos, ypos);
		GenerateMap(0);
	}

	void MakeDiamondMap(int rowLen = 6, int totalRows = 6) {
		int curRow, curTile;

		for (curRow = 0; curRow < totalRows; curRow++) {
			for (curTile = 0; curTile < rowLen; curTile++) {
				// choose the right type of tile from the tilemap matric (by int in the list I guess..)
				xpos = xpos + (tileWidth / 2);
				ypos = ypos - (tileHeight / 4);
				AddTile(xpos, ypos);
			}
			// move pointer back to start
			xpos = xpos - ((rowLen + 1) * (tileWidth / 2));
			ypos = ypos + ((rowLen - 1) * (tileHeight / 4));
		}
	}

	void MakeHexMap(int totalRows = 8, int sideLen = 5) {
		
		int rowIncRate = 2; // can only be even (for a symmetrical map)!! (usually 2 is best)
		
		int nextRowLen = sideLen;
		int curRow, curTile;
		
		for (curRow = 0; curRow < totalRows; curRow++) {
			for (curTile = 0; curTile < nextRowLen; curTile++) {
				// choose the right type of tile from the tilemap matric (by int in the list I guess..)
				xpos = xpos + (tileWidth / 2);
				ypos = ypos - (tileHeight / 4);
				AddTile(xpos, ypos);
			}
			// move pointer back to start
			xpos = xpos - ((nextRowLen + 1) * (tileWidth / 2));
			ypos = ypos + ((nextRowLen - 1) * (tileHeight / 4));
			
			// even number of rows
			if (totalRows % 2 == 0) {
				// first half
				if (curRow < totalRows/2 - 1) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (tileWidth / 2);
						ypos = ypos + (tileHeight / 4);
					}
				}
				// first row of second half
				else if (curRow == totalRows/2 - 1) {
					//nextRowLen = nextRowLen;
					//xpos = xpos;
					//ypos = ypos;
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (tileWidth / 2);
						ypos = ypos - (tileHeight / 4);
					}
				}
			}
			// odd number of rows, total/2 is rounded down
			else {
				// first half
				if (curRow < totalRows/2) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (tileWidth / 2);
						ypos = ypos + (tileHeight / 4);
					}
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (tileWidth / 2);
						ypos = ypos - (tileHeight / 4);
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
				// choose the right type of tile from the tilemap matric (by int in the list I guess..)
				xpos = xpos + (tileWidth / 2);
				ypos = ypos - (tileHeight / 4);
				AddTile(xpos, ypos);
			}
			// move pointer back to start
			xpos = xpos - ((nextRowLen + 1) * (tileWidth / 2));
			ypos = ypos + ((nextRowLen - 1) * (tileHeight / 4));
			
			// even number of rows
			if (totalRows % 2 == 0) {
				// first half
				if (curRow < totalRows/2 - sideLen/2) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (tileWidth / 2);
						ypos = ypos + (tileHeight / 4);
					}
				}
				// first row of second half
				else if ((curRow >= totalRows/2 - sideLen/2) && (curRow < totalRows/2 + sideLen/2 - 1)) {
					//nextRowLen = nextRowLen;
					//xpos = xpos;
					//ypos = ypos;
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (tileWidth / 2);
						ypos = ypos - (tileHeight / 4);
					}
				}
			}
			// odd number of rows, total/2 is rounded down
			else {
				// first half
				if (curRow < totalRows/2 - sideLen/2 + 1) {
					nextRowLen = nextRowLen + rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos - (tileWidth / 2);
						ypos = ypos + (tileHeight / 4);
					}
				}
				// first row of second half
				else if ((curRow >= totalRows/2 - sideLen/2 + 1) && (curRow < totalRows/2 + sideLen/2 - 1)) {
					//nextRowLen = nextRowLen;
					//xpos = xpos;
					//ypos = ypos;
				}
				// second half
				else {
					nextRowLen = nextRowLen - rowIncRate;
					for (int i = 0; i < rowIncRate/2; i++) {
						xpos = xpos + (tileWidth / 2);
						ypos = ypos - (tileHeight / 4);
					}
				}
			}
		}
	}


	[PunRPC]
	void GenerateMap (int seed) {

		if (seed != 0) {
			Debug.Log("Running GenerateMap with seed from network " + seed);
			Random.seed = seed;
		}
		else {
			Debug.Log("Running GenerateMap with random seed " + Random.seed);
		}

		// Make sure to preserve the order of random generation

		// usually best to use half of number of rows as side len
		//MakeHexMap(18,9);
		MakeHexMap(5,2);

	}

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
	}

	void AddTile (float x, float y) {
		PickRandomTile();
		position.x = x;
		position.y = y;
		string newName = "Tile " + sortingCount.ToString();
		Basics.assert(GameObject.Find(newName) == null);
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.name = newName;
		newTile.transform.parent = gameObject.transform;
		newTile.GetComponent<SpriteRenderer>().sortingOrder = sortingCount;
		sortingCount++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
