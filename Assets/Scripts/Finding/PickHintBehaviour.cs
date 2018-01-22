using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickHintBehaviour : PickBehaviour {

	public Transform ScaleRoot;

	float targetScale = 1;
	public override bool TriggerPlayerPickAction()
	{
		if (DeviceContainer.Instance.CurrentLeftHintCount > 0) {
			//pick wrong item
			DeviceContainer.Instance.PickItemContainer_Current.RandomFindOne ();
			DeviceContainer.Instance.CurrentLeftHintCount--;
			DeviceContainer.Instance.HintCount.text = DeviceContainer.Instance.CurrentLeftHintCount.ToString() + " / 5";

			return true;
		} else {
			return false;
		}
	}

	public override void OnRayEnter()
	{
		targetScale = 1.5f;
	}

	public override void OnRayExit()
	{
		targetScale = 1f;
	}

	void Update()
	{
		ScaleRoot.transform.localScale = Vector3.Lerp (ScaleRoot.transform.localScale,Vector3.one * targetScale,Time.deltaTime * 3f);
	}


}
