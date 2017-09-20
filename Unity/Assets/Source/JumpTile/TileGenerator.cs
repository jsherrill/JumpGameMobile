using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	[SerializeField]
	private float minTileX = -10f;

	[SerializeField]
	private float maxTileX = 10f;

	[SerializeField]
	private float tileGapY = 5f;

	private float lastTileY = 5f;

	private JumpTile[] jumpTiles = null;

	public JumpTile[] JumpTiles
	{
		get { return jumpTiles; }
		private set { }
	}

	// Use this for initialization
	void Start () {
		jumpTiles = GameObject.FindObjectsOfType<JumpTile> ();	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void RecycleTiles(float cutoffHeight)
	{
		if (jumpTiles == null || jumpTiles.Length == 0)
			return;

		for (int i = 0; i < jumpTiles.Length; i++)
		{
			if (jumpTiles [i].transform.position.y < cutoffHeight)
			{
				GenerateSingleTile (jumpTiles [i]);
			}
		}
	}

	public void GenerateTileLayout(float playerHeight)
	{
		if (jumpTiles == null || jumpTiles.Length == 0)
			return;

		List<JumpTile> useableTiles = new List<JumpTile> ();

		for (int i = 0; i < jumpTiles.Length; i++)
		{
			if (jumpTiles [i].transform.position.y < playerHeight - 100f)
			{
				useableTiles.Add (jumpTiles [i]);
			} else if (!jumpTiles [i].gameObject.activeSelf)
			{
				useableTiles.Add (jumpTiles [i]);
			}
		}

		if (useableTiles == null || useableTiles.Count == 0)
			return;

		for (int i = 0; i < useableTiles.Count; i++)
		{
			GenerateSingleTile (useableTiles [i]);
		}
	}

	private void GenerateSingleTile(JumpTile tile)
	{
		if (tile == null)
			return;

		Vector3 tilePosition = tile.transform.position;
		tilePosition.y = lastTileY + tileGapY;

		lastTileY = tilePosition.y;

		tilePosition.x = Random.Range (minTileX, maxTileX + 1);

		tile.transform.position = tilePosition;

		if (!tile.gameObject.activeSelf)
			tile.gameObject.SetActive (true);

		Collider tileCollider = tile.GetComponent<Collider> ();
		if (tileCollider != null && !tileCollider.enabled)
			tileCollider.enabled = true;

		int tileType = Random.Range (0, 201);

		MeshRenderer tileMesh = tile.GetComponentInChildren<MeshRenderer> ();
		bool isSpecialTile = false;

		if (tileType == 1)
		{
			tile.Type = JumpTile.TileType.DOUBLE_SCORE;
			tile.MoveStyle = JumpTile.MovementStyle.HORIZONTAL;
			tileMesh.material.color = Color.blue;
			isSpecialTile = true;
		}

		//if (tilePosition.y > 100f)
		{
			// brick
			if (tileType >= 10 && tileType <= 15)
			{
				tile.Type = JumpTile.TileType.BRICK;

				tileMesh.material.color = Color.red;
				isSpecialTile = true;
			}
		}

		//if (tilePosition.y > 500f)
		{
			// spike
			if (tileType == 200)
			{
				tile.Type = JumpTile.TileType.SPIKE;

				tileMesh.material.color = Color.black;
				isSpecialTile = true;
			}

			// shield
			if (tileType >= 50 && tileType <= 55)
			{
				tile.Type = JumpTile.TileType.SHIELD;

				tileMesh.material.color = Color.cyan;
				isSpecialTile = true;
			}
		}

		//if (tilePosition.y > 1000f)
		{
			if (tileType == 150)
			{
				tile.Type = JumpTile.TileType.ROCKET;

				tileMesh.material.color = Color.magenta;
				isSpecialTile = true;
			}
		}

		if (!isSpecialTile)
		{
			tile.MoveStyle = JumpTile.MovementStyle.NONE;
			tile.Type = JumpTile.TileType.REGULAR;
			tileMesh.material.color = Color.white;
		}
	}

	public void Reset()
	{
		lastTileY = 0f;

		if (jumpTiles != null && jumpTiles.Length > 0)
		{
			MeshRenderer tileMesh = null;
			for (int i = 0; i < jumpTiles.Length; i++)
			{
				// disable the gameobject so GenerateTileLayout will select it as useable
				jumpTiles[i].gameObject.SetActive(false);
				jumpTiles [i].transform.position = Vector3.zero;
				jumpTiles [i].Type = JumpTile.TileType.REGULAR;
				jumpTiles [i].MoveStyle = JumpTile.MovementStyle.NONE;

				tileMesh = jumpTiles [i].GetComponent<MeshRenderer> ();
				if (tileMesh != null)
				{
					tileMesh.material.color = Color.white;
				}
			}
		}
	}
}
