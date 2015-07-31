using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	[PunRPC]
	void GenerateMap (int seed) {
		seed = 1358043;
		Random.seed = seed;



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
