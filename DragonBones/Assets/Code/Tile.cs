using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public bool diggable = true;
	public bool dug = false;

	public void Dig() {
		// TODO: cool effects
		Basics.assert(!dug);
		dug = false;
		gameObject.SetActive(false);
	}
}
