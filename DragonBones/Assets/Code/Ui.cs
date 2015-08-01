using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ui : MonoBehaviour {
	
	public string debug;
	
	public float dragSpeedCoefficient;
	public float keyboardScrollSpeed;

	// {touch->durationPressed}
	//Dictionary<Touch, float> touchDurations = new Dictionary<Touch, float>();
	// fingerId of touch objects that were dragged
	HashSet<int> wasDragged = new HashSet<int>();

	public static Ui instance;

	// Use this for initialization
	void Start () {
		Basics.assert(!instance);
		instance = this;

		debug = "_";
		dragSpeedCoefficient = 1f;
		keyboardScrollSpeed = 1000f;
	}

	public static Tile GetTile(GameObject obj) {
		// Is the object a floor?
		if (obj.tag == "floor") {
			Basics.Log ("Tile floor clicked");
			return obj.transform.parent.gameObject.GetComponent<Tile>();
		}

		// Nope. Might be a Tile
		return obj.GetComponent<Tile>();
	}

	public void OnGUI()
	{
		GUILayout.Label(debug);
	}

	/// @return an index in the given array, -1 if nothing clickable
	GameObject ThingClicked(Vector2 screenPos) {
		var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(screenPos), Vector2.zero);

		if (hits.Length == 0)
			return null;
		if (hits.Length == 1) {
			return hits[0].collider.gameObject;
		}

		// Multiple things clicked, pick the one in front (based on draw order)
		var bestLayer = -1;
		var result = -1;
		for (int i = 0; i < hits.Length; ++i) {
			var h = hits[i];
			var layer = h.collider.GetComponent<SpriteRenderer>().sortingOrder;
			if (layer > bestLayer) {
				bestLayer = layer;
				result = i;
			}
		}
					
		return hits[result].collider.gameObject;
	}

	public void TryMoveCamera(Vector3 delta) {
		var viewportSize = new Vector2(0, Camera.main.orthographicSize); // misnomer. Actually HALF viewport size
		viewportSize.y = viewportSize.y * Camera.main.aspect;

		var position = Camera.main.transform.position + delta;
		// Keep our view within the map bounds
		position.x = Mathf.Clamp(position.x, viewportSize.x, 
		                                               DragonGame.instance.mapBounds.x - viewportSize.x);
		position.y = Mathf.Clamp(position.y, viewportSize.y, 
		                                               DragonGame.instance.mapBounds.y - viewportSize.y);
		Camera.main.transform.position = position;
	}

	// Update is called once per frame
	void Update () {
		if (!Network.instance.started)
			// Ignore inputs while waiting for game start
			return;

		keyboardScrollSpeed = 2000f;
		// Note that we can drag the camera around when it's not our turn, we just can't click stuff

		// Keyboard inputs, for debug
		var scroll = Vector3.zero;
		if (Input.GetKeyUp("space")) {
			Basics.Log("Space pressed");
			DragonGame.instance.EndTurn();
		}
		if (Input.GetKey(KeyCode.UpArrow)) {
			scroll = new Vector3(0f, 1f, 0f);
		}
		if (Input.GetKey(KeyCode.RightArrow)) {
			scroll = new Vector3(1f, 0f, 0f);
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			scroll = new Vector3(0f, -1f, 0f);
		}
		if (Input.GetKey(KeyCode.LeftArrow)) {
			scroll = new Vector3(-1f, 0f, 0f);
		}
		if (scroll != Vector3.zero)
			Camera.main.transform.position += scroll * keyboardScrollSpeed * Time.deltaTime;
		

		if (Input.GetMouseButtonDown (0)) {
			// BEGIN COPYPASTE=======================================================================================
			var mousePos = Input.mousePosition;
			mousePos.z = -Camera.main.transform.position.z; // Unity can't quite get its head around this whole 2D thing
			var clickPos = Camera.main.ScreenToWorldPoint(mousePos);

			// Looks like a tap
			Basics.Log ("Casting at " + clickPos.ToString());
			var clicked = ThingClicked (clickPos);
			
			if (clicked != null) {
				// We've clicked something
				if (clicked.tag == "endTurn")
					// It's the end turn button
					DragonGame.instance.EndTurn();
				
				Tile tile = GetTile(clicked);
				
				if (tile != null) {
					// Tile clicked. At the moment, all we can do is dig
					DragonGame.instance.ProcessAction(new Incantation(tile));
				}
			}
			// END COPYPASTE=======================================================================================
		}
		
		// Touch inputs
		if (Input.touches.Length > 0) {
			var summary = "TOUCH INFO:\n";
			// Iterate over data
			foreach (var touch in Input.touches) {
				if (touch.phase == TouchPhase.Moved) {
					// Looks like we're dragging. Scroll.
					wasDragged.Add(touch.fingerId);

					var delta = touch.deltaPosition * dragSpeedCoefficient;
					TryMoveCamera(-new Vector3(delta.x, delta.y, 0));
				}
				else if (touch.phase == TouchPhase.Ended) {
					if (wasDragged.Contains(touch.fingerId)) {
						// We've finished dragging, take no special action
						wasDragged.Remove(touch.fingerId);
					} else {
						// Looks like a tap
						var clicked = ThingClicked (touch.position);

						if (clicked != null) {
							// We've clicked something
							if (clicked.tag == "endTurn")
								// It's the end turn button
								DragonGame.instance.EndTurn();

							Tile tile = GetTile (clicked);
						
							if (tile != null) {
								// Tile clicked. At the moment, all we can do is dig
								DragonGame.instance.ProcessAction(new Incantation(tile));
							}
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
