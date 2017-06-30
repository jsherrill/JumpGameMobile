using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	bool hasJumped = false;

	[SerializeField]
	private float initialJumpForce = 10f;

	private float currentMomentum = 0f;
	private float mouseDeltaX = 0f;

	private long playerScore = 0;

	private Vector3 previousMousePosition = Vector3.zero;

	private Rigidbody rigidBody = null;

	private List<PlayerEffect> playerEffects = new List<PlayerEffect>();

	private MeshRenderer meshRenderer = null;

	public bool HasJumped
	{
		get { return hasJumped; }
		private set { }
	}

	public float InitialJumpForce
	{
		get { return initialJumpForce; }
		set { initialJumpForce = value; }
	}

	public float CurrentMomentum
	{
		get { return currentMomentum; }
		set { currentMomentum = value; }
	}

	public long PlayerScore
	{
		get { return playerScore; }
		set { playerScore = value; }
	}

	public Rigidbody Rigidbody
	{
		get { return rigidBody; }
		set { rigidBody = value; }
	}

	public List<PlayerEffect> PlayerEffects
	{
		get { return playerEffects; }
		private set { playerEffects = value; }
	}

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();

		meshRenderer = GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR || UNITY_EDITOR_OSX
		//HandleInput ();
		#endif

		HandleTouchInput ();
		UpdatePlayerEffects ();
		UpdatePosition ();

		Vector3 lineStart = transform.position + -transform.up * 10f + -transform.right * 10f;
		Vector3 lineEnd = transform.position + -transform.up * 10f + transform.right * 10f;
		Debug.DrawLine (lineStart, lineEnd);
	}

	void HandleInput() 
	{
		if (Input.GetMouseButtonDown (0))
		{
			hasJumped = true;

			currentMomentum = initialJumpForce;
		}

		if (previousMousePosition == Vector3.zero)
		{
			previousMousePosition = Input.mousePosition;
		}

		mouseDeltaX = Input.mousePosition.x - previousMousePosition.x;
		previousMousePosition = Input.mousePosition;
	}

	void HandleTouchInput()
	{
		if (Input.touchCount > 0)
		{
			Touch touch;
			for (int i = 0; i < Input.touches.Length; i++)
			{
				touch = Input.touches [i];

				if (touch.phase == TouchPhase.Began && !hasJumped)
				{
					currentMomentum = initialJumpForce;
					hasJumped = true;
				} else if (touch.phase == TouchPhase.Moved)
				{
					mouseDeltaX = touch.deltaPosition.x;
				}
			}
		} else
		{
			mouseDeltaX = 0f;
		}
	}

	void UpdatePosition()
	{
		if (mouseDeltaX != 0f)
		{
			if (mouseDeltaX < 0)
			{
				transform.position = transform.position + transform.right * mouseDeltaX * Time.deltaTime;
			} else
			{
				transform.position = transform.position + transform.right * mouseDeltaX * Time.deltaTime;
			}
		}

		if (currentMomentum != 0f)
		{
			transform.position += transform.up * currentMomentum * Time.deltaTime;
		}
	}

	void UpdatePlayerEffects()
	{
		if (playerEffects == null || playerEffects.Count == 0)
		{
			if (meshRenderer != null)
			{
				meshRenderer.material.color = Color.white;
			}
			return;
		}

		for (int i = 0; i < playerEffects.Count; i++)
		{
			playerEffects [i].Update ();

			// duration effects will remove them
			if (i >= playerEffects.Count)
				break;

			if (playerEffects [i].Type == PlayerEffect.EffectType.SHIELD)
			{
				if (meshRenderer != null)
				{
					meshRenderer.material.color = Color.cyan;
				}
			}

			if (playerEffects [i].Type == PlayerEffect.EffectType.SHIELD)
			{
				if (meshRenderer != null)
				{
					meshRenderer.material.color = Color.magenta;
				}
			}
		}

		playerEffects.RemoveAll (e => e.IsExpired);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag == Tags.JumpTile)
		{
			JumpTileCollision tileCollision = new JumpTileCollision (col, this, col.collider.GetComponent<JumpTile> ());
			Messenger<JumpTileCollision>.Broadcast (MessengerEventNames.JumpTileHit, tileCollision, MessengerMode.DONT_REQUIRE_LISTENER);
		} else
		{
			Messenger.Broadcast (GameController.MSG_RESET_GAME, MessengerMode.DONT_REQUIRE_LISTENER);
		}
	}

	public void Reset()
	{
		currentMomentum = 0f;
		hasJumped = false;

		if (rigidBody != null)
		{
			rigidBody.velocity = Vector3.zero;
		}

		transform.position = Vector3.zero;

		if (playerEffects != null && playerEffects.Count > 0)
		{
			playerEffects.Clear ();
		}
	}

	public void AddPlayerEffect(PlayerEffect effect)
	{
		if (effect == null || playerEffects == null)
			return;

		playerEffects.Add (effect);
	}
}
