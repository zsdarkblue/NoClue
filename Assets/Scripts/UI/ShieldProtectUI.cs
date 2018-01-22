using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;
using BeatsFever.GameState;
using System.Collections.Generic;

public class ShieldProtectUI : FrameBaseUI {

	public GameObject ShieldScreen;
	public GameObject ShieldScanLine;
	public GameObject RankProgressBarRoot;

	public Transform[] ProgressAnchors;
	public GameObject DotItem;
	public GameObject IndicatorItem;
	public Material dotMaterial;

	const float fadeInTime = 1f;
	const int dotItemLength = 49;
	float hitPower = 0.0f;
	float hitAlpha = 1.0f;

	List<GameObject> dotItemList = new List<GameObject>();
	void Update()
	{
		transform.position = DeviceContainer.Instance.GetShieldGlassPosition();

//		App.Game.ScoreMgr.CurrentScore += Time.deltaTime * 10;
//		UpdateProgress();
	}

	void CreateProgressDots()
	{
		if (dotItemList.Count == 0) {
			for (int i = 0; i <= dotItemLength; ++i) {
				var item = Instantiate (DotItem) as GameObject;
				item.transform.parent = RankProgressBarRoot.transform;
				item.transform.localScale = Vector3.one;
				Vector3 pos = iTween.PointOnPath (ProgressAnchors,i/(float)dotItemLength);
				item.transform.position = pos;

				item.SetActive (false);
				dotItemList.Add (item);
			}
		}
	}

	public override void FadeIn()
	{
//		App.Game.ScoreMgr.maxScore = 100;
//		App.Game.ScoreMgr.CurrentScore = 0;

		if (App.Game.GameStateMgr.ActiveState is BeatState) {
			CreateProgressDots ();
			RankProgressBarRoot.SetActive (true);
			IndicatorItem.SetActive (true);
		}
		else {
			RankProgressBarRoot.SetActive (false);
			IndicatorItem.SetActive (false);
		}

		//App.Game.ScoreMgr.OnRightPickUpdate += ShowPrefact;
		//App.Game.ScoreMgr.OnGreatUpdate += ShowGreat;
		//App.Game.ScoreMgr.OnWrongPickUpdate += ShowMiss;
		//
		App.SoundMgr.Play(AudioResources.shield_boost);
		ShieldScanLine.GetComponent<MeshRenderer> ().enabled = false;

		var tween = ShieldScreen.AddComponent<TweenScale> ();
		tween.from = new Vector3 (ShieldScreen.transform.localScale.x,ShieldScreen.transform.localScale.y,1);
		tween.to = new Vector3 (ShieldScreen.transform.localScale.x,ShieldScreen.transform.localScale.y,ShieldScreen.transform.localScale.z);
		tween.duration = 0.2f;
		tween.PlayForward ();
		tween.AddOnFinished (()=> {
			ShieldScanLine.GetComponent<MeshRenderer> ().enabled = true;
		});
	}

	public override void FadeOut()
	{
		//App.Game.ScoreMgr.OnRightPickUpdate -= ShowPrefact;
		//App.Game.ScoreMgr.OnWrongPickUpdate -= ShowMiss;

		App.SoundMgr.Play(AudioResources.shield_boost);
		var tween = ShieldScreen.AddComponent<TweenScale> ();
		tween.from = new Vector3 (ShieldScreen.transform.localScale.x,ShieldScreen.transform.localScale.y,ShieldScreen.transform.localScale.z);
		tween.to = new Vector3 (ShieldScreen.transform.localScale.x,ShieldScreen.transform.localScale.y,1);
		tween.duration = 0.2f;
		tween.PlayForward ();
		tween.AddOnFinished (()=> {
			DeActive();
		});
	}


	public void ShowMiss(int slideIndex,Vector3 hitWorldPos,float hitPositionGap)
	{
		if (slideIndex > 0)
			return;

		App.SoundMgr.Play (AudioResources.shield_hit);
	}

	void ProcessCloseUI()
	{
		DeActive ();
	}

}
