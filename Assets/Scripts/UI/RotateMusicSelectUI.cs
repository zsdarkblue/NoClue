using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;
using BeatsFever.GameState;
using System.Collections.Generic;

public class RotateMusicSelectUI : FrameBaseUI {
	static bool isFirstOpen = true;

	float fadeInTime = 0.5f;
	float fadeDistance = 10f;

	public GameObject MusicItem;
	public GameObject RotateRoot;
	public GameObject RotateRoot2;

	public UITexture backGround;
	public UITexture branding;
	public UITexture cover;
	public UITexture text;
	public UITexture startBackGround;
	public GameObject startButton;
	public GameObject ScreenRoot;

	private int pageTotalItemCount = 10;
	public GameObject GridRoot;
	public GameObject LeaderItem;
	private List<GameObject> gridItems = new List<GameObject>();
	private Dictionary<int,RotateMusicItem> scriptDic = new Dictionary<int, RotateMusicItem> ();

	public UITexture userLevelHighestRank;
	public UILabel userLevelHighestScore;
	public GameObject ControlObjects;
	public GameObject AchievementButtonObjects;
	public GameObject LeaderBoardButtonObjects;

	public UILabel MusicName;
	public UILabel AuthorName;
	public UILabel DifficultName;
	public UISprite[] DifficultStars;

	public Renderer BigScreenMaterialRenderer;
	public GameObject BigScreenObject;

	public GameObject UserInfoRoot;

	public Texture2D[] CityTexture2DArray;

	private static int coverGroupIndex = 0;
	private int coverGroupLength = 2;

	Vector3 initScreenPos;

	GameObject teachHammer;

	int CurrentSelectedLevelId = -1;
	bool notEnterMusic = false;
	private bool isFirstEnter = true;

	private GameMode CurrentGameMode;

	bool isFreezPageRefresh = true;
	bool isSelectedMode = false;
	void Awake()
	{
		initScreenPos = ScreenRoot.transform.transform.localPosition;

		#if Steamworks_Off
		AchievementButtonObjects.SetActive(false);
		LeaderBoardButtonObjects.SetActive(false);
		#endif
	}
	void Start () {
		backGround.alpha = 0;
		cover.alpha = 0;
		text.alpha = 0;
		startBackGround.alpha = 0;

		startButton.SetActive (false);
		//text.GetComponent<BoxCollider> ().enabled = true;

		MusicName.alpha = 0;
		AuthorName.alpha = 0;
		DifficultName.alpha = 0;

		for (int i = 0; i < DifficultStars.Length; i++) {
			DifficultStars [i].alpha = 0;
		}

		isFirstOpen = false;

		//Invoke ("OnChallengeHit", 1f);
	}

	private IEnumerator CreateMusicItems()
	{
		int index = 0;
		RotateMusicItem firstItem = null;
		foreach (var levelId in App.Game.ConfigureMgr.GetStoryConfig ().MusicDataDic.Keys) {
			var conf = App.Game.ConfigureMgr.GetStoryConfig ().GetMusicConfig (levelId);
			if (!conf.IsOpenToUser) {
				continue;
			}

			GameObject musicItem = Instantiate(MusicItem) as GameObject;
			musicItem.SetActive (true);

			musicItem.transform.parent = RotateRoot.transform;
			musicItem.transform.localScale = Vector3.one * 1.5f;
			musicItem.transform.localRotation = Quaternion.identity;
			musicItem.transform.localPosition = Vector3.zero;

			int targetIndex = index % 10;
			targetIndex = targetIndex < 5 ? targetIndex : (targetIndex + 1);
			musicItem.transform.Rotate (new Vector3 (0, -80 + targetIndex * 16, 0), Space.Self);

			musicItem.transform.localPosition += musicItem.transform.forward * 1f;

			musicItem.GetComponentInChildren<QuickMessage> ().param = levelId;

			var script = musicItem.GetComponent<RotateMusicItem> ();
			script.Init (levelId,coverGroupIndex);
			if (levelId == 0) {
				firstItem = musicItem.GetComponent<RotateMusicItem> ();
			}

			if (isFirstOpen && index == 3) {
				teachHammer = UnityEngine.Object.Instantiate (Resources.Load ("harrmerReal")) as GameObject;
				teachHammer.transform.localScale = Vector3.one * 3f;
				teachHammer.transform.position = musicItem.transform.TransformPoint(0.1f,0.1f,0f);
				teachHammer.SetActive (false);

				Invoke ("ShowHammer", 2);
			}

			scriptDic.Add (levelId, script);

			index++;
			yield return null;
		}

		coverGroupLength = (index + 1) / 10;
	}

		
	void ShowHammer()
	{
		if (teachHammer != null) {
			teachHammer.SetActive (true);
		}
	}
	void OnEnable()
	{
		
	}

