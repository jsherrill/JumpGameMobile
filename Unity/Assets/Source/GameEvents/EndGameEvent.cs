using UnityEngine;
using System.Collections;

public class EndGameEvent
{
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

	public EndGameEvent(long score, float height)
	{
		Score = score;
		Height = height;
	}
}

