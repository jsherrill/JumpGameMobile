using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
	[SerializeField]
	private GameObject newHighScoreLabel = null;
	private UnityEngine.UI.Text highScoreText = null;

	[SerializeField]
	private GameObject newMaxHeightLabel = null;
	private UnityEngine.UI.Text maxHeightText = null;

	private float highScoreElapsed = 0f;
	private float maxHeightElapsed = 0f;

	// Use this for initialization
	void Start () {
		if (newHighScoreLabel != null)
		{
			highScoreText = newHighScoreLabel.GetComponentInChildren<UnityEngine.UI.Text> ();
			newHighScoreLabel.SetActive (false);
		}

		if (newMaxHeightLabel != null)
		{
			maxHeightText = newMaxHeightLabel.GetComponentInChildren<UnityEngine.UI.Text> ();
			newMaxHeightLabel.SetActive (false);
		}

		AddMessageListeners ();
	}

	void OnDestroy()
	{
		RemoveMessageListeners ();
	}

	// Update is called once per frame
	void Update () {
		if (newHighScoreLabel != null && newHighScoreLabel.activeSelf)
		{
			highScoreElapsed += Time.deltaTime;
			if (highScoreElapsed >= 10f)
			{
				newHighScoreLabel.gameObject.SetActive (false);
				highScoreElapsed = 0f;
			}
		}

		if (newMaxHeightLabel != null && newMaxHeightLabel.activeSelf)
		{
			maxHeightElapsed += Time.deltaTime;
			if (maxHeightElapsed >= 10f)
			{
				newMaxHeightLabel.gameObject.SetActive (false);
				maxHeightElapsed = 0f;
			}
		}
	}

	private void AddMessageListeners()
	{
		Messenger<NewHighScore>.AddListener (NewHighScore.MSG_NEW_HIGH_SCORE, OnNewHighScore);
		Messenger<EndGameEvent>.AddListener (GameController.MSG_END_GAME, OnEndGame);
	}

	private void RemoveMessageListeners()
	{
		Messenger<NewHighScore>.RemoveListener (NewHighScore.MSG_NEW_HIGH_SCORE, OnNewHighScore);
		Messenger<EndGameEvent>.RemoveListener (GameController.MSG_END_GAME, OnEndGame);
	}

	private void OnNewHighScore(NewHighScore newScore)
	{
		if (newScore.IsNewHighScore && newHighScoreLabel != null && highScoreText != null)
		{
			newHighScoreLabel.SetActive (true);
			highScoreText.text = string.Format ("New High Score: {0}", newScore.Score);
		}

		if (newScore.IsNewMaxHeight && newMaxHeightLabel != null && maxHeightText != null)
		{
			newMaxHeightLabel.SetActive (true);
			maxHeightText.text = string.Format ("New Max Height: {0} Meters", (int)newScore.Height);
		}
	}

	private void OnEndGame(EndGameEvent endGame)
	{
		if (newHighScoreLabel != null && highScoreText != null)
		{
			newHighScoreLabel.SetActive (true);
			highScoreText.text = string.Format ("Final Score: {0}", endGame.Score);
		}

		if (newMaxHeightLabel != null && maxHeightText != null)
		{
			newMaxHeightLabel.SetActive (true);
			maxHeightText.text = string.Format ("Final Height: {0} Meters", (int)endGame.Height);
		}
	}
}
