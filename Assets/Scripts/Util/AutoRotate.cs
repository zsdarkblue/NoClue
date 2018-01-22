using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

	public float speed = 0.01f;

	public Vector3 rotateAxis = new Vector3(0,1,0);
	// Update is called once per frame
	void Update () {
		Vector3 target = rotateAxis * Time.deltaTime * speed;
		transform.Rotate (target,Space.Self);
	}
}
