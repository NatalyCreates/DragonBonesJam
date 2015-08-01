using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ui : MonoBehaviour {
	
	public string debug = "_";
	
	public float dragSpeedCoefficient = 1.0f;
	public float keyboardScrollSpeed = 10f;

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

		// Note that we can drag the camera around when it's not our turn, we just can't click stuff

		// Keyboard inputs, for debug
		var scroll = Vector3.zero;
		if (Input.GetKeyUp("space")) {
			DragonGame.instance.EndTurn();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			scroll = new Vector3(0f, 1f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			scroll = new Vector3(1f, 0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			scroll = new Vector3(0f, -1f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			scroll = new Vector3(-1f, 0f, 0f);
		}
		if (scroll != Vector3.zero)
			Camera.main.transform.position += scroll * keyboardScrollSpeed;

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
						bool hitSomething = false; // RaycastHit2D is a struct; can't be null
						RaycastHit2D hit;
						var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
						if (hits.Length == 1) {
							hit = hits[0];
							hitSomething = true;
						}
						else if (hits.Length > 1) {
							// Multiple tiles clicked, pick the one in front (based on draw order)
							hitSomething = true;
							var bestLayer = -1;
							foreach (var h in hits) {
								var layer = h.collider.GetComponent<SpriteRenderer>().sortingOrder;
								if (layer > bestLayer) {
									bestLayer = layer;
									hit = h;
								}
							}
						}			

						if (hitSomething) {
							// We've clicked something
							if (hit.collider.gameObject.tag == "endTurn")
								// It's the end turn button
								DragonGame.instance.EndTurn();

							var tile = hit.collider.GetComponent<Tile>();

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
