using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {	
	public bool dug; // TODO: get rid of this, just destroy the tile and keep a set of destroyed tile IDs
	public bool hidden = true;
	public bool solid = true;

	public void Dig() {
		// TODO: cool effects
		Basics.assert(!dug);
		dug = false;
		gameObject.SetActive(false);
		// TODO: ensure child objects (specifically the floor and its collider) remain active
	}

	public bool diggable {
		get {
			return !(hidden || solid || dug);
		}
	}

	public HashSet<Tile> neighbors() {
		// Assume we're a standard-sized (square) tile. Further assume our anchor is in the top left (sprite extends
		// in the +x and +y directions)
		Basics.assert(DragonGame.tileWidth == DragonGame.tileHeight);

		var center = new Vector2(transform.position.x, transform.position.y)
			+ new Vector2(DragonGame.tileWidth / 2, DragonGame.tileHeight / 2);
		var result = new HashSet<Tile>();
		var candidates = Physics2D.CircleCastAll(center, DragonGame.maxTileNeighborDistance, Vector2.zero);


		foreach (var h in candidates) {
			if (h.collider.gameObject == gameObject)
				continue;

			var tile = Ui.GetTile(h.collider.gameObject);
			if (tile != null)
				result.Add(tile);
		}

		return result;
	}

	public void reveal() {
		Basics.assert(!hidden);

		hidden = false;

		// Remove fog sprite
		Destroy(transform.Find("Fog").gameObject);
	}	
}
