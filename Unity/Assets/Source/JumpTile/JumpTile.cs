using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTile : MonoBehaviour {

	public enum TileType
	{
		REGULAR,
		SPRING,
		DOUBLE_SCORE,
		NUM_TYPES
	}

	protected TileType tileType = TileType.REGULAR;

	public TileType Type
	{
		get { return tileType; }
		protected set { tileType = value; }
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
		Messenger<JumpTile>.Broadcast(MessengerEventNames.JumpTileHit, this, MessengerMode.DONT_REQUIRE_LISTENER);
	}

	protected virtual void OnDestroy() {
		RemoveMessageHandlers ();
	}

	protected virtual void AddMessageHandlers() {

	}

	protected virtual void RemoveMessageHandlers() {

	}
}
