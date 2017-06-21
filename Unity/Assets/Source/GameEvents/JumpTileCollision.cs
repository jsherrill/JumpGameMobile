using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTileCollision {

	public Collision Collision
	{
		get;
		set;
	}

	public JumpTile Tile
	{
		get;
		set;
	}

	public Player Player
	{
		get;
		set;
	}

	public JumpTileCollision(Collision col, Player player, JumpTile tile)
	{
		Collision = col;
		Tile = tile;
		Player = player;
	}
}
