using UnityEngine;
using System.Collections;
using BeatsFever;
using System.Collections.Generic;
#if !Steamworks_Off
using Steamworks;
#endif
public class RotateMusicItem : MonoBehaviour {

	public UITexture uiTexture;
	//public GameObject CoverRoot;

	public GameObject MoveRoot;
	public AudioSource source;
	public TweenAlpha SelectedTweenAlpa;
	public UITexture SelectTexture;
	public BoxCollider collider;
	public UISprite rankIcon;

	private int level;

	#if !Steamworks_Off
	private SteamLeaderboard_t steamLeaderboard_TopTotalScore;
	protected CallResult<LeaderboardFindResult_t> m_LeaderboardFindResult;  
	protected CallResult<LeaderboardScoreUploaded_t> m_LeaderboardScoreUploaded;  
	protected CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded;  
	#endif
	public List<LeaderBoardItem> leaderBoards = new List<LeaderBoardItem>();
	bool activeLeaderBoard = false;


	// Use this for initialization
	void OnEnable () {
		
	}

	public void HightSelectedLevel(int levelId)
	{
//		if (levelId == level) {
//			iTween.MoveTo(CoverRoot, iTween.Hash("position", new Vector3(0,0.03f,-0.02f), "easeType", "linear","time",0.1f,"islocal",true));
//			iTween.RotateTo(CoverRoot, iTween.Hash("x", -20f, "easeType", "linear","time",0.1f,"islocal",true));
//			SetPreviewSongActive (true);
//		}
//		else {
//			iTween.MoveTo(CoverRoot, iTween.Hash("position", Vector3.zero, "easeType", "linear","time",0.1f,"islocal",true));
//			iTween.RotateTo(CoverRoot, iTween.Hash("x", 0, "easeType", "linear","time",0.1f,"islocal",true));
//			SetPreviewSongActive (false);
//		}
	}

	void Update()
	{
		if (source.volume <= 1) {
			source.volume += Time.deltaTime;
		}
	}

	void SetPreviewSongActive(bool active)
	{
		if (active && source.clip == null) {
			var conf = App.Game.ConfigureMgr.GetStoryConfig ().GetMusicConfig (level);
			source.clip = Resources.Load ("Audio/"+conf.MusicRes + "_preview") as AudioClip;
		}

		if (active) {
			source.time = 0;
			source.volume = 0;
			source.Play ();

			SelectedTweenAlpa.from = SelectTexture.alpha;
			SelectedTweenAlpa.to = 1;
			SelectedTweenAlpa.ResetToBeginning ();
			SelectedTweenAlpa.PlayForward ();
		}
		else {
			source.volume = 0;
			source.Stop ();

			SelectedTweenAlpa.from = SelectTexture.alpha;
			SelectedTweenAlpa.to = 0;
			SelectedTweenAlpa.ResetToBeginning ();
			SelectedTweenAlpa.PlayForward ();
		}
	}

