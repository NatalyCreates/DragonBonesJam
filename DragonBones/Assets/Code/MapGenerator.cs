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

	float tileSizeX = 256.0f;
	float tileSizeY = 256.0f;

	float tileWidth = 256.0f;
	float tileHeight = 256.0f;

	float xpos;
	float ypos;
	Vector2 position;
	GameObject newTile;

	int sortingCount = 0;
	int rows = 0;

	List<int> tileMap =  new List<int>();

	Tile tileScriptRef;

	// Use this for initialization
	void Start () {
		xpos = 0;
		ypos = 0;
		position = new Vector2(xpos, ypos);
		GenerateMap(0);
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

		//int rowLen = 6;

		int totalRows = 10;

		int firstRowLen = 4;
		int nextRowLen = firstRowLen;
		int curRow, curTile;

		//float x = 0;
		//float y = 0;

		for (curRow = 0; curRow < totalRows; curRow++) {
			//xpos = 0 - ((curRow) * tileWidth / 2);
			//ypos = 0 - ((curRow) * tileWidth / 4);
			for (curTile = 0; curTile < nextRowLen; curTile++) {
				// choose the right type of tile from the tilemap matric (by int in the list I guess..)
				xpos = xpos + (tileWidth / 2);
				ypos = ypos - (tileHeight / 4);
				AddTile(xpos, ypos);
			}
			// move pointer back to start
			//xpos = (xpos - ((rowLen) * (tileWidth / 2))) - (tileWidth / 2);
			//ypos = (ypos + ((rowLen) * (tileHeight / 4))) - (tileHeight / 4);
			xpos = xpos - ((nextRowLen + 1) * (tileWidth / 2));
			ypos = ypos + ((nextRowLen - 1) * (tileHeight / 4));

			// even number of rows
			if (totalRows % 2 == 0) {
				Debug.Log("Even number of rows, totalRows/2 = " + totalRows/2);
				// first half
				if (curRow < totalRows/2 - 1) {
					nextRowLen = nextRowLen + firstRowLen;
					for (int i = 0; i < firstRowLen/2; i++) {
						xpos = xpos - (tileWidth / 2);
						ypos = ypos + (tileHeight / 4);
					}
				}
				// first row of second half
				else if (curRow == totalRows/2 - 1) {
					nextRowLen = nextRowLen;
					xpos = xpos;
					ypos = ypos;
				}
				// second half
				else {
					nextRowLen = nextRowLen - firstRowLen;
					for (int i = 0; i < firstRowLen/2; i++) {
						xpos = xpos + (tileWidth / 2);
						ypos = ypos - (tileHeight / 4);
					}
				}
			}
			// odd number of rows
			else {
				Debug.Log("Odd number of rows, totalRows/2 = " + totalRows/2); // rounded down
				// first half
				if (curRow < totalRows/2) {
					xpos = xpos - (tileWidth / 2);
					ypos = ypos + (tileHeight / 4);
					nextRowLen = nextRowLen + firstRowLen;
				}
				// second half
				else {
					xpos = xpos + (tileWidth / 2);
					ypos = ypos - (tileHeight / 4);
					nextRowLen = nextRowLen - firstRowLen;
				}
			}
		}

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