	void PlayItemFadeIn(GameObject go,Vector3 initPos)
	{
		go.transform.localPosition = initPos + Vector3.forward * fadeDistance;
		go.transform.localScale = Vector3.zero;

		iTween.MoveTo(go, iTween.Hash("position", initPos, "easeType", "linear","time",fadeInTime,"islocal",true));
	}

	void PlayItemFadeOut(GameObject go)
	{
		iTween.MoveTo(go, iTween.Hash("position", go.transform.localPosition + Vector3.forward * fadeDistance, "easeType", "linear","time",fadeInTime,"islocal",true));
	}

	public override void FadeIn()
	{
		FreezPage ();
		notEnterMusic = false;
		CurrentGameMode = GameMode.Null;
		for (int i = 0; i < pageTotalItemCount; i++) {
			GameObject leaderItem = Instantiate(LeaderItem) as GameObject;
			leaderItem.SetActive (false);
			leaderItem.transform.parent = GridRoot.transform;
			leaderItem.transform.localScale = Vector3.one;
			leaderItem.transform.localRotation = Quaternion.identity;
			leaderItem.transform.localPosition = Vector3.zero;

			leaderItem.transform.localPosition = new Vector3 (0, i * (-99), 0);

			gridItems.Add (leaderItem);
		}

		isSelectedMode = false;
	}

	public override void FadeOut()
	{
		if (teachHammer != null) {
			GameObject.Destroy (teachHammer);
			teachHammer = null;
		}

		if (!notEnterMusic) {
			Invoke ("ProcessCloseUI", fadeInTime);
		}
		else {
			ProcessCloseUI ();
		}
	}

	void ProcessCloseUI()
	{
		if (!notEnterMusic) {
			(App.Game.GameStateMgr.ActiveState as LobbyState).EnterCity (CurrentGameMode);
		}

		DeActive ();
	}
		

	void OnMusicStart()
	{
		if (CurrentGameMode == GameMode.Null)
			return;
		
		App.SoundMgr.Play (AudioResources.menuUIHit);

		branding.alpha = 1;
		TweenAlpha tweenAlpha = branding.gameObject.AddComponent<TweenAlpha> ();
		tweenAlpha.from = 1;
		tweenAlpha.to = 0.5f;
		tweenAlpha.duration = 0.5f;
		tweenAlpha.AddOnFinished (()=>{
			App.Game.GUIFrameMgr.DeActive (GUIFrameID.RotateMusicSelectUI);
		});
		tweenAlpha.PlayForward ();
	}

	private void UnFreez()
	{
		isFreezPageRefresh = false;
	}

	void FreezPage()
	{
		isFreezPageRefresh = true;
		Invoke ("UnFreez", 0.3f);
	}

	void ResumeMusicSelected()
	{
		FreezPage ();
		branding.alpha = 0;
		branding.gameObject.SetActive (true);
		TweenAlpha tweenBrand = branding.GetComponent<TweenAlpha> ();
		tweenBrand.ResetToBeginning ();
		tweenBrand.PlayForward ();

		ScreenRoot.SetActive (false);
		ControlObjects.SetActive (true);

		TweenPosition tween = ControlObjects.GetComponent<TweenPosition> ();
		tween.ResetToBeginning ();
		tween.PlayForward ();
	}

	void SetControlObjectsActive(bool active)
	{
		if (!active) {
			CurrentSelectedLevelId = -1;
			//BroadcastMessage ("HightSelectedLevel", -1);
		}
		FreezPage ();
		ScreenRoot.SetActive (active);
		ControlObjects.SetActive (active);
	}

