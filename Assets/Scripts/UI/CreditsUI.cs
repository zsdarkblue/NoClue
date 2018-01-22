using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;
#if !Steamworks_Off
using Steamworks;
#endif

public class CreditsUI : FrameBaseUI {

	public UITexture MainTexture;

	Vector3 rootInitPos;
	void Awake()
	{
		
	}
	void Start () {
		MainTexture.gameObject.transform.localPosition = new Vector3 (0, -250f, 0);
	}

	public override void FadeIn()
	{
		// FOR TESTING PURPOSES ONLY!
//					SteamUserStats.ResetAllStats (true);
//					SteamUserStats.RequestCurrentStats ();

		MainTexture.alpha = 1;
		iTween.MoveTo(MainTexture.gameObject, iTween.Hash("position", Vector3.up * 250f, "easeType", "linear","time",30,"islocal",true,
			"oncompletetarget",gameObject,"oncomplete","OnTransFinish"));

		//SteamAPIMgr.Instance.CheckCreditsAchievementUnlock ();
	}

	public void OnTransFinish()
	{
		Invoke ("AutoDeactive",1);
	}

	void AutoDeactive()
	{
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.CreditsUI);
	}

	public override void FadeOut()
	{
//		TweenAlpha tween = MainTexture.GetComponent<TweenAlpha> ();
//		tween.AddOnFinished (() => {
//			
//		});
//		tween.PlayForward ();

		DoCloseUI();
	}

	void DoCloseUI()
	{
		CancelInvoke ();
		DeActive ();
	}

}
