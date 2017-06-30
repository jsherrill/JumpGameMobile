using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTile : MonoBehaviour {

	public enum TileType
	{
		REGULAR,
		SPRING,
		DOUBLE_SCORE,
		BRICK,
		SPIKE,
		SHIELD,
		ROCKET,
		NUM_TYPES
	}

	protected TileType tileType = TileType.REGULAR;
	protected int tileValue = 10;

	public TileType Type
	{
		get { return tileType; }
		set { tileType = value; }
	}

	public int TileValue
	{
		get { return tileValue; }
		set { tileValue = value; }
	}

	// Use this for initialization
	protected virtual void Start () {
		AddMessageHandlers ();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == Tags.Player)
		{
			Player player = collision.collider.GetComponent<Player> ();

			switch (tileType)
			{
				case TileType.SHIELD:
					if (player != null && player.PlayerEffects != null)
					{
						PlayerEffect shieldEffect = player.PlayerEffects.Find (e => e.Type == PlayerEffect.EffectType.SHIELD);

						if (shieldEffect == null)
						{
							player.AddPlayerEffect (new ShieldEffect (0f, player));
						}
					}
					break;

				case TileType.ROCKET:
					if (player != null && player.PlayerEffects != null)
					{
						PlayerEffect rocketEffect = player.PlayerEffects.Find (e => e.Type == PlayerEffect.EffectType.ROCKET_JUMP);

						if (rocketEffect == null)
						{
							player.AddPlayerEffect (new RocketJumpEffect (3f, 50f, player));
						}
					}
					gameObject.SetActive (false);
					break;

				case TileType.BRICK:
					return;

				case TileType.SPIKE:
					return;

				default:
					gameObject.SetActive (false);
					break;
			}
		}
	}

	protected virtual void OnDestroy() {
		RemoveMessageHandlers ();
	}

	protected virtual void AddMessageHandlers() {

	}

	protected virtual void RemoveMessageHandlers() {

	}
}
