using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ui : MonoBehaviour {
	
	public string debug = "_";

	public float dragSpeedCoefficient = 1.0f;

	// {touch->durationPressed}
	//Dictionary<Touch, float> touchDurations = new Dictionary<Touch, float>();
	HashSet<Touch> wasDragged = new HashSet<Touch>();

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
					wasDragged.Add(touch);

					var delta = touch.deltaPosition * dragSpeedCoefficient;
					Camera.main.transform.position += new Vector3(delta.x, delta.y, 0);
				}
				else if (touch.phase == TouchPhase.Ended) {
					if (wasDragged.Contains(touch)) {
						// We've finished dragging, take no special action
						wasDragged.Remove(touch);
						debug = "dragged";
					} else {
						// Looks like a tap
						debug = "tapped";

						var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
						if (hits.Length > 1)
							// My god... overlapping colliders!
							Basics.log("Collider overlap");
						else if (hits.Length > 0) {
							var tile = hits[0].collider.GetComponent<Tile>();

							if (tile) {
								// Tile clicked
								Basics.log ("Tile tapped");
							}
						}
					}
				}
				summary += string.Format("{0}: {1} ({2})\n", touch.fingerId.ToString(), touch.phase, touch.position);
			}
		
			summary += "\nDrag count" + wasDragged.Count.ToString();
			//Basics.assert(wasDragged.Count <= Input.touches.Length);
			Ui.instance.debug = summary;
		}
	}

}
