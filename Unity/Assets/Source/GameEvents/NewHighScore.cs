using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHighScore {
	public static readonly string MSG_NEW_HIGH_SCORE = "MSG_NEW_HIGH_SCORE";

	private uint typeFlag;

	public bool IsNewHighScore
	{
		get { return (typeFlag & 0x1) != 0; }
		private set { }
	}

	public bool IsNewMaxHeight
	{
		get { return (typeFlag & 0x2) != 0; }
		private set { }
	}

	public long Score
	{
		get;
		set;
	}

	public float Height
	{
		get;
		set;
	}

	public NewHighScore(long score = 0, float height = 0f)
	{
		if (score > 0)
		{
			typeFlag |= 0x1;
			Score = score;
		}

		if (height > 0)
		{
			typeFlag |= 0x2;
			Height = height;
		}
	}

	public void SaveScores()
	{
		if (IsNewHighScore)
		{
			PlayerPrefs.SetInt ("HighScore", (int)Score);
		}

		if (IsNewMaxHeight)
		{
			PlayerPrefs.SetFloat ("MaxHeight", Height);
		}
	}
}
