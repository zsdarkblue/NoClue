using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosToTarget : MonoBehaviour {

	public Transform target;
	Vector3 localOffset = new Vector3(0,0,0.382f);
	float lerpSpeed = 7;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPos = target.TransformPoint(localOffset);
		transform.position = Vector3.Lerp (transform.position,targetPos,Time.deltaTime * lerpSpeed);

		Vector3 dir = transform.position - target.position;
		transform.LookAt (transform.position + dir * 100);
	}
}
