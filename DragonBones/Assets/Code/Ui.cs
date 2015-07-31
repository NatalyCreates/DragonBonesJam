using UnityEngine;
using System.Collections;

public class Ui : MonoBehaviour {
	
	public string debug = "_";

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
			string summary = "TOUCH INFO:\n";
			// Iterate over data
			foreach (var touch in Input.touches) {
				/*if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) {
					// TODO: raycast or raycastAll
					var ray = Camera.main.ScreenPointToRay (touch.position);
				}*/
				summary += string.Format("{}: {} ({})\n", touch.fingerId.ToString(), touch.phase, touch.position);
			}
		
			Ui.instance.debug = summary;
		}
	}

}
