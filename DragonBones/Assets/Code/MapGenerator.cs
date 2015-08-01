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

	Tile tileScriptRef;

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
		Random.seed = seed;

		// Make sure to preserve the order of random generation

		int rowLen = 30;
		int totalRows = 30;

		int curRow, curTile;

		float x = 0;
		float y = 0;

		for (curRow = 0; curRow < totalRows; curRow++) {
			xpos = 0 - ((curRow) * tileWidth / 2);
			ypos = 0 - ((curRow) * tileWidth / 4);

			for (curTile = 0; curTile < rowLen; curTile++) {
				// choose the right type of tile from the tilemap matric (by int in the list I guess..)
				x = xpos + (curTile * tileWidth / 2);
				y = ypos - (curTile * tileHeight / 4);
				AddTile(x, y);
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
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		newTile.GetComponent<SpriteRenderer>().sortingOrder = sortingCount;
		sortingCount++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
