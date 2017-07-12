using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static readonly string MSG_RESET_GAME = "MSG_RESET_GAME";
	public static readonly string MSG_END_GAME = "MSG_END_GAME";
	public static bool isPaused = false;

	private bool hasGeneratedTiles = false;
	private float playerMaxHeight = 0f;
	private float playerFallHeight = 0f;
	private Vector3 playerStartPosition = Vector3.zero;
	Player player = null;
	TileGenerator tileGenerator = null;

	[SerializeField]
	Transform particleContainer = null;

	public Player Player()
	{
		return player;
	}

	public float PlayerMaxHeight()
	{
		return playerMaxHeight;
	}
		
	// Use this for initialization
	void Start () {
		AddMessageHandlers ();

		player = GameObject.FindObjectOfType<Player> ();

		if (player == null)
		{
			GameObject playerGameObj = GameObject.FindGameObjectWithTag (Tags.Player);

			if (playerGameObj != null)
			{
				player = playerGameObj.GetComponent<Player> ();
			} else
			{
				Debug.LogWarning ("No player object found in scene!");
			}
		}

		if (player)
		{
			playerStartPosition = player.transform.position;
		}

		tileGenerator = GetComponent<TileGenerator> ();

		if (tileGenerator == null)
		{
			tileGenerator = GameObject.FindObjectOfType<TileGenerator> ();
		}

		// generate the initial layout
		if (tileGenerator != null)
		{
			tileGenerator.GenerateTileLayout (float.MaxValue);
		}

		#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
		PlayerPrefs.DeleteAll();
		#endif

		DisableParticles ();

		StartCoroutine (GetHighScoresWWW ());
	}

	void OnDestroy() {
		RemoveMessageHandlers ();
	}

	// Update is called once per frame
	void Update () {
		if (isPaused)
			return;
		
		if (player != null)
		{
			if ((int)player.transform.position.y % 50 == 0 && player.HasJumped)
			{
				if (!hasGeneratedTiles)
				{
					Debug.Log ("Generating tile layout at height: " + (int)player.transform.position.y * 2);
					hasGeneratedTiles = true;
					tileGenerator.GenerateTileLayout (player.transform.position.y - 10f);
				}
			} else
			{
				hasGeneratedTiles = false;
			}

			if (tileGenerator != null)
			{
				tileGenerator.RecycleTiles (player.transform.position.y - 100f);
			}

			if (player.transform.position.y > playerMaxHeight)
			{
				playerMaxHeight = player.transform.position.y;
				playerFallHeight = playerMaxHeight - 35f;
			}

			if (player.transform.position.y < playerFallHeight)
			{
				Debug.Log ("Player has fallen below the line!! They lose.");
				ResetGame ();
			}
		}
	}

	void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus && isPaused)
		{
			StartCoroutine (UnpauseRoutine());
		} else
		{
			isPaused = !hasFocus;
		}
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus && isPaused)
		{
			StartCoroutine (UnpauseRoutine());
		} else
		{
			isPaused = pauseStatus;
		}
	}

	public void SetPause(bool pauseStatus)
	{
		OnApplicationPause (pauseStatus);
	}

	private IEnumerator UnpauseRoutine()
	{
		Messenger<string>.Broadcast (UIManager.MSG_SET_NOTIFICATION, "Get Ready!", MessengerMode.DONT_REQUIRE_LISTENER);

		yield return new WaitForSeconds (0.5f);

		int ticks = 3;

		for (int i = ticks; i > 0; i--)
		{
			Messenger<string>.Broadcast (UIManager.MSG_SET_NOTIFICATION, string.Format ("{0}...", i), MessengerMode.DONT_REQUIRE_LISTENER);
			yield return new WaitForSeconds (0.5f);
		}

		Messenger<string>.Broadcast (UIManager.MSG_SET_NOTIFICATION, "GO!", MessengerMode.DONT_REQUIRE_LISTENER);

		yield return new WaitForSeconds (0.25f);

		Messenger<string>.Broadcast (UIManager.MSG_SET_NOTIFICATION, string.Empty, MessengerMode.DONT_REQUIRE_LISTENER);
		isPaused = false;
	}

	private void AddMessageHandlers()
	{
		Messenger.AddListener (MSG_RESET_GAME, ResetGame);
		Messenger<JumpTileCollision>.AddListener (MessengerEventNames.JumpTileHit, OnJumpTileHit);
	}

	private void RemoveMessageHandlers()
	{
		Messenger.RemoveListener (MSG_RESET_GAME, ResetGame);
		Messenger<JumpTileCollision>.RemoveListener (MessengerEventNames.JumpTileHit, OnJumpTileHit);
	}

	public long PlayerScore()
	{
		return player != null ? player.PlayerScore : 0;
	}

	private void CheckForHighScore()
	{
		long highScore = (long)PlayerPrefs.GetInt ("HighScore", -1);
		float maxHeight = PlayerPrefs.GetFloat ("MaxHeight", -1);


		bool isNewHighScore = false;
		bool isNewMaxHeight = false;

		isNewHighScore = highScore == -1 || player.PlayerScore > highScore;
		isNewMaxHeight = maxHeight == -1 || playerMaxHeight > maxHeight;

		if (isNewHighScore || isNewMaxHeight)
		{
			
			NewHighScore newScore = new NewHighScore (isNewHighScore ? player.PlayerScore : 0,
				                        isNewMaxHeight ? playerMaxHeight : 0f);
			newScore.SaveScores ();
			Messenger<NewHighScore>.Broadcast (NewHighScore.MSG_NEW_HIGH_SCORE, newScore, MessengerMode.DONT_REQUIRE_LISTENER);
		} else
		{
			Messenger<EndGameEvent>.Broadcast (MSG_END_GAME, new EndGameEvent (player.PlayerScore, playerMaxHeight), MessengerMode.DONT_REQUIRE_LISTENER);
		}
	}

	private IEnumerator CheckForHighScoreWWW()
	{
		long score = player.PlayerScore;
		float height = Mathf.Floor(playerMaxHeight);

		var networkReachability = Application.internetReachability;
		var networkConnectionStatus = Network.TestConnection ();

		if (networkReachability == NetworkReachability.NotReachable || networkConnectionStatus == ConnectionTesterStatus.Error ||
			networkConnectionStatus == ConnectionTesterStatus.Undetermined)
		{
			Messenger<EndGameEvent>.Broadcast (MSG_END_GAME, new EndGameEvent (score, height), MessengerMode.DONT_REQUIRE_LISTENER);
			yield break;
		}

		WWWForm requestData = new WWWForm ();
		requestData.AddField ("score", score.ToString());
		requestData.AddField ("height", height.ToString());
		requestData.AddField ("initials", "ZZZ");

		//string dbURL = "http://10.0.0.10:1337/connect";
		string dbURL = "http://71.229.150.150:1337/score";

		WWW request = new WWW (dbURL, requestData);

		yield return new WaitUntil (() => request.isDone);

		if (request.error == null)
		{
			if (!string.IsNullOrEmpty (request.text))
			{
				string[] responseParams = request.text.Split(':');

				if (responseParams.Length == 3)
				{
					if (responseParams[0] == "0")
					{
						int highScoreRank = System.Convert.ToInt32 (responseParams [1]);
						int heightRank = System.Convert.ToInt32 (responseParams [2]);

						bool isNewHighScore = highScoreRank != -1;
						bool isNewMaxHeight = heightRank != -1;

						if (isNewHighScore || isNewMaxHeight)
						{
							NewHighScore newScore = new NewHighScore (isNewHighScore ? score : 0,
								isNewMaxHeight ? height : 0f);
							newScore.SaveScores ();
							Messenger<NewHighScore>.Broadcast (NewHighScore.MSG_NEW_HIGH_SCORE, newScore, MessengerMode.DONT_REQUIRE_LISTENER);
						} else
						{
							Messenger<EndGameEvent>.Broadcast (MSG_END_GAME, new EndGameEvent (player.PlayerScore, playerMaxHeight), MessengerMode.DONT_REQUIRE_LISTENER);
						}
					}
				}
			}
		} else
		{
			Debug.Log (request.error);
			Messenger<EndGameEvent>.Broadcast (MSG_END_GAME, new EndGameEvent (score, height), MessengerMode.DONT_REQUIRE_LISTENER);
		}
	}

	private IEnumerator GetHighScoresWWW()
	{
		string scoreURL = "http://71.229.150.150:1337/requestScores";
		string heightURL = "http://71.229.150.150:1337/requestHeights";

		WWW scoreRequest = new WWW (scoreURL);
		WWW heightRequest = new WWW (heightURL);

		Handheld.StartActivityIndicator ();

		yield return new WaitUntil (() => scoreRequest.isDone && heightRequest.isDone);

		Handheld.StopActivityIndicator();

		if (string.IsNullOrEmpty(scoreRequest.error))
		{
			if (!string.IsNullOrEmpty (scoreRequest.text))
			{
				ParseScoreData (scoreRequest.text, ScoreEntry.ScoreType.HIGH_SCORE);

				UIManager uiManager = GameObject.FindObjectOfType<UIManager> ();

				if (uiManager != null)
				{
					uiManager.PopulateHighScores ();
				}
			}
		} else
		{
			Debug.Log (scoreRequest.error);
		}

		if (string.IsNullOrEmpty (heightRequest.error))
		{
			if (!string.IsNullOrEmpty (heightRequest.text))
			{
				ParseScoreData (heightRequest.text, ScoreEntry.ScoreType.MAX_HEIGHT);
			}
		} else
		{
			Debug.Log (heightRequest.error);
		}
	}

	private void ParseScoreData(string data, ScoreEntry.ScoreType scoreType)
	{
		if (string.IsNullOrEmpty (data))
			return;

		HighScoreTable scoreTable = GameObject.FindObjectOfType<HighScoreTable> ();

		if (scoreTable == null)
			return;

		string[] scoreData = data.Split ('&');
		string[] entryData;

		if (scoreData != null && scoreData.Length > 0)
		{
			for (int i = 0; i < scoreData.Length; i++)
			{
				entryData = scoreData [i].Split (':');

				if (entryData.Length == 2)
				{
					Debug.Log (string.Format ("{0}: {1} - {2}", i, entryData [0], entryData [1]));
					ScoreEntry score = new ScoreEntry ((long)System.Convert.ToDouble (entryData [1]), entryData [0], scoreType);
					scoreTable.AddScoreEntry (score);
				}
			}
		}
	}

	private void OnJumpTileHit(JumpTileCollision tileCollision)
	{
		if (player == null)
			return;

		switch (tileCollision.Tile.Type)
		{
			case JumpTile.TileType.REGULAR:
				player.PlayerScore += tileCollision.Tile.TileValue;
				break;

			case JumpTile.TileType.DOUBLE_SCORE:
				player.PlayerScore *= 2;
				break;
		}

		bool isBottomCollision = false;

		for (int i = 0; i < tileCollision.Collision.contacts.Length; i++)
		{
			if (tileCollision.Collision.contacts [i].point.y > tileCollision.Player.transform.position.y)
			{
				isBottomCollision = true;
				break;
			}
		}

		switch (tileCollision.Tile.Type)
		{
			case JumpTile.TileType.BRICK:
				if (!isBottomCollision)
				{
					tileCollision.Player.Rigidbody.velocity = Vector3.zero;
				}
				tileCollision.Player.CurrentMomentum = isBottomCollision ? 0f : tileCollision.Player.InitialJumpForce * 0.75f;
				break;

			case JumpTile.TileType.SPIKE:
				{
					PlayerEffect shieldEffect = null;

					if (player.PlayerEffects != null && player.PlayerEffects.Count > 0)
					{
						shieldEffect = player.PlayerEffects.Find (e => e.Type == PlayerEffect.EffectType.SHIELD);
					}

					// shield effect protects the player from the effects of the spike tile
					if (shieldEffect != null)
					{
						tileCollision.Player.CurrentMomentum = tileCollision.Player.InitialJumpForce;
						player.PlayerEffects.Remove (shieldEffect);
						tileCollision.Collision.collider.enabled = false;
					} else
					{
						// spike tiles instantly kill the player
						tileCollision.Player.CurrentMomentum = 0f;
						ResetGame ();
					}
					tileCollision.Player.Rigidbody.velocity = Vector3.zero;
				}
				break;

			case JumpTile.TileType.ROCKET:
				tileCollision.Player.Rigidbody.velocity = Vector3.zero;
				break;

			default:
				tileCollision.Player.Rigidbody.velocity = Vector3.zero;
				tileCollision.Player.CurrentMomentum = tileCollision.Player.InitialJumpForce;
				EnableRegularParticleEffect (tileCollision.Tile.transform.position);
				break;
		}
	}

	private void ResetGame()
	{
		if (player.HasJumped)
		{
			StartCoroutine(CheckForHighScoreWWW ());
			//CheckForHighScore ();
		}

		// reset the colliders in editor
		if(tileGenerator == null || tileGenerator.JumpTiles == null || tileGenerator.JumpTiles.Length == 0)
			return;

		Collider tileCollider = null;
		foreach (var tile in tileGenerator.JumpTiles) 
		{
			tileCollider = tile.GetComponent<Collider>();

			if(tileCollider != null)
			{
				tileCollider.enabled = true;
			}

			if(!tile.gameObject.activeSelf)
			{
				tile.gameObject.SetActive(true);
			}
		}

		if(player != null)
		{
			player.PlayerScore = 0;
			playerMaxHeight = 0f;
			player.Reset();
			playerFallHeight = player.transform.position.y - 10f;
			player.transform.position = playerStartPosition;
		}

		if(tileGenerator != null)
		{
			tileGenerator.Reset();
			tileGenerator.GenerateTileLayout(player != null ? player.transform.position.y : 0f);
		}
	}

	void DisableParticles()
	{
		if (particleContainer != null && particleContainer.childCount > 0)
		{
			ParticleSystem particles;
			for (int i = 0; i < particleContainer.childCount; i++)
			{
				particles = particleContainer.GetChild (i).GetComponent<ParticleSystem> ();

				if (particles != null)
				{
					particles.Stop ();
				}
			}
		}
	}

	void EnableRegularParticleEffect(Vector3 position)
	{
		if (particleContainer != null && particleContainer.childCount > 0)
		{
			ParticleSystem particles;
			for (int i = 0; i < particleContainer.childCount; i++)
			{
				particles = particleContainer.GetChild (i).GetComponent<ParticleSystem> ();

				if (particles != null && !particles.isPlaying)
				{
					particles.transform.position = position;
					particles.Play ();
					break;
				}
			}
		}
	}
}
