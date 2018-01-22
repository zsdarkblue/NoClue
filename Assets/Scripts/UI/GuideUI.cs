using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;
using BeatsFever.UI;

public class GuideUI : FrameBaseUI {

	public UILabel OculusDesc;

	public GameObject OculusBack;
	public GameObject ViveBack;
	public override void FadeIn()
	{
		if (DeviceContainer.Instance.IsLaunchedByOculus ()) {
			string content = OculusDesc.text;
			content = content.Replace ("touchpad", "button");
			OculusDesc.text = content;
			OculusBack.SetActive (true);
			ViveBack.SetActive (false);
		} else {
			OculusBack.SetActive (false);
			ViveBack.SetActive (true);
		}
	}

	public override void FadeOut()
	{
		ProcessCloseUI ();
	}

	void Next()
	{
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.GuideUI);
		App.Game.LocalDataBase.SetGuideOver (true);
	}

	void ProcessCloseUI()
	{
		App.Game.GUIFrameMgr.Active (GUIFrameID.RotateMusicSelectUI);
		DeActive ();
	}
}
