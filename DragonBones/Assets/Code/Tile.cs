using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {	
	public bool solid = false;

	public void Dig() {
		// TODO: cool effects
		Basics.assert(!dug);

		if (hidden)
			Reveal();

		GetComponent<Collider2D>().enabled = false;
		GetComponent<Renderer>().enabled = false;

		Basics.assert(dug);
	}

	public bool diggable {
		get {
			if (!(hidden || solid || dug))
				Debug.Log(string.Format("Not diggable: {0}, {1}, {2}", hidden, solid, dug));
			return !(hidden || solid || dug);
		}
	}

	public bool dug {
		get {
			return !GetComponent<Collider2D>().enabled;
		}
	} 

	public bool hidden {
		get {
			return transform.Find("Fog") != null;
		}
	}

	/// TODO: this won't work for isometric maps, gotta reimplement
	public HashSet<Tile> neighbors {
		get {
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
	}

	public void Reveal() {
		Basics.assert(hidden);

		// Remove fog sprite
		//Destroy(transform.Find("Fog").gameObject);
		// ^ I guess Destroy calls are non-blocking? Because child is still around when we return.
		// Do it like so, instead:
		var fog = transform.Find("Fog");
		fog.parent = null;
		Destroy(fog.gameObject);

		Basics.assert(!hidden);
	}	
}
