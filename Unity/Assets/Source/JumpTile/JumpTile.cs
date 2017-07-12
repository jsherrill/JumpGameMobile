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

	public enum MovementStyle
	{
		NONE,
		VERTICAL,
		HORIZONTAL
	}

	protected int tileValue = 10;
	protected float moveMagnitude = 0.25f;
	protected Vector3 moveAxes = Vector3.zero;
	protected TileType tileType = TileType.REGULAR;
	protected MovementStyle movementStyle = MovementStyle.NONE;

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

	public float MoveMagnitude
	{
		get { return moveMagnitude; }
		set { moveMagnitude = value; }
	}

	public Vector3 MoveAxes
	{
		get { return moveAxes; }
		protected set { moveAxes = value; }
	}

	public MovementStyle MoveStyle
	{
		get { return movementStyle; }
		set 
		{ 
			movementStyle = value;

			switch (movementStyle)
			{
				case MovementStyle.HORIZONTAL:
					moveAxes = new Vector3 (1f, 0f, 0f);
					break;

				case MovementStyle.VERTICAL:
					moveAxes = new Vector3 (0f, 1f, 0f);
					break;

				case MovementStyle.NONE:
					moveAxes = Vector3.zero;
					break;
			}
		}
	}

	// Use this for initialization
	protected virtual void Start () {
		movementStyle = MovementStyle.VERTICAL;
		if (movementStyle != MovementStyle.NONE)
		{
			if (movementStyle == MovementStyle.HORIZONTAL)
			{
				moveAxes = new Vector3 (1f, 0f, 0f);
			} else if (movementStyle == MovementStyle.VERTICAL)
			{
				moveAxes = new Vector3 (0f, 1f, 0f);
			}

			moveAxes.Normalize ();
		}

		AddMessageHandlers ();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (movementStyle != MovementStyle.NONE)
		{
			transform.position += moveAxes * Mathf.Sin(Time.time) * moveMagnitude;
		}
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
					gameObject.SetActive (false);
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
					for (int i = 0; i < collision.contacts.Length; i++)
					{
						if (collision.contacts [i].point.y > transform.position.y)
						{
							gameObject.SetActive (false);
							break;
						}
					}
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
