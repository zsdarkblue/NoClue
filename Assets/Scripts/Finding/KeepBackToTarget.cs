using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepBackToTarget : MonoBehaviour {

	public Transform target;

	// Update is called once per frame
	void Update () {
		Vector3 dir = transform.position - target.position;
		//dir.z = 0;
		//transform.LookAt (transform.position + dir * 100);
	}
}
