using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{	
	void Start(){
		Debug.Log ("MoveTo by time start, time:" + Time.time);

		Vector3 targetPos = new Vector3 (0,0,transform.position.z);
		iTween.MoveTo(gameObject, iTween.Hash("position", targetPos, "easeType", "linear","time",2,"oncomplete","OnFinish"));
	}

	void OnFinish()
	{
		Debug.Log ("MoveTo by time finish, time:" + Time.time);
	}
}

