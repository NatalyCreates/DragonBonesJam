using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ui : MonoBehaviour {
	
	public string debug = "_";

	public float dragSpeedCoefficient = 1.0f;

	// {touch->durationPressed}
	//Dictionary<Touch, float> touchDurations = new Dictionary<Touch, float>();
	// fingerId of touch objects that were dragged
	HashSet<int> wasDragged = new HashSet<int>();

	public static Ui instance;

	// Use this for initialization
	void Start () {
		Basics.assert(!instance);
		instance = this;
	}

	public void OnGUI()
	{
		GUILayout.Label(debug);
	}

	// Update is called once per frame
	void Update () {
		if (Input.touches.Length > 0) {
			var summary = "TOUCH INFO:\n";
			// Iterate over data
			foreach (var touch in Input.touches) {
				if (touch.phase == TouchPhase.Moved) {
					// Looks like we're dragging. Scroll.
					wasDragged.Add(touch.fingerId);

					var delta = touch.deltaPosition * dragSpeedCoefficient;
					Camera.main.transform.position -= new Vector3(delta.x, delta.y, 0);
				}
				else if (touch.phase == TouchPhase.Ended) {
					if (wasDragged.Contains(touch.fingerId)) {
						// We've finished dragging, take no special action
						wasDragged.Remove(touch.fingerId);
					} else {
						// Looks like a tap
						RaycastHit2D hit;
						var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
						if (hits.Length == 1)
							hit = hits[0];
						else if (hits.Length > 1) {
							// Multiple tiles clicked, pick the one in front (based on draw order)
							var bestLayer = -1;
							foreach (var h in hits) {
								var layer = h.collider.GetComponent<SpriteRenderer>().sortingOrder;
								if (layer > bestLayer) {
									bestLayer = layer;
									hit = h;
								}
							}
						}												

						var tile = hit.collider.GetComponent<Tile>();

						if (tile) {
							// Tile clicked
							Basics.log ("There was a TILE here. It's gone now.");
							Destroy(tile.gameObject);
						}
					}
				}
				summary += string.Format("{0}: {1} ({2})\n", touch.fingerId, touch.phase, touch.position);
			}
		
			Basics.assert(wasDragged.Count <= Input.touches.Length);
			//Ui.instance.debug = summary;
		}
	}

}