	void OnProfileHit()
	{
		if (isFreezPageRefresh)
			return;

		if (teachHammer != null) {
			GameObject.Destroy (teachHammer);
			teachHammer = null;
		}

		if (!App.Game.GUIFrameMgr.IsActive (GUIFrameID.ProfileUI)) {
			notEnterMusic = true;
			App.SoundMgr.Play (AudioResources.menuUIHit);
			App.Game.GUIFrameMgr.Active (GUIFrameID.ProfileUI);

			//SetControlObjectsActive (false);
			//branding.gameObject.SetActive (false);

			App.Game.GUIFrameMgr.DeActive (GUIFrameID.RotateMusicSelectUI);
		}
	}

	void OnLeaderBoardHit()
	{
		#if Steamworks_Off
		return;
		#endif

		if (isFreezPageRefresh)
			return;

		if (teachHammer != null) {
			GameObject.Destroy (teachHammer);
			teachHammer = null;
		}

		if (!App.Game.GUIFrameMgr.IsActive (GUIFrameID.LeaderBoardUI)) {
			notEnterMusic = true;
			App.SoundMgr.Play (AudioResources.menuUIHit);
			App.Game.GUIFrameMgr.Active (GUIFrameID.LeaderBoardUI);

			SetControlObjectsActive (false);
			branding.gameObject.SetActive (false);

			App.Game.GUIFrameMgr.DeActive (GUIFrameID.RotateMusicSelectUI);
		}
	}

	void OnAchievementHit()
	{
		#if Steamworks_Off
		return;
		#endif

		if (isFreezPageRefresh)
			return;

		if (teachHammer != null) {
			GameObject.Destroy (teachHammer);
			teachHammer = null;
		}

		if (!App.Game.GUIFrameMgr.IsActive (GUIFrameID.AchievementUI)) {
			notEnterMusic = true;
			App.SoundMgr.Play (AudioResources.menuUIHit);
			App.Game.GUIFrameMgr.Active (GUIFrameID.AchievementUI);

			SetControlObjectsActive (false);
			branding.gameObject.SetActive (false);

			App.Game.GUIFrameMgr.DeActive (GUIFrameID.RotateMusicSelectUI);
		}
	}

	void OnStoryHit()
	{
		if (isSelectedMode)
			return;
		
		isSelectedMode = true;
		CurrentGameMode = GameMode.Story;
		DeviceContainer.Instance.gameMode = CurrentGameMode;
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.RotateMusicSelectUI);

		DeviceContainer.Instance.CurrentLeftHintCount = 5;
	}

	void OnChallengeHit()
	{
		if (isSelectedMode)
			return;

		LobbyState.ArtIndex = 0;
		isSelectedMode = true;

		CurrentGameMode = GameMode.Challenge;
		DeviceContainer.Instance.gameMode = CurrentGameMode;
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.RotateMusicSelectUI);

		DeviceContainer.Instance.CurrentLeftHintCount = 5;
	}

	void OnChallengeVikingHit()
	{
		if (isSelectedMode)
			return;

		LobbyState.ArtIndex = 1;
		isSelectedMode = true;

		CurrentGameMode = GameMode.Challenge;
		DeviceContainer.Instance.gameMode = CurrentGameMode;
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.RotateMusicSelectUI);

		DeviceContainer.Instance.CurrentLeftHintCount = 5;
	}


	public void RefreshLeaderBoard(int levelId)
	{
		#if Steamworks_Off
		return;
		#endif
		var script = scriptDic [levelId];
		for (int i = 0; i < pageTotalItemCount; ++i) {
			GameObject item = gridItems [i];

			if (i == 0) {
				item.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (1,0.109f,0.65f);
			}
			else if(i == 1)
			{
				item.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (1,0.0667f,0.243f);
			}
			else if(i == 2)
			{
				item.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.925f,0.514f,0.074f);
			}
			else
			{
				if (i % 2 == 0) {
					item.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.223f,0.196f,0.267f);
				}
				else {
					item.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.122f,0.102f,0.157f);
				}
			}

			if (i >= script.leaderBoards.Count) {
				item.SetActive (true);
				continue;
			}
			else {
				item.SetActive (true);
			}

			var leaderItem = script.leaderBoards[i];

			item.transform.FindChild ("name").GetComponent<UILabel> ().text = leaderItem.name;
			item.transform.FindChild ("rank").GetComponent<UILabel> ().text = leaderItem.rank.ToString();
			item.transform.FindChild ("score").GetComponent<UILabel> ().text = leaderItem.score.ToString("###,###");
		}
	}
}
