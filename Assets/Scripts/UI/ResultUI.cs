using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;
using BeatsFever.UnityEx;
using BeatsFever.GameState;
using System.Collections.Generic;
#if !Steamworks_Off
using Steamworks;
#endif

public class ResultUI : FrameBaseUI {

	public static int currentLevelId = 0;

	public GameObject storyBack;
	public GameObject challengeBack;

	public GameObject storyTextRoot;
	public GameObject challengeTextRoot;

	public UIGrid StoryGrid;
	public GameObject StoryItem;


	public UILabel challengeLevelScore;
	public UILabel challengeComboScore;
	public UILabel challengeGhostScore;

	public UILabel FunctionButtonName;
	public UILabel TotalScore;

	public UILabel Title;

	float fadeInTime = 0.5f;

	#if !Steamworks_Off
	private CallResult<LeaderboardFindResult_t> m_LeaderboardFindResult;
	private SteamLeaderboard_t steamLeaderboard_TopScore;
	protected CallResult<LeaderboardScoreUploaded_t> m_LeaderboardScoreUploaded;  
	#endif

	void Awake()
	{
		
	}

	public override void FadeIn()
	{
		var scoreMgr = App.Game.ScoreMgr;
		Dictionary<int,LevelScoreItem> scoreDic = new Dictionary<int, LevelScoreItem> ();
		var scoreList = App.Game.ScoreMgr.GetScoreList ();

		if (DeviceContainer.Instance.gameMode == GameMode.Story) {
			storyBack.SetActive (true);
			challengeBack.SetActive (false);
			storyTextRoot.SetActive (true);
			challengeTextRoot.SetActive (false);

			foreach (var score in scoreList) {
				scoreDic.Add (score.index, score);
			}

			for (int i = 0; i < ScoreMgr.StoryTotalLevelCount; i++) {
				GameObject leaderItem = Instantiate(StoryItem) as GameObject;
				leaderItem.SetActive (true);
				leaderItem.transform.parent = StoryGrid.transform;
				leaderItem.transform.localScale = Vector3.one;
				leaderItem.transform.localRotation = Quaternion.identity;
				leaderItem.transform.localPosition = Vector3.zero;

				leaderItem.transform.FindChild ("levelIndex").GetComponent < UILabel> ().text = string.Format(MultilanguageMgr.GetMutiText("text_14"), (i + 1).ToString ());

				string multilId = "text_11" + i.ToString ();
				leaderItem.transform.FindChild ("levelName").GetComponent < UILabel> ().text = MultilanguageMgr.GetMutiText (multilId);
				if (scoreDic.ContainsKey (i)) {
					var score = scoreDic [i];
					leaderItem.transform.FindChild ("timeScore").GetComponent < UILabel> ().text = score.timeScore.ToString (); 
					leaderItem.transform.FindChild ("comboScore").GetComponent < UILabel> ().text = score.comboScore.ToString (); 
					leaderItem.transform.FindChild ("ghostScore").GetComponent < UILabel> ().text = score.ghostScore.ToString (); 
					leaderItem.transform.FindChild ("totalScore").GetComponent < UILabel> ().text = score.totalScore.ToString (); 

					var localData = App.Game.LocalDataBase.GetLevelRecordData (i);
					localData.id = i;
					localData.timeScore = score.timeScore;
					localData.comboScore = score.comboScore;
					localData.ghostScore = score.ghostScore;
					localData.SaveLevelRecord ();
						
				} else {
					leaderItem.transform.FindChild ("timeScore").GetComponent < UILabel> ().text = "N/A";
					leaderItem.transform.FindChild ("comboScore").GetComponent < UILabel> ().text = "N/A";
					leaderItem.transform.FindChild ("ghostScore").GetComponent < UILabel> ().text = "N/A";
					leaderItem.transform.FindChild ("totalScore").GetComponent < UILabel> ().text = "N/A";
				}
			}

			if (scoreMgr.IsWin && scoreList.Count < ScoreMgr.StoryTotalLevelCount) {
				FunctionButtonName.text = MultilanguageMgr.GetMutiText ("text_33");
			} else {
				FunctionButtonName.text = MultilanguageMgr.GetMutiText ("text_6");
			}

			if (App.Game.ScoreMgr.IsWin) {
				Title.text = MultilanguageMgr.GetMutiText ("text_31");
			} else {
				Title.text = MultilanguageMgr.GetMutiText ("text_32");
			}
		}
		else if(DeviceContainer.Instance.gameMode == GameMode.Challenge)
		{
			storyBack.SetActive (false);
			challengeBack.SetActive (true);
			storyTextRoot.SetActive (false);
			challengeTextRoot.SetActive (true);

			challengeLevelScore.text = (scoreList.Count * ScoreMgr.ChallengeLevelRewardScore).ToString();

			int totalCombo = 0;
			int totalGhost = 0;
			foreach (var score in scoreList) {
				totalCombo += score.comboScore;
				totalGhost += score.ghostScore;
			}
			challengeComboScore.text = totalCombo.ToString ();
			challengeGhostScore.text = totalGhost.ToString ();

			FunctionButtonName.text = MultilanguageMgr.GetMutiText ("text_6");

			int challengeBestScore = App.Game.ScoreMgr.GetTotalScore (DeviceContainer.Instance.gameMode);
			var oldBest = App.Game.LocalDataBase.GetBestChallengeScore ();
			if (challengeBestScore > oldBest) {
				App.Game.LocalDataBase.SetBestChallengeScore (challengeBestScore);
			}

			Title.text = MultilanguageMgr.GetMutiText ("text_2");
		}

	

		TotalScore.text = App.Game.ScoreMgr.GetTotalScore (DeviceContainer.Instance.gameMode).ToString ();
		SteamAPIMgr.Instance.StorePlayResult (DeviceContainer.Instance.gameMode,App.Game.ScoreMgr.GetScoreList());

		//Invoke ("OnFunctionButtonClick", 5);
	}

