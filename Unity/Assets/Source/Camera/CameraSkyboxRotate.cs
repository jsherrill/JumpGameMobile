using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSkyboxRotate : MonoBehaviour {
	Transform cameraTransform;

	private GameController gameController = null;

	// Use this for initialization
	void Start () {
		cameraTransform = transform;

		gameController = GameObject.FindObjectOfType<GameController> ();

		float xRot = Random.Range (0f, 360f);
		float yRot = Random.Range (0f, 360f);
		float zRot = Random.Range (0f, 360f);

		cameraTransform.Rotate (new Vector3 (xRot, yRot, zRot));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate()
	{
		if (gameController != null)
		{
			float rot = -gameController.Player ().CurrentMomentum * 0.25f;
			cameraTransform.Rotate (Vector3.right, rot * Time.deltaTime);
		}
	}
}
