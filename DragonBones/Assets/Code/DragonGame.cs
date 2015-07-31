using UnityEngine;
using System.Collections;

public class DragonGame : MonoBehaviour {
	public static DragonGame instance;
	public Vector2 mapBounds;

	void Start () {
		Basics.assert(!instance);
		instance = this;
	}
    
}
