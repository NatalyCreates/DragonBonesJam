using UnityEngine;
using System.Collections;

/// A player action. We execute these locally, then send them to the other player, who does the same.
public class Incantation : MonoBehaviour {
	// This class is presently kinda trivial, as there is only one thing we can do

	/// This is how we identify the tile selected. Tile names should be unique
	public string tileName;

	public Incantation() {}

	public Incantation(Tile tile) {
		tileName = tile.name;
	}

	public Incantation(string descriptor) {
		tileName = descriptor;
	}

	/// TEMP: can do this more cleanly with Photon.SendNext() and ReceiveNext(), but for now we use RPCs and manual
	/// serialization, because there's less room for error
	public string SortaSerialize() {
		return tileName;
	}
}