	public void Init(int levelId,int coverGroupIndex)
	{
		level = levelId;
		var conf = App.Game.ConfigureMgr.GetStoryConfig ().GetMusicConfig (levelId);
		//uiTexture.mainTexture = App.ResourceMgr.LoadRes ("cover"+levelId.ToString()) as Texture2D;
		uiTexture.mainTexture = App.ResourceMgr.LoadRes ("cover"+conf.CoverTexIndex.ToString()) as Texture2D;

		if (!SteamAPIMgr.DisableSteamWorks) {
//			int levelScore = SteamAPIMgr.Instance.GetStat (StatsName.BestRecordScoreLevel + (levelId).ToString ()).value;
//			if (levelScore > 0) {
//				rankIcon.enabled = true;
//				//int rank = App.Game.ScoreMgr.GetRankByScoreAndMaxScore (levelScore, conf.MaxScore);
//				//rankIcon.spriteName = GameUtil.GetRankSpriteName (rank);
//			} else {
//				rankIcon.enabled = false;
//			}
		} else {
			var data = App.Game.LocalDataBase.GetLevelRecordData (conf.ID);
//			if (data.highestScore > 0) {
//				rankIcon.enabled = true;
//				rankIcon.spriteName = GameUtil.GetRankSpriteName (data.highestRank);
//			} else {
//				rankIcon.enabled = false;
//			}
		}

		MoveRoot.transform.localPosition = new Vector3 (0, -0.05f,0);
		float distanceFromCenter = Mathf.Abs ((level % 10) - 4.5f);

		bool active = ((level / 10) == coverGroupIndex);
		if (active) {
			collider.enabled = true;
			Invoke ("DoTweenUp", 0.2f + distanceFromCenter * 0.2f);
		}
		else {
			collider.enabled = false;
		}

		#if !Steamworks_Off
		if (activeLeaderBoard) {
			if (leaderBoards.Count == 0) {
				m_LeaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult); 
				m_LeaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);

				//SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(LeaderBoardName.TopScoreLevel + level.ToString(), ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);  
				//m_LeaderboardFindResult.Set(handle);  
			}
		}
		#endif
	}

	void DoTweenUp()
	{
//		App.SoundMgr.Play (AudioResources.card);
//		iTween.MoveTo(MoveRoot, iTween.Hash("position", Vector3.zero, "easeType", "linear","time",0.5f,"islocal",true));
//		//CoverRoot.SetActive (true);
//		uiTexture.enabled = true;
//		var tweenAlpha = CoverRoot.GetComponent<TweenAlpha> ();
//		tweenAlpha.onFinished.Clear ();
//		tweenAlpha.PlayForward();
	}

	void DoTweenDown()
	{
//		if (uiTexture.enabled) {
//			iTween.MoveTo(MoveRoot, iTween.Hash("position", new Vector3 (0, -0.05f,0), "easeType", "linear","time",0.5f,"islocal",true));
//			var tweenAlpha = CoverRoot.GetComponent<TweenAlpha> ();
//			tweenAlpha.AddOnFinished (()=>{
//				uiTexture.enabled = false;
//			});
//			tweenAlpha.PlayReverse();
//			//CoverRoot.GetComponent<TweenAlpha>().PlayReverse();
//		}

	}
	void SwitchCovers(int coverIndex)
	{
		float distanceFromCenter = Mathf.Abs ((level % 10) - 4.5f);

		if (coverIndex >= 0) {
			bool active = ((level / 10) == coverIndex);
			if (active) {
				collider.enabled = true;
				Invoke ("DoTweenUp",0.5f + distanceFromCenter * 0.2f);
			}
			else {
				collider.enabled = false;
				Invoke ("DoTweenDown",distanceFromCenter * 0.2f);
			}
		} else {
			collider.enabled = false;
			Invoke ("DoTweenDown",distanceFromCenter * 0.2f);
		}
	}

	#if !Steamworks_Off
	//排行榜
	//请求下载最新排行榜数据 回调
	private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)  
	{  
		//判断是不是同一个排行榜
		if (pCallback.m_hSteamLeaderboard == steamLeaderboard_TopTotalScore)  
		{
			if (pCallback.m_cEntryCount > 0)  
			{
				for (int i = 0; i < pCallback.m_cEntryCount; i++)  
				{
					LeaderboardEntry_t leaderboardEntry;  
					int[] details = new int[pCallback.m_cEntryCount];  
					//获取下载好的排行榜信息
					SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, i, out leaderboardEntry, details, pCallback.m_cEntryCount);  
					leaderBoards.Add (new LeaderBoardItem (leaderboardEntry.m_nGlobalRank,SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser),leaderboardEntry.m_nScore));
				}
			}  
			else  
			{  
				Debug.LogError("排行榜数据为空！");  
			}  
		}  
	}  



	//初始化排行榜
	private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)  
	{  
		if (!SteamManager.Initialized)  
			return;  

		if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard != 0 && pCallback.m_bLeaderboardFound != 0)  
		{  
			steamLeaderboard_TopTotalScore = pCallback.m_hSteamLeaderboard;  
			DownloadLeaderboardEntries();  
		}
	}  
		

	void DownloadLeaderboardEntries()  
	{  
		if (steamLeaderboard_TopTotalScore.m_SteamLeaderboard != 0)  
		{  
			var handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_TopTotalScore, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 10);  
			m_LeaderboardScoresDownloaded.Set(handle);  
		}  
	}  
	#endif
}
