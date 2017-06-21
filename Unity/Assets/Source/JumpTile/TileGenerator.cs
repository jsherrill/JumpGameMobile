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
			if (jumpTiles [i].transform.position.y < playerHeight)
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
	}
}
