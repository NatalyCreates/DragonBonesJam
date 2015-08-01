using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {
	/// Digging power per round
	public IList<Incantation> actionsTaken = new List<Incantation>();
	public int maxShovels = 1;
	public int shovels = 1;

	public bool turnTaken = false;
}

public class DragonGame : MonoBehaviour {
	public static DragonGame instance;
	public Player localPlayer = new Player();
	public Vector2 mapBounds;

	void Start () {
		Basics.assert(!instance);
		instance = this;
	}

	public void EndTurn() {
		if (!Network.instance.started) {
			Basics.Log("End turn? I've hardly started!");
			return;
		}
		if (localPlayer.turnTaken)
			return; // can this happen? Can we process more than one click per Unity update, or could we get another update
					// call before the previous one returns?
		localPlayer.turnTaken = true;
	}
	
	/// @arg repeating If true, we are repeating an already-confirmed action (from the other player's client)
	/// @note All local player actions should be executed via this method (hence TryDig() etc. are private)
	public void ProcessAction(Incantation action, bool repeating = false) {
		if (localPlayer.turnTaken)
			return; // no actions until next turn

		bool result = false;

		result = TryDig (action.tileName); // only action we can carry out at the moment

		Basics.assert(result || !repeating); // we shouldn't be replaying unsuccessful actions
		if (result && !repeating)
			// Successfully took an action, queue for replay
			localPlayer.actionsTaken.Add(action);
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
