using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreTable : MonoBehaviour {

	private List<ScoreEntry> highScores = new List<ScoreEntry>();
	private List<ScoreEntry> maxHeights = new List<ScoreEntry>();

	public List<ScoreEntry> HighScores
	{
		get { return highScores; }
		private set { }
	}

	public List<ScoreEntry> MaxHeights
	{
		get { return maxHeights; }
		private set { }
	}

	// Use this for initialization
	void Start () {
		highScores.Clear ();
		maxHeights.Clear ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ClearHighScores()
	{
		highScores.Clear ();
	}

	public void ClearMaxHeights()
	{
		maxHeights.Clear ();
	}

	public void AddScoreEntry(ScoreEntry entry)
	{
		if (entry == null)
			return;

		switch (entry.EntryType)
		{
			case ScoreEntry.ScoreType.HIGH_SCORE:
				highScores.Add (entry);
				break;

			case ScoreEntry.ScoreType.MAX_HEIGHT:
				maxHeights.Add (entry);
				break;
		}
	}
}
