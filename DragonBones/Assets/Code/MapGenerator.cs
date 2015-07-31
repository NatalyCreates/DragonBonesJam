using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public GameObject tilePrefab;

	float tileSizeX = 256.0f;
	float tileSizeY = 256.0f;

	// Use this for initialization
	void Start () {

		//GenerateMap(1);
	}

	[PunRPC]
	void GenerateMap (int seed) {

		Debug.Log("Running GenerateMap with seed " + seed);

		//seed = 1358043;
		//Random.seed = seed;
		//float width = (float)Random.Range(2, 5);

		//Debug.Log("seed = " + Random.seed);

		// Make sure to preserve the order of random generation

		GameObject newTile;

		float xpos = 0;
		float ypos = 0;
		Vector2 position = new Vector2(xpos, ypos);

		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;

		xpos = xpos + (-1) * tileSizeX/2;
		ypos = ypos + tileSizeY/4;
		position.x = xpos;
		position.y = ypos;

		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;

		xpos = xpos + (-1) * tileSizeX/2;
		ypos = ypos + tileSizeY/4;
		position.x = xpos;
		position.y = ypos;
		
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;

		xpos = xpos + (-1) * tileSizeX/2;
		ypos = ypos + tileSizeY/4;
		position.x = xpos;
		position.y = ypos;
		
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;

		xpos = xpos + (-1) * tileSizeX/2;
		ypos = ypos + tileSizeY/4;
		position.x = xpos;
		position.y = ypos;
		
		newTile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		newTile.transform.parent = gameObject.transform;


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
