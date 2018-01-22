using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;

public class MenuUI : FrameBaseUI {

	public GameObject Story;
	public GameObject Adventure;
	public GameObject Collection;
	public GameObject Setting;

	Vector3 storyInitPos;
	Vector3 adventureInitPos;
	Vector3 collectionInitPos;
	Vector3 settingInitPos;


	float fadeInTime = 0.5f;
	float fadeDistance = 10f;

	void Awake()
	{
		storyInitPos = Story.transform.localPosition;
		adventureInitPos = Adventure.transform.localPosition;
		collectionInitPos = Collection.transform.localPosition;
		settingInitPos = Setting.transform.localPosition;

	}
	void Start () {
	
	}

	void PlayItemFadeIn(GameObject go,Vector3 initPos)
	{
		go.transform.localPosition = initPos + Vector3.forward * fadeDistance;
		go.transform.localScale = Vector3.zero;

		iTween.MoveTo(go, iTween.Hash("position", initPos, "easeType", "linear","time",fadeInTime,"islocal",true));
		iTween.ScaleTo (go,Vector3.one,fadeInTime);
	}

	void PlayItemFadeOut(GameObject go)
	{
		iTween.MoveTo(go, iTween.Hash("position", go.transform.localPosition + Vector3.forward * fadeDistance, "easeType", "linear","time",fadeInTime,"islocal",true));
		iTween.ScaleTo (go,Vector3.zero,fadeInTime);
	}

	public override void FadeIn()
	{
		PlayItemFadeIn (Story,storyInitPos);
		PlayItemFadeIn (Adventure,adventureInitPos);
		PlayItemFadeIn (Collection,collectionInitPos);
		PlayItemFadeIn (Setting,settingInitPos);
	}

	public override void FadeOut()
	{
		PlayItemFadeOut (Story);
		PlayItemFadeOut (Adventure);
		PlayItemFadeOut (Collection);
		PlayItemFadeOut (Setting);

		Invoke ("ProcessCloseUI", fadeInTime);
	}

	void ProcessCloseUI()
	{
		DeActive ();
	}

	void OnStorySelect()
	{
		SetNextFrameID (GUIFrameID.CitySelectUI);
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.MenuUI);
	}

	void OnAdventureSelect()
	{

	}

	void OnCollectionSelect()
	{

	}

	void OnSettingSelect()
	{

	}
}
