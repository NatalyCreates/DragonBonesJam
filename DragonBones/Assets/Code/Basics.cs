using UnityEngine;
using System.Collections;

public class Basics {

	public static void assert(bool condition, string message = "Something went wrong") {
		if (!condition) {
			Basics.Log("Assertion failed: " + message);
			throw new UnityException(message);
		}
	}

	public static void Log(string message) {
		Debug.Log(message);
		Ui.instance.debug = message;
	}
}
