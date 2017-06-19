using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	bool hasJumped = false;

	[SerializeField]
	private float initialJumpForce = 10f;

	private float currentMomentum = 0f;
	private float mouseDeltaX = 0f;
	private float fallStart = 2f;
	private float fallElapsed = 0f;

	private long playerScore = 0;

	private Vector3 previousMousePosition = Vector3.zero;

	public long PlayerScore
	{
		get { return playerScore; }
		set { playerScore = value; }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR || UNITY_EDITOR_OSX
		//HandleInput ();
		#endif

		HandleTouchInput ();
		UpdatePosition ();

		if (hasJumped)
		{
			fallElapsed += Time.deltaTime;
			if (fallElapsed >= fallStart)
			{
				//currentMomentum = Mathf.Lerp (currentMomentum, 0f, Time.deltaTime);
			}
		}

		Debug.Log ("Current Momentum == " + currentMomentum.ToString ());
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
				//rigidBody.AddForce(-Vector3.right * (initialJumpForce / 2) * Time.deltaTime, ForceMode.Acceleration);
			} else
			{
				transform.position = transform.position + transform.right * mouseDeltaX * Time.deltaTime;
				//rigidBody.AddForce(Vector3.right * (initialJumpForce / 2) * Time.deltaTime, ForceMode.Acceleration);
			}
		}

		if (currentMomentum != 0f)
		{
			transform.position += transform.up * currentMomentum * Time.deltaTime;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		bool isBottomCollision = false;

		for (int i = 0; i < col.contacts.Length; i++)
		{
			if (col.contacts [i].point.y > transform.position.y)
			{
				isBottomCollision = true;
				break;
			}
		}

		if (isBottomCollision)
		{
			Debug.Log ("Bottom Collision Detected");
			col.collider.enabled = false;
			currentMomentum = initialJumpForce;
		} else
		{
			if (col.gameObject.name != "Plane")
			{
				currentMomentum = initialJumpForce;
			} else
			{
				hasJumped = false;
				currentMomentum = 0f;

				// reset the colliders in editor
				#if DEBUG && (UNITY_EDITOR || UNITY_EDITOR_OSX)
				Collider[] colliders = GameObject.FindObjectsOfType<Collider>();

				if(colliders != null && colliders.Length > 0)
				{
					foreach (var c in colliders) 
					{
						c.enabled = true;
					}
				}
				#endif
			}
			Debug.Log ("Top Collision Detected");
		}
	}

	void OnCollisionExit(Collision col)
	{
		if (!col.collider.enabled)
		{
			// exiting the block we jumped through, re-enable the collider
			col.collider.enabled = true;
		}
	}
}
