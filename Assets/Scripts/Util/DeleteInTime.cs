using UnityEngine;
using System.Collections;

public class DeleteInTime : MonoBehaviour {

	public void StartCounting(float leftTime)
	{
		Invoke ("DoDelete", leftTime);
	}

	public void Stop()
	{
		CancelInvoke ("DoDelete");
	}
	void DoDelete()
	{
		GameObject.Destroy (this.gameObject);
	}
}
