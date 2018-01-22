using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatsFever;
//using BeatsFever.GameState;

public class DayNightSwitcher : MonoBehaviour {

	public const float ReduSteps = 2;  
	public LightsController DayContorller;
	public LightsController NightContorller;
	public bool IsLobbySwitcher = false;

	public bool IsDay = true;
	public bool IsSwitching = false;

	SkyboxElement skyElement = null;
	AmbientElement ambientElement;
	// Use this for initialization
	void Awake () {
		IsSwitching = false;
		float reduWeightInNight = 1;

		if (!IsLobbySwitcher) {
			if (DeviceContainer.Instance.gameMode == GameMode.Challenge) {
				//reduWeightInNight = Mathf.Max (0, (ReduSteps - App.Game.ScoreMgr.LevelIndex)) / ReduSteps;
				reduWeightInNight = (ReduSteps - (App.Game.ScoreMgr.LevelIndex % ReduSteps)) / ReduSteps;
				Debug.Log ("reduWeightInNight:" + reduWeightInNight);
			} else if (DeviceContainer.Instance.gameMode == GameMode.Story) {
				if (App.Game.ScoreMgr.LevelIndex == 5) {
					reduWeightInNight = 0;
				}
			}
		}

		DayContorller.Init (1);
		NightContorller.Init (reduWeightInNight);

		ambientElement = new AmbientElement (reduWeightInNight);

		GameObject skyObject = GameObject.Find ("SkyDome");
		if (null != skyObject) {
			skyElement = new SkyboxElement (skyObject,reduWeightInNight);
		} else {
			skyElement = null;
		}
	}
	
	// Update is called once per frame
	void Update () {

		#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.O)) {
			LaunchSwitchProcess ();
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			DeviceContainer.Instance.PickItemContainer_Current.RandomFindOne ();
		}
		#endif

		if (null != skyElement) {
			skyElement.Update ();
		}

		ambientElement.Update ();
	}

	public void LaunchSwitchProcess()
	{
		if (IsSwitching)
			return;
		
		IsSwitching = true;
		if (IsDay) {
			DayContorller.SwitchLights (false);
		} else {
			NightContorller.SwitchLights (false);
			DeviceContainer.Instance.SwitchFlashLight (false);
		}

		if (skyElement != null) {
			skyElement.TurnOff ();
		}

		ambientElement.TurnOff ();


		CancelInvoke ();
		//Invoke ("PrepareSwitch", 1f);
		Invoke ("SwitchLightsBack", 1.1f);
	}

//	void PrepareSwitch()
//	{
//		DeviceContainer.Instance.PickItemContainer_Current.PrepareSwitch (!IsDay);
//	}
//
	void SwitchLightsBack()
	{
		if (IsDay) {
			NightContorller.SwitchLights (true);
			DeviceContainer.Instance.SwitchFlashLight (true);
		} else {
			DayContorller.SwitchLights (true);
		}

		IsDay = !IsDay;

		if (skyElement != null) {
			skyElement.TurnOn (IsDay);
		}

		ambientElement.TurnOn (IsDay);
		DeviceContainer.Instance.PickItemContainer_Current.SwitchDayNight (IsDay);

		CancelInvoke ();
		Invoke ("SwitchEnd", 0.1f);
	}

	void SwitchEnd()
	{
		IsSwitching = false;
		//DeviceContainer.Instance.PickItemContainer_Current.EndSwitch (IsDay);
	}
}
