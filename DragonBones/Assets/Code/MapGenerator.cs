using UnityEngine;
using System.Collections;

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

	float xpos;
	float ypos;
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
		PickRandomTile();
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
	}

	void SetSortingAndIncOrder () {
		newTile.GetComponent<SpriteRenderer>().sortingOrder = sortingCount;
		sortingCount++;
	}

	void PickRandomTile () {
		int tile = Random.Range (1,9);
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
		xpos = xpos + tileSizeX/2;
		ypos = ypos - tileSizeY/4;
		position.x = xpos;
		position.y = ypos;
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;
		SetSortingAndIncOrder();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
