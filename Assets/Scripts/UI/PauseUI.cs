using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;
using BeatsFever.GameState;
using System;

public class PauseUI : FrameBaseUI {
	public GameObject BackToLobbyButton;
	public GameObject ResumeButton;

	void Awake()
	{
		
	}
	void Start () {
	
	}

	public void BackToLobby()
	{
		(App.Game.GameStateMgr.ActiveState as BeatState).UserForceEnd ();
		Time.timeScale = 1;
	}

	public void ResumeMusic()
	{
		//(App.Game.GameStateMgr.ActiveState as BeatState).musicBulletPlayer.OnUserInput(InputType.Resume);
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.B)) {
			BackToLobby ();
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			ResumeMusic ();
		}
	}

	public override void FadeIn()
	{
		if (App.Game.GUIFrameMgr.IsActive (GUIFrameID.ResultUI)) {
			App.Game.GUIFrameMgr.GetFrame (GUIFrameID.ResultUI).CallWapperFunction ("SetRootActive", false);
		}

		//string name = Enum.GetName(typeof(AudioResources), AudioResources.beatsfever);
		//App.SoundMgr.Stop (name);
	}

	public override void FadeOut()
	{
		if (App.Game.GUIFrameMgr.IsActive (GUIFrameID.ResultUI)) {
			App.Game.GUIFrameMgr.GetFrame (GUIFrameID.ResultUI).CallWapperFunction ("SetRootActive", true);
		}

		ProcessCloseUI ();
	}
		
	void ProcessCloseUI()
	{
		DeActive ();
	}
}
