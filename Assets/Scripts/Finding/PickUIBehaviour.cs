using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatsFever;

public class PickUIBehaviour : PickBehaviour {
	public string Message;
	public int param = -1;
	float scaleSpeed = 10;

	public static void ResetEventTimer()
	{
		isLaunchingEvent = false;
	}

	static bool isLaunchingEvent = false;
	public GameObject InfoObject;
	Vector3 targetLocalScale = Vector3.one;
	UIButton uiButton;
	void Awake()
	{
		uiButton = GetComponent<UIButton> ();

		if (null != InfoObject) {
			InfoObject.SetActive (false);
		}
	}

	public override bool TriggerPlayerPickAction()
	{
		if (isLaunchingEvent)
			return false;
		
		if (null != uiButton) {
			uiButton.SetState (UIButtonColor.State.Pressed,false);
			App.SoundMgr.Play (AudioResources.button_click);
		}

		isLaunchingEvent = true;
		Invoke ("DoSendEvent",0.5f);

		return true;
	}

	void DoSendEvent()
	{
		if (param == -1) {
			SendMessageUpwards (Message, SendMessageOptions.RequireReceiver);		
		}
		else {
			SendMessageUpwards (Message,param,SendMessageOptions.RequireReceiver);
		}
		isLaunchingEvent = false;
	}

	void Update()
	{
		transform.localScale = Vector3.Lerp (transform.localScale,targetLocalScale,Time.deltaTime * scaleSpeed);
	}

	public override void OnRayEnter()
	{
		if (isLaunchingEvent)
			return;
		
		if (null != uiButton) {
			uiButton.SetState (UIButtonColor.State.Hover,false);
		}

		if (null != InfoObject) {
			InfoObject.SetActive (true);
		}

		App.SoundMgr.Play (AudioResources.button_select);

		targetLocalScale = Vector3.one * 1.2f;
	}

	public override void OnRayExit()
	{
		if (isLaunchingEvent)
			return;
		
		if (null != uiButton) {
			uiButton.SetState (UIButtonColor.State.Normal,false);
		}

		if (null != InfoObject) {
			InfoObject.SetActive (false);
		}

		targetLocalScale = Vector3.one;
	}
}
