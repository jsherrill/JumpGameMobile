using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeightUpdater : MonoBehaviour {
	UnityEngine.UI.Text textLabel = null;
	private GameController gameController = null;

	// Use this for initialization
	void Start () {
		textLabel = GetComponent<UnityEngine.UI.Text> ();

		gameController = GameObject.FindObjectOfType<GameController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (textLabel != null && gameController != null)
		{
			if (gameController.Player () != null)
			{
				float meters = gameController.Player ().transform.position.y;
				textLabel.text = string.Format ("Height: {0} Meters", (int)meters);
			}
		}
	}
}
