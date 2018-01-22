using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;

public class CitySelectUI : FrameBaseUI {
	public static int SelectedId;

	public GameObject Tokyo;
	public GameObject London;
	public GameObject Rio;

	public GameObject Back;

	public Transform HighLightPosMask;

	Vector3 tokyoInitPos;
	Vector3 londonInitPos;
	Vector3 rioInitPos;
	Vector3 backInitPos;

	const float fadeInTime = 1f;
	const float fadeDistance = 1f;
	const float transToHighLightTime = 1f;

	void Awake()
	{
		tokyoInitPos = Tokyo.transform.localPosition;
		londonInitPos = London.transform.localPosition;
		rioInitPos = Rio.transform.localPosition;
		backInitPos = Back.transform.localPosition;
	}
	void Start () {
	
	}

	void OnEnable()
	{
		Tokyo.transform.localPosition = tokyoInitPos + Vector3.forward * fadeDistance;
		Tokyo.transform.localScale = Vector3.zero;

		London.transform.localPosition = londonInitPos + Vector3.forward * fadeDistance;
		London.transform.localScale = Vector3.zero;

		Rio.transform.localPosition = rioInitPos + Vector3.forward * fadeDistance;
		Rio.transform.localScale = Vector3.zero;

		Back.transform.localPosition = backInitPos + Vector3.forward * fadeDistance;
		Back.transform.localScale = Vector3.zero;

	}

	void PlayItemFadeIn(GameObject go,Vector3 initPos,float fadeTime = fadeInTime,bool ignoreScale = false)
	{
		iTween.MoveTo(go, iTween.Hash("position", initPos, "easeType", "linear","time",fadeTime,"islocal",true));
		iTween.ScaleTo (go,Vector3.one,fadeTime);	
	}

	void PlayItemFadeOut(GameObject go,float fadeTime = fadeInTime)
	{
		iTween.MoveTo(go, iTween.Hash("position", go.transform.localPosition + Vector3.forward * fadeDistance, "easeType", "linear","time",fadeTime,"islocal",true));
		iTween.ScaleTo (go,Vector3.zero,fadeTime);
	}

	public void Reset()
	{
		PlayItemFadeIn (Tokyo,tokyoInitPos,fadeInTime);
		PlayItemFadeIn (London,londonInitPos,fadeInTime);
		PlayItemFadeIn (Rio, rioInitPos, fadeInTime);
		PlayItemFadeIn (Back,backInitPos,fadeInTime);
	}

	public override void FadeIn()
	{
		PlayItemFadeIn (Tokyo,tokyoInitPos);
		PlayItemFadeIn (London,londonInitPos);
		PlayItemFadeIn (Rio,rioInitPos);
		PlayItemFadeIn (Back,backInitPos);
	}

	public override void FadeOut()
	{
		PlayItemFadeOut (Tokyo);
		PlayItemFadeOut (London);
		PlayItemFadeOut (Rio);

		Invoke ("ProcessCloseUI", fadeInTime);
	}

	void ProcessCloseUI()
	{
		DeActive ();
	}

	void TransCityToHighLightPos(GameObject go)
	{
		iTween.MoveTo(go, iTween.Hash("position", HighLightPosMask.localPosition, "easeType", "linear","time",transToHighLightTime,"islocal",true,
			"oncompletetarget",gameObject,"oncomplete","OnTransCityToHighLightPosFinish"));
	}

	public void OnTransCityToHighLightPosFinish()
	{
		App.Game.GUIFrameMgr.Active (GUIFrameID.MusicSelectUI);
	}

	void OnTokyoSelect()
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		MusicSelectUI.cityName = "tokyo";
		TransCityToHighLightPos (Tokyo);
		PlayItemFadeOut (London);
		PlayItemFadeOut (Rio);
		PlayItemFadeOut (Back,0.001f);
	}

	void OnLondonSelect()
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		MusicSelectUI.cityName = "london";
		TransCityToHighLightPos (London);
		PlayItemFadeOut (Tokyo);
		PlayItemFadeOut (Rio);
		PlayItemFadeOut (Back,0.001f);
	}

	void OnRioSelect()
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		MusicSelectUI.cityName = "rio";
		TransCityToHighLightPos (Rio);
		PlayItemFadeOut (London);
		PlayItemFadeOut (Tokyo);
		PlayItemFadeOut (Back,0.001f);
	}

	void OnBackSelect()
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		PlayItemFadeOut (Back,0.001f);
		SetNextFrameID (GUIFrameID.MenuUI);
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.CitySelectUI);
	}
}
