using UnityEngine;
using System.Collections;

public class RotateSample : MonoBehaviour
{	
	Vector3 dir;
	void Start(){
		Debug.Log ("MoveTo by unity start, time:" + Time.time);
		//iTween.RotateBy(gameObject, iTween.Hash("position", Vector3.zero, "easeType", "linear","time",2));
		dir = Vector3.zero - transform.position;
	}
	bool isEnd = false;
	void Update()
	{
		if (isEnd) {
			return;
		}
		transform.position += dir.normalized * Time.deltaTime * 10;
		var currentDir = Vector3.zero - transform.position;
		var dot = Vector3.Dot (dir, currentDir);
		Debug.Log (dot);
		if (dot < 0) {
			Debug.Log ("MoveTo by unity finish, time:" + Time.time);
			isEnd = true;
		}
	}
}

