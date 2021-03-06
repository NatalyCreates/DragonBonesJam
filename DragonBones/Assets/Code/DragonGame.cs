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

		//maxTileNeighborDistance = 1.1f * Mathf.Sqrt(2 * Mathf.Pow(tileWidth / 2, 2));
		// Wait, tiles aren't aligned like that. Screw Pythagoras
		maxTileNeighborDistance = 1.1f * tileWidth / 2;;
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
									
	/// If tile is dug, reveal any contiguous dug tiles, and any of said tiles' undug neighbors. Else just reveal tile.
	private void Reveal(Tile tile) {
		// Just a breadth-first traversal
		var toVisit = new List<Tile>();
		var discovered = new HashSet<Tile>();
		discovered.Add(tile);
		toVisit.Add(tile);

		while (toVisit.Count > 0) {
			var current = toVisit[0];
			toVisit.RemoveAt(0);

			if (current.hidden)
				current.Reveal();

			// If tile is dug, reveal neighbors, else just reveal itself
			if (current.dug) {
				foreach (var n in current.neighbors) {
					if (!discovered.Contains(n)) {
						discovered.Add(n);
						toVisit.Add(n);
					}
				}
			}
		}
	}

	public void StartTurn() {
		Basics.assert(localPlayer.turnTaken);

		localPlayer.shovels = localPlayer.maxShovels;
		localPlayer.actionsTaken.Clear();
		localPlayer.turnTaken = false;
	}
	
	
	/// @return true on success
	private bool TryDig(Tile tile) {
		if (tile.hidden) // TEMP DEBUG: reveal tile
			tile.Reveal();
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

		// Transitively reveal tile's neighbors
		Reveal(tile);

		return true;
	}

	private bool TryDig(string tileName) {
		return TryDig (GameObject.Find(tileName).GetComponent<Tile>());
	}	 	
}
