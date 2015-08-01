using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {
	/// Digging power per round
	public IList<Incantation> actionsTaken = new List<Incantation>();
	public int maxShovels = 1000; // TEMP: DIGGING OVERWHELMING
	public int shovels = 1000; // TEMP: DIGGING OVERWHELMING

	public bool turnTaken = false;
}

public class DragonGame : MonoBehaviour {
	public static DragonGame instance;
	public Player localPlayer = new Player();
	public Vector2 mapBounds;
	public const float tileWidth = 256.0f;
	public const float tileHeight = 256.0f;
	public static float maxTileNeighborDistance; // should be a bit more than the distance from the center of a tile to the nearest point on its diagonally adjacent neighbors

	void Start () {
		Basics.assert(!instance);
		instance = this;

		maxTileNeighborDistance = 1.1f * Mathf.Sqrt(2 * Mathf.Pow(tileWidth, 2));
	}

	public void EndTurn() {
		if (!Network.instance.started) {
			Basics.Log("End turn? I've hardly started!");
			return;
		}
		if (localPlayer.turnTaken) {
			Basics.Log ("End turn? It's already ogre.");
			return; // can this happen? Can we process more than one click per Unity update, or could we get another update
					// call before the previous one returns?
		}
		Basics.Log ("It's all ogre.");
		localPlayer.turnTaken = true;
		Network.instance.SendActions();
	}
	
	/// @arg repeating If true, we are repeating an already-confirmed action (from the other player's client)
	/// @note All local player actions should be executed via this method (hence TryDig() etc. are private)
	public void ProcessAction(Incantation action, bool repeating = false) {
		if (localPlayer.turnTaken && !repeating) {
			Basics.Log("Unable to comply; turn not in progress");
			return; // no actions until next turn
		}

		bool result = false;

		result = TryDig (action.tileName); // only action we can carry out at the moment

		Basics.assert(result || !repeating); // we shouldn't be replaying unsuccessful actions
		if (result && !repeating)
			// Successfully took an action, queue for replay
			localPlayer.actionsTaken.Add(action);
	}

	/// If tile is dug, recursively reveals it and any dug neighbors
	private void reveal(Tile tile, HashSet<Tile> visited = null) {
		if (visited == null)
			visited = new HashSet<Tile>();
		// TODO: implement
		//if (!visited.Contains.t
	}

	public void StartTurn() {
		Basics.assert(localPlayer.turnTaken);

		localPlayer.shovels = localPlayer.maxShovels;
		localPlayer.actionsTaken.Clear();
		localPlayer.turnTaken = false;
	}
	
	
	/// @return true on success
	private bool TryDig(Tile tile) {
		if (!tile.diggable || localPlayer.shovels < 1)
			return false;

		if (tile.dug) {
			// I guess this is game over? Might wanna assert it was the other player that dug this tile.
			Basics.Log("SOMEONE JUST WON THE GAME!!!1!");
			return false;
		}

		localPlayer.shovels -= 1;

		// Loot happens here

		tile.Dig ();

		return true;
	}

	private bool TryDig(string tileName) {
		return TryDig (GameObject.Find(tileName).GetComponent<Tile>());
	}	 	
}
