using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

	bool hasJumped = false;

	[SerializeField]
	private float initialJumpForce = 10f;

	private float lastMomentum = 0f;
	private float currentMomentum = 0f;
	private float mouseDeltaX = 0f;

	private long playerScore = 0;

	private Vector3 previousMousePosition = Vector3.zero;

	private Rigidbody rigidBody = null;

	private List<PlayerEffect> playerEffects = new List<PlayerEffect>();

	private SkinnedMeshRenderer meshRenderer = null;

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

		meshRenderer = GetComponentInChildren<SkinnedMeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameController.isPaused)
		{
			rigidBody.Sleep ();

			if (lastMomentum == 0f)
			{
				lastMomentum = currentMomentum;
				currentMomentum = 0f;
				mouseDeltaX = 0f;
				previousMousePosition = Vector3.zero;
			}
			return;
		} else
		{
			if (lastMomentum != 0f)
			{
				currentMomentum = lastMomentum;
				lastMomentum = 0f;
			}
		}

		#if UNITY_EDITOR || UNITY_EDITOR_OSX
		//HandleInput ();
		#endif

		HandleTouchInput ();
		UpdatePlayerEffects ();
		UpdatePosition ();
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
					// don't jump if the user is clicking a UI item
					if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (touch.fingerId))
					{
						continue;
					}

					currentMomentum = initialJumpForce;
					hasJumped = true;

					Messenger<string>.Broadcast (UIManager.MSG_SET_NOTIFICATION, string.Empty, MessengerMode.DONT_REQUIRE_LISTENER);
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
			transform.position = transform.position + transform.right * mouseDeltaX * Time.deltaTime;

			meshRenderer.transform.Rotate (Vector3.forward, -mouseDeltaX * 5f * Time.deltaTime);
		} else
		{
			meshRenderer.transform.rotation = Quaternion.Lerp (meshRenderer.transform.rotation, Quaternion.Euler (-90f, 0f, 0f), Time.deltaTime * 5f);
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
