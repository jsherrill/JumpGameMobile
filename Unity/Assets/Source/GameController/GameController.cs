using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	[SerializeField]
	private int regularTileWorth = 10;

	Player player = null;

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
	}

	void OnDestroy() {
		RemoveMessageHandlers ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	private void AddMessageHandlers()
	{
		Messenger<JumpTile>.AddListener (MessengerEventNames.JumpTileHit, OnJumpTileHit);
	}

	private void RemoveMessageHandlers()
	{
		Messenger<JumpTile>.RemoveListener (MessengerEventNames.JumpTileHit, OnJumpTileHit);
	}

	public long PlayerScore()
	{
		return player != null ? player.PlayerScore : 0;
	}

	private void OnJumpTileHit(JumpTile tile)
	{
		if (player == null)
			return;

		switch (tile.Type)
		{
			case JumpTile.TileType.REGULAR:
				player.PlayerScore += regularTileWorth;
				break;

			case JumpTile.TileType.DOUBLE_SCORE:
				player.PlayerScore *= 2;
				break;
		}
	}
}
