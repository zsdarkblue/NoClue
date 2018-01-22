using UnityEngine;
using System.Collections;

public class QuickMessage : MonoBehaviour {

	public string Message;
	public int param = -1;

	void OnTriggerEnter(Collider other) {
		if (param == -1) {
			SendMessageUpwards (Message, SendMessageOptions.RequireReceiver);		
		}
		else {
			SendMessageUpwards (Message,param,SendMessageOptions.RequireReceiver);
		}
	}
}
