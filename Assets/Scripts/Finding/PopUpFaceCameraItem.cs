using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpFaceCameraItem : MonoBehaviour {

	public Transform cameraTransform;
	public UITexture sprite;

	float speed = 0.1f;
	bool isPopping = false;

	// Use this for initialization
	void Start () {
		isPopping = false;
	}

	public void PopUp(GameObject findObject)
	{
		Vector3 dir = findObject.transform.position - cameraTransform.position;
		dir.y = 0;

		transform.position = cameraTransform.position + dir.normalized * 1;
		//Vector3 dir = transform.position - cameraTransform.position;
		transform.LookAt (transform.position + dir * 100);

		sprite.enabled = true;
		isPopping = true;

		CancelInvoke ();
		Invoke ("PopEnd",1);
	}

	void PopEnd()
	{
		sprite.enabled = false;
		isPopping = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (isPopping) {
			transform.position += Vector3.up * Time.deltaTime * speed;
		}
	}
}
