using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickBehaviour : MonoBehaviour {
	public bool IgnorePick = false;

	public virtual void OnRayEnter()
	{
		
	}

	public virtual void OnRayExit()
	{

	}

	public virtual bool TriggerPlayerPickAction()
	{
		return false;
	}
}
