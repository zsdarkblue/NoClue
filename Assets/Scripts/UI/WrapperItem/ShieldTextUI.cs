using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;
using BeatsFever.GameState;

public class ShieldTextUI : FrameBaseUI {		
	public GameObject UserHitSprite;

	public Transform UIparent;

	public UILabel Combo;
	public UILabel Score;

	public GameObject NewComboObject;
	public UISprite ComboName;
	public UILabel NewComboNum;

	TweenScale comboNameTweenScale;
	TweenScale comboNumTweenScale;

	public TweenScale[] ComboTweenScales;

	LinkedList<GameObject> idleQueue = new LinkedList<GameObject> ();
	LinkedList<GameObject> activeQueue = new LinkedList<GameObject> ();

	private Transform shieldTransform;

	public override void FadeIn()
	{

		Combo.text = "";
		Score.text = "";

		//App.Game.ScoreMgr.OnRightPickUpdate += ShowPrefact;
	//	App.Game.ScoreMgr.OnGreatUpdate += ShowGreat;
	//	App.Game.ScoreMgr.OnWrongPickUpdate += ShowMiss;
	}

	public override void FadeOut()
	{
	//	App.Game.ScoreMgr.OnRightPickUpdate -= ShowPrefact;
	//	App.Game.ScoreMgr.OnGreatUpdate -= ShowGreat;
	//	App.Game.ScoreMgr.OnWrongPickUpdate -= ShowMiss;
		DeActive ();
	}

	void Update()
	{
		transform.position = DeviceContainer.Instance.ShieldRoot.transform.position;
	}

	// Use this for initialization
	void Awake()
	{
		activeQueue.Clear ();
		for (int i = 0; i < 60; ++i) {
			var spriteObject = Instantiate (UserHitSprite,UIparent) as GameObject;
			//DontDestroyOnLoad (spriteObject);
			idleQueue.AddLast (spriteObject);
		}

		comboNameTweenScale = ComboName.GetComponent<TweenScale> ();
		comboNumTweenScale = NewComboNum.GetComponent<TweenScale> ();

		shieldTransform = DeviceContainer.Instance.ShieldRoot.transform;
	}

	GameObject GetItemFromPool()
	{
		GameObject go;
		if (idleQueue.Count > 0) {
			go = idleQueue.Last.Value;
			idleQueue.RemoveLast ();
		}
		else {
			go = activeQueue.Last.Value;
			activeQueue.RemoveLast ();
		}

		activeQueue.AddFirst (go);
		go.SetActive (true);

		Invoke ("OnSpriteShowOver", 1);
		return go;
	}

	void OnSpriteShowOver()
	{
		if (activeQueue.Count > 0) {
			var go = activeQueue.Last.Value;
			go.SetActive (false);
			activeQueue.RemoveLast ();
			idleQueue.AddLast (go);
		}
	}

	public void ShowPrefact(int slideIndex,Vector3 hitWorldPos,float hitPositionGap = 0)
	{
		if (slideIndex > 0)
			return;
		
		var go = GetItemFromPool ();
		go.GetComponent<UISprite> ().spriteName = "perfect";
		UpdateTransformByHitPos (go, hitWorldPos,true);
		if (App.Game.ScoreMgr.CurrentScore > 0) {
			UpdateComboEffectAndScoreText (App.Game.ScoreMgr.Combo,App.Game.ScoreMgr.CurrentScore);
			ShowComboText (hitWorldPos, App.Game.ScoreMgr.Combo);
		}
	}

	public void ShowGreat(int slideIndex,Vector3 hitWorldPos,float hitPositionGap = 0)
	{
		if (slideIndex > 0)
			return;
		
		var go = GetItemFromPool ();
		go.GetComponent<UISprite> ().spriteName = "good";
		UpdateTransformByHitPos (go, hitWorldPos);
		if (App.Game.ScoreMgr.CurrentScore > 0) {
			UpdateComboEffectAndScoreText (App.Game.ScoreMgr.Combo,App.Game.ScoreMgr.CurrentScore);
			ShowComboText (hitWorldPos, App.Game.ScoreMgr.Combo);
		}
	}

	public void ShowMiss(int slideIndex,Vector3 hitWorldPos,float hitPositionGap = 0)
	{
		if (slideIndex > 0)
			return;
		
		var go = GetItemFromPool ();
		go.GetComponent<UISprite> ().spriteName = "miss";
		UpdateTransformByHitPos (go, hitWorldPos);
		if (App.Game.ScoreMgr.CurrentScore > 0) {
			UpdateComboEffectAndScoreText (App.Game.ScoreMgr.Combo,App.Game.ScoreMgr.CurrentScore);
			ShowComboText (hitWorldPos, App.Game.ScoreMgr.Combo);
		}
		NewComboObject.SetActive (false);
	}
		
	void ShowComboText(Vector3 pos,int combo)
	{
		UpdateTransformByHitPos (NewComboObject, pos + Vector3.up * 0.05f);

		NewComboObject.SetActive (true);
		NewComboNum.text = combo.ToString();

		comboNameTweenScale.ResetToBeginning ();
		comboNameTweenScale.PlayForward ();

		comboNumTweenScale.ResetToBeginning ();
		comboNumTweenScale.PlayForward ();

		CancelInvoke ("HideCombo");
		Invoke ("HideCombo", 1);
	}

	void HideCombo()
	{
		NewComboObject.SetActive (false);
	}

	void UpdateTransformByHitPos(GameObject uiGo,Vector3 uiPos,bool shake = false)
	{
		uiGo.SetActive (true);

		var center = DeviceContainer.Instance.GetShieldRootCenterPosition ();
		var radius = DeviceContainer.Instance.GetShieldRadius ();
		var dir = uiPos -center;
		Ray ray = new Ray (center, dir);
		uiGo.transform.position = ray.GetPoint (radius);
		uiGo.transform.LookAt ( ray.GetPoint(radius + 1));

		if (shake) {
			TweenScale tween = uiGo.GetComponent<TweenScale> ();
			tween.ResetToBeginning ();
			tween.PlayForward ();
		}
	}
		
	public void UpdateComboEffectAndScoreText(int combo,float score)
	{
		Combo.text = combo.ToString ();
		if (combo > 0) {
			for (int i = 0; i < ComboTweenScales.Length; ++i) {
				var tween = ComboTweenScales [i];
				tween.ResetToBeginning ();
				tween.PlayForward ();
			}
		}
		else {
			if (Combo.transform.parent.gameObject.activeSelf) {
				Combo.transform.parent.gameObject.SetActive (false);
			}
		}

		Score.text = ((int)score).ToString ("###,###");
	}
}
