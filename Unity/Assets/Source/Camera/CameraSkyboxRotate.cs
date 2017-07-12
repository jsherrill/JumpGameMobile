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
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate()
	{
		if (gameController != null)
		{
			float rot = -gameController.Player ().CurrentMomentum * 0.25f;
			cameraTransform.Rotate (cameraTransform.right, rot * Time.deltaTime);
		}
	}
}
