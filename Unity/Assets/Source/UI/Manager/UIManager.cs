using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
	public static readonly string MSG_SET_NOTIFICATION = "MSG_SET_NOTIFICATION";

	[SerializeField]
	private GameObject newHighScoreLabel = null;
	private UnityEngine.UI.Text highScoreText = null;

	[SerializeField]
	private GameObject newMaxHeightLabel = null;
	private UnityEngine.UI.Text maxHeightText = null;

	[SerializeField]
	private GameObject notificationLabel = null;
	private UnityEngine.UI.Text notificationText = null;

	[SerializeField]
	private GameObject highScoresPanel = null;

	[SerializeField]
	private GameObject highScoresContainer = null;

	[SerializeField]
	private UnityEngine.UI.Text highScoresTitle = null;

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

		if (notificationLabel != null)
		{
			notificationText = notificationLabel.GetComponentInChildren<UnityEngine.UI.Text> ();
			notificationLabel.SetActive (false);
		}

		if (highScoresPanel != null)
		{
			PopulateHighScores ();
			highScoresPanel.SetActive (false);
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
		Messenger<string>.AddListener (UIManager.MSG_SET_NOTIFICATION, SetNotificationText);
	}

	private void RemoveMessageListeners()
	{
		Messenger<NewHighScore>.RemoveListener (NewHighScore.MSG_NEW_HIGH_SCORE, OnNewHighScore);
		Messenger<EndGameEvent>.RemoveListener (GameController.MSG_END_GAME, OnEndGame);
		Messenger<string>.RemoveListener (UIManager.MSG_SET_NOTIFICATION, SetNotificationText);
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

	public void SetNotificationText(string text)
	{
		if (notificationLabel != null && notificationText != null)
		{
			notificationText.text = text;

			if (text != string.Empty)
			{
				notificationLabel.SetActive (true);
			} else
			{
				notificationLabel.SetActive (false);
			}
		}
	}

	public void SetHighScoresTitleText(string text)
	{
		if (highScoresTitle != null)
		{
			highScoresTitle.text = text;
		}
	}

	public void ShowHighScores()
	{
		if (highScoresPanel != null && !highScoresPanel.activeSelf)
		{
			highScoresPanel.SetActive (true);
		}
	}

	public void HideHighScores()
	{
		if (highScoresPanel != null && highScoresPanel.activeSelf)
		{
			highScoresPanel.SetActive (false);
		}
	}

	public void PopulateHighScores()
	{
		if (highScoresContainer == null)
			return;

		if (highScoresContainer.transform.childCount == 0)
			return;
		
		HighScoreTable scoreTable = GameObject.FindObjectOfType<HighScoreTable> ();

		UnityEngine.UI.Text scoreText = null;
		Transform child = null;

		if (scoreTable.HighScores.Count > 0)
		{
			for (int i = 0; i < scoreTable.HighScores.Count; i++)
			{
				child = highScoresContainer.transform.GetChild (i);

				if (child != null)
				{
					scoreText = child.GetComponent<UnityEngine.UI.Text> ();

					if (scoreText != null)
					{
						scoreText.text = string.Format ("#{0}. {1} - {2}", i + 1, scoreTable.HighScores [i].Initials, scoreTable.HighScores [i].Score); 
					}
				}
			}
		} else
		{
			for (int i = 0; i < highScoresContainer.transform.childCount; i++)
			{
				child = highScoresContainer.transform.GetChild (i);

				if (child != null)
				{
					scoreText = child.GetComponent<UnityEngine.UI.Text> ();

					if (scoreText != null)
					{
						scoreText.text = string.Format ("#{0}. ", i + 1);
					}
				}
			}
		}
	}
}
