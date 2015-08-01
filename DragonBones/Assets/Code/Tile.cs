using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	
	public bool dug;
	public Transform fog;
	public bool hidden = true;
	public bool solid = true;

	public void Dig() {
		// TODO: cool effects
		Basics.assert(!dug);
		dug = false;
		gameObject.SetActive(false);
	}

	public bool diggable() {
		return !(hidden || solid || dug);
	}

	public IList<Tile> neighbors() {
		// Assume we're a standard-sized (square) tile. Further assume our anchor is in the top left (sprite extends
		// in the +x and +y directions)
		Basics.assert(DragonGame.tileWidth == DragonGame.tileHeight);

		var center = transform.position + Vector2(DragonGame.tileWidth / 2, DragonGame.tileHeight / 2);
		var result = new List<Tile>();
		var candidates = Physics2D.CircleCastAll(center, DragonGame.maxTileNeighborDistance, Vector2.zero);


		foreach (var h in candidates) {
			if (h.collider.gameObject == gameObject)
				continue;

			var tile = h.collider.gameObject.GetComponent<Tile>();
			if (tile != null)
				result.Add(tile);
		}

		return result;
	}

	public void reveal() {
		Basics.assert(!hidden);

		hidden = false;
		Destroy(fog);
	}	
}
