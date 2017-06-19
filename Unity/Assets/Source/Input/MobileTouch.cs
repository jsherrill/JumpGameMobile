using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileTouch {
	public MobileTouch(int index)
	{
		TouchIndex = index;
		TouchStartTime = Time.time;
		TouchEndTime = 0f;
		TouchTotalTime = 0f;
	}

	public int TouchIndex
	{
		get;
		set;
	}

	public float TouchStartTime
	{
		get;
		set;
	}

	public float TouchEndTime
	{
		get;
		set;
	}

	public float TouchTotalTime
	{
		get { return TouchEndTime - TouchStartTime; }
		set { TouchTotalTime = value; }
	}
}
