﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScoreUpdater : MonoBehaviour {
	private UnityEngine.UI.Text scoreText = null;
	private GameController gameController = null;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindObjectOfType<GameController> ();
		scoreText = GetComponent<UnityEngine.UI.Text> ();

		if (gameController == null)
		{
			Debug.LogWarning ("UIScoreUpdater::No Game Controller found.");
		}

		if (scoreText == null)
		{
			Debug.LogWarning ("UIScoreUpdater::Text component not found.  Attach script to GameObject containing a UI Text component.");
		}
		AddMessageListeners ();
	}

	void OnDestroy() {
		RemoveMessageListeners ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void AddMessageListeners() {
		Messenger<JumpTile>.AddListener (MessengerEventNames.JumpTileHit, OnJumpTileHit);
	}

	void RemoveMessageListeners() {
		Messenger<JumpTile>.RemoveListener (MessengerEventNames.JumpTileHit, OnJumpTileHit);
	}

	void OnJumpTileHit(JumpTile tile)
	{
		if (gameController != null && scoreText != null)
		{
			scoreText.text = string.Format ("SCORE: {0}", gameController.PlayerScore().ToString ());
		}
	}
}
