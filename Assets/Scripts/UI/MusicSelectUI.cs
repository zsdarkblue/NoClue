using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;
using BeatsFever.GameState;

public class MusicSelectUI : FrameBaseUI {
	public static string cityName;

	public GameObject Music0;
	public GameObject Music1;
	public GameObject Music2;

	public GameObject Back;

	Vector3 Music0InitPos;
	Vector3 Music1InitPos;
	Vector3 Music2InitPos;
	Vector3 backInitPos;

	const float fadeInTime = 1f;
	const float fadeDistance = 1f;
	const float transToHighLightTime = 1f;
	private bool resetCityMask = false;
	private bool startBeatsMask = false;
	private int selectLevelId;

	void Awake()
	{
		Music0InitPos = Music0.transform.localPosition;
		Music1InitPos = Music1.transform.localPosition;
		Music2InitPos = Music2.transform.localPosition;
		backInitPos = Back.transform.localPosition;
	}
	void Start () {
	
	}

	void OnEnable()
	{
		Music0.transform.localPosition = Music0InitPos + Vector3.forward * fadeDistance;
		Music0.transform.localScale = Vector3.zero;

		Music1.transform.localPosition = Music1InitPos + Vector3.forward * fadeDistance;
		Music1.transform.localScale = Vector3.zero;

		Music2.transform.localPosition = Music2InitPos + Vector3.forward * fadeDistance;
		Music2.transform.localScale = Vector3.zero;

		Back.transform.localPosition = backInitPos + Vector3.forward * fadeDistance;
		Back.transform.localScale = Vector3.zero;
	}


	void PlayItemFadeIn(GameObject go,Vector3 initPos,float fadeTime = fadeInTime)
	{
		iTween.MoveTo(go, iTween.Hash("position", initPos, "easeType", "linear","time",fadeTime,"islocal",true));
		iTween.ScaleTo (go,Vector3.one,fadeTime);
	}

	void PlayItemFadeOut(GameObject go,float fadeTime = fadeInTime)
	{
		iTween.MoveTo(go, iTween.Hash("position", go.transform.localPosition + Vector3.forward * fadeDistance, "easeType", "linear","time",fadeTime,"islocal",true));
		iTween.ScaleTo (go,Vector3.zero,fadeTime);
	}

	public override void FadeIn()
	{
		var musicList = App.Game.ConfigureMgr.GetStoryConfig ().CityMusicDic [cityName];
		Music0.GetComponent<MusicItem> ().Init (musicList [0].ID);
		Music1.GetComponent<MusicItem> ().Init (musicList [1].ID);
		Music2.GetComponent<MusicItem> ().Init (musicList [2].ID);

		PlayItemFadeIn (Music0,Music0InitPos);
		PlayItemFadeIn (Music1,Music1InitPos);
		PlayItemFadeIn (Music2,Music2InitPos);
		PlayItemFadeIn (Back,backInitPos);
	}

	public override void FadeOut()
	{
		PlayItemFadeOut (Music0);
		PlayItemFadeOut (Music1);
		PlayItemFadeOut (Music2);
		PlayItemFadeOut (Back,0.001f);

		Invoke ("ProcessCloseUI", fadeInTime);
	}

	void ProcessCloseUI()
	{
		if (resetCityMask) {
			App.Game.GUIFrameMgr.GetFrame (GUIFrameID.CitySelectUI).CallWapperFunction ("Reset");
		}
		else if (startBeatsMask) {
			//(App.Game.GameStateMgr.ActiveState as LobbyState).EnterCity (selectLevelId);
		}
		DeActive ();
	}

	void OnMusicSelect(int levelId)
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		var conf = App.Game.ConfigureMgr.GetStoryConfig ().GetMusicConfig (levelId);
		if (!conf.IsOpenToUser) {
			return;
		}
		startBeatsMask = true;
		selectLevelId = levelId;
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.MusicSelectUI);
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.CitySelectUI);

	}

	void OnBackSelect()
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		resetCityMask = true;
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.MusicSelectUI);
	}
}
