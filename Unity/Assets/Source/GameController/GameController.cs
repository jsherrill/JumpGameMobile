using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static readonly string MSG_RESET_GAME = "MSG_RESET_GAME";

	private bool hasGeneratedTiles = false;
	Player player = null;
	TileGenerator tileGenerator = null;

	public Player Player()
	{
		return player;
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
	}

	void OnDestroy() {
		RemoveMessageHandlers ();
	}

	// Update is called once per frame
	void Update () {
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
				tileGenerator.RecycleTiles (player.transform.position.y - 10f);
			}
		}
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

		Rigidbody playerRigidBody = tileCollision.Player.GetComponent<Rigidbody> ();

		if (playerRigidBody != null)
		{
			playerRigidBody.velocity = Vector3.zero;
		}

		tileCollision.Player.CurrentMomentum = tileCollision.Player.InitialJumpForce;

//		if (isBottomCollision)
//		{
//			tileCollision.Player.CurrentMomentum = tileCollision.Player.InitialJumpForce;
//		} else
//		{
//			if (tileCollision.Collision.collider.name != "Plane")
//			{
//				tileCollision.Player.CurrentMomentum = tileCollision.Player.InitialJumpForce;
//			} else
//			{
//				tileCollision.Player.CurrentMomentum = 0f;
//			}
//			Debug.Log ("Top Collision Detected");
//		}
	}

	private void ResetGame()
	{
		// reset the colliders in editor
		#if DEBUG && (UNITY_EDITOR || UNITY_EDITOR_OSX)
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
		}
		#endif
	}
}
