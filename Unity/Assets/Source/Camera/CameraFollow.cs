using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	[SerializeField]
	private GameObject followTarget = null;

	[SerializeField]
	private float followDistance = 10f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		FollowTarget ();		
	}

	void FollowTarget()
	{
		if (followTarget == null)
			return;

		transform.position = Vector3.Lerp(transform.position, followTarget.transform.position - transform.forward * followDistance, Time.deltaTime);
	}
}
