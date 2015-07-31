using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class TileMap {

}*/

//struct TileMap


public class MapGenerator : MonoBehaviour {

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



	// Use this for initialization
	void Start () {
		xpos = 0;
		ypos = 0;
		position = new Vector2(xpos, ypos);
		GenerateMap(1);
	}

	[PunRPC]
	void GenerateMap (int seed) {

		Debug.Log("Running GenerateMap with seed " + seed);

		//seed = 1358043;
		//Random.seed = seed;
		//float width = (float)Random.Range(2, 5);

		//Debug.Log("seed = " + Random.seed);

		// Make sure to preserve the order of random generation
		//float tileY, tileX, mapSize;

		//tileMap.Add(1);

		//mapSize = 25;
		//tileX = 0;
		int firstRowLen = 2;
		//int totalTilesPerRow = firstRowTileNum;
		int nextRowLen;
		int totalRows = 6;
		int curRow, curTile;
		float x = 0;
		float y = 0;
		for (curRow = 0; curRow < totalRows; curRow++) {
			if (curRow < totalRows / 2) {
				nextRowLen = firstRowLen * (curRow + 1);
				xpos = 0 - ((curRow) * tileWidth / 2) - (curRow * 128);
				ypos = 0 - ((curRow) * tileWidth / 4) + (curRow * 64);
			}
			else {
				nextRowLen = firstRowLen * (totalRows - curRow);
				xpos = 0 - ((curRow) * tileWidth / 2) + (curRow * 128);
				ypos = 0 - ((curRow) * tileWidth / 4) - (curRow * 64);
			}

			Debug.Log("next row has " + nextRowLen.ToString() + " tiles");

			//xpos = 0 - ((curRow + 1) * tileWidth / 2);
			//Debug.Log("xpos, ypos = " + xpos.ToString() + ", " + ypos.ToString());
			//xpos = 0 - ((curRow - 1) * tileWidth / 2) - (tileWidth / 2)*curRow;
			//ypos = 0 - ((curRow - 1) * tileWidth / 4) + (tileWidth / 4)*curRow;
			//Debug.Log("xpos, ypos = " + xpos.ToString() + ", " + ypos.ToString());
			for (curTile = 0; curTile < nextRowLen; curTile++) {
				// choose the right type of tile from the tilemap matric (by int in the list I guess..)
				x = xpos + (curTile * tileWidth / 2);
				y = ypos - (curTile * tileHeight / 4);
				Debug.Log(x.ToString() + ", " + y.ToString());
				AddTile(x, y);
			}
		}

		/*
		// Row 1
		AddTileRight();
		AddTileRight();
		AddTileRight();
		// Row 2
		RepositionPointerHex();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		// Row 3
		RepositionPointerHex();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		// Row 4
		RepositionPointerHex();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		// Row 5
		RepositionPointerHex();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		// Row 6
		RepositionPointerHex();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		*/


	}

	void RepositionPointerHex () {
		rows++;
		xpos = 0 - (rows + 1) * tileSizeX/2;
		ypos = 0 - (rows + 1) * tileSizeY/4;
	}

	void RepositionPointer () {
		rows++;
		xpos = 0 - rows * tileSizeX/2;
		ypos = 0 - rows * tileSizeY/4;
	}

	void RepositionPointerAfterBelow () {
		//rows++;
		xpos = 0 - rows * tileSizeX/2;
		ypos = 0 - rows * tileSizeY/4;
	}

	void RepositionPointerBelow () {
		rows++;
		xpos = 0 - (rows - 1) * tileSizeX/2;
		ypos = 0 - (rows + 1) * tileSizeY/4;
	}

	/*
	void CreateFirstTile () {
		PickRandomTile();
		position.x = xpos;
		position.y = ypos;
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
	}*/

	void SetSortingAndIncOrder () {
		newTile.GetComponent<SpriteRenderer>().sortingOrder = sortingCount;
		sortingCount++;
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

	void AddTileRight () {
		PickRandomTile();
		position.x = xpos;
		position.y = ypos;
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
		xpos = xpos + tileSizeX/2;
		ypos = ypos - tileSizeY/4;
	}

	void AddTile (float x, float y) {
		PickRandomTile();
		position.x = x;
		position.y = y;
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