	public override void FadeOut()
	{
		Invoke ("ProcessCloseUI", fadeInTime);
	}


	void Start () {

	}

	void OnFunctionButtonClick()
	{
		var scoreMgr = App.Game.ScoreMgr;
		Dictionary<int,LevelScoreItem> scoreDic = new Dictionary<int, LevelScoreItem> ();
		var scoreList = App.Game.ScoreMgr.GetScoreList ();
		if (DeviceContainer.Instance.gameMode == GameMode.Story) {
			if (scoreMgr.IsWin && scoreList.Count < ScoreMgr.StoryTotalLevelCount) {
				App.Game.GUIFrameMgr.DeActive (GUIFrameID.ResultUI,true);
				(App.Game.GameStateMgr.ActiveState as LobbyState).EnterCity (DeviceContainer.Instance.gameMode);
			} else {
				ProcessCloseUI ();
				(App.Game.GameStateMgr.ActiveState as LobbyState).ShowLobbyMenu();
				App.Game.ScoreMgr.ResetAll ();
			}
		}
		else if(DeviceContainer.Instance.gameMode == GameMode.Challenge)
		{
			ProcessCloseUI ();
			(App.Game.GameStateMgr.ActiveState as LobbyState).ShowLobbyMenu();
			App.Game.ScoreMgr.ResetAll ();
		}
	}


	void ProcessCloseUI()
	{
		DeActive ();
	}


	void OnNextSelect()
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		App.Game.GUIFrameMgr.DeActive(GUIFrameID.ResultUI);
	}

	#if !Steamworks_Off
	//初始化排行榜
	private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)  
	{  
		if (!SteamManager.Initialized)  
			return;  

		if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard != 0 && pCallback.m_bLeaderboardFound != 0) {  
			steamLeaderboard_TopScore = pCallback.m_hSteamLeaderboard;  
			Invoke ("UploadScore", 0.1f);
		} else {
			Debug.LogError ("leader board find error:" + pCallback.m_hSteamLeaderboard.m_SteamLeaderboard + "   " + pCallback.m_bLeaderboardFound);
		}
	}  

	void UploadScore()  
	{  
		
		var scoreMgr = App.Game.ScoreMgr;
		int score = (int)scoreMgr.CurrentScore;
		if (steamLeaderboard_TopScore.m_SteamLeaderboard != 0)  
		{  
			Debug.Log ("upload level leader board, my score:"+score);
			var handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_TopScore, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);  
			m_LeaderboardScoreUploaded.Set(handle);  
		}  
	}  
	//上传排行榜数据回调
	private void OnLeaerboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)  
	{  
		if (pCallback.m_bSuccess == 1) { 

			var scoreMgr = App.Game.ScoreMgr;
			int score = (int)scoreMgr.CurrentScore;
			SteamAPIMgr.Instance.StorePlayResult (DeviceContainer.Instance.gameMode,App.Game.ScoreMgr.GetScoreList());
		}
		else {
			Debug.LogError ("OnLeaerboardScoreUploaded Error:"+pCallback.m_bSuccess);		
		}
	}  
	#endif
}
