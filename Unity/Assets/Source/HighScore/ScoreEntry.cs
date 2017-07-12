using System.Collections;
using System.Collections.Generic;

public class ScoreEntry {

	public enum ScoreType
	{
		HIGH_SCORE,
		MAX_HEIGHT,
		NUM_TYPES
	}

	private long score;
	private string initials;
	private ScoreType scoreType;

	public long Score
	{
		get { return score; }
		set { score = value; }
	}

	public string Initials
	{
		get { return initials; }
		set { initials = value; }
	}

	public ScoreType EntryType
	{
		get { return scoreType; }
		set { scoreType = value; }
	}

	public ScoreEntry(long score = 0, string initials = "", ScoreType type = ScoreType.NUM_TYPES)
	{
		this.score = score;
		this.initials = initials;
		this.scoreType = type;
	}
}
