using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	GameObject tilePrefab;
	public GameObject tilePrefab0;
	public GameObject tilePrefab1;
	public GameObject tilePrefab2;

	//float tileSizeX = 256.0f;
	//float tileSizeY = 256.0f;

	int tileSizeX = 256;
	int tileSizeY = 256;

	//float xpos;
	int xpos;
	//float ypos;
	int ypos;
	Vector2 position;

	GameObject newTile;

	int sortingCount = 0;

	int rows = 0;

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



		CreateFirstTile();

		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();

		RepositionPointer();

		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();

		RepositionPointer();

		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();

		RepositionPointer();
		
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();
		AddTileRight();

	}

	void RepositionPointer () {
		rows++;
		xpos = 0;
		ypos = 0 - rows * tileSizeY/2;
	}

	void CreateFirstTile () {
		int tile = Random.Range (2,4);
		Debug.Log ("rand tile is " + tile);
		switch (tile) {
		case 1:
			tilePrefab = tilePrefab0;
			break;
		case 2:
			tilePrefab = tilePrefab1;
			break;
		case 3:
			tilePrefab = tilePrefab2;
			break;
		}
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
	}

	void SetSortingAndIncOrder () {
		newTile.GetComponent<SpriteRenderer>().sortingOrder = sortingCount;
		sortingCount++;
	}
	/*
	void AddTileTopLeft () {
		xpos = xpos + (-1) * tileSizeX/2;
		ypos = ypos + tileSizeY/4;
		position.x = xpos;
		position.y = ypos;
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
	}*/
	/*
	void AddTileLeft () {
		xpos = xpos - tileSizeX/2;
		ypos = ypos - tileSizeY/4;
		position.x = xpos;
		position.y = ypos;
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
	}*/

	void AddTileRight () {
		int tile = Random.Range (2,4);
		switch (tile) {
		case 1:
			tilePrefab = tilePrefab0;
			break;
		case 2:
			tilePrefab = tilePrefab1;
			break;
		case 3:
			tilePrefab = tilePrefab2;
			break;
		}
		xpos = xpos + tileSizeX/2;
		ypos = ypos - tileSizeY/4;
		position.x = (float) xpos;
		position.y = (float) ypos;
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
