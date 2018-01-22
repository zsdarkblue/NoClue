using UnityEngine;
using System.Collections;

public class MoveSample2 : MonoBehaviour
{	
	void Start(){
		Debug.Log ("MoveTo by speed start, time:" + Time.time);

		Vector3 targetPos = new Vector3 (0,0,transform.position.z);
		iTween.MoveTo(gameObject, iTween.Hash("position", targetPos, "easeType", "linear","speed",10,"oncomplete","OnFinish"));
	}
	void OnFinish()
	{
		Debug.Log ("MoveTo by speed finish, time:" + Time.time);
	}
}

