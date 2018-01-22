using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;
using BeatsFever.UI;

public class ProfileUI : FrameBaseUI {
	public UIGrid StoryGrid;
	public GameObject StoryItem;

	public UILabel TotalScoreStory;
	public UILabel TotalScoreChallenge;

	public override void FadeIn()
	{
		var scoreMgr = App.Game.ScoreMgr;
		Dictionary<int,LevelScoreItem> scoreDic = new Dictionary<int, LevelScoreItem> ();
		var scoreList = App.Game.ScoreMgr.GetScoreList ();

		foreach (var score in scoreList) {
			scoreDic.Add (score.index, score);
		}

		int storyTotalScore = 0;
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

			#if !Steamworks_Off
			int timeScore = SteamAPIMgr.Instance.GetStat (StatsName.StoryTimeScorePrefix_Level + i.ToString()).value;
			int comboScore = SteamAPIMgr.Instance.GetStat (StatsName.StoryComboScorePrefix_Level + i.ToString()).value;
			int ghostScore = SteamAPIMgr.Instance.GetStat (StatsName.StoryGhostScorePrefix_Level + i.ToString()).value;
			#else
			var localData = App.Game.LocalDataBase.GetLevelRecordData (i);
			int timeScore = localData.timeScore;
			int comboScore = localData.comboScore;
			int ghostScore = localData.ghostScore;
			#endif


			if (ghostScore > 0) {
				leaderItem.transform.FindChild ("timeScore").GetComponent < UILabel> ().text = timeScore.ToString (); 
				leaderItem.transform.FindChild ("comboScore").GetComponent < UILabel> ().text = comboScore.ToString (); 
				leaderItem.transform.FindChild ("ghostScore").GetComponent < UILabel> ().text = ghostScore.ToString (); 
				int total = timeScore + comboScore + ghostScore;
				storyTotalScore += total;
				leaderItem.transform.FindChild ("totalScore").GetComponent < UILabel> ().text = total.ToString ();
			} else {
				leaderItem.transform.FindChild ("timeScore").GetComponent < UILabel> ().text = "N/A";
				leaderItem.transform.FindChild ("comboScore").GetComponent < UILabel> ().text = "N/A";
				leaderItem.transform.FindChild ("ghostScore").GetComponent < UILabel> ().text = "N/A";
				leaderItem.transform.FindChild ("totalScore").GetComponent < UILabel> ().text = "N/A";
			}
		}

		#if !Steamworks_Off
		TotalScoreStory.text = SteamAPIMgr.Instance.GetStat (StatsName.BestStoryScore).value.ToString();
		TotalScoreChallenge.text = SteamAPIMgr.Instance.GetStat (StatsName.BestChallengeScore).value.ToString();
		#else
		TotalScoreStory.text = storyTotalScore.ToString();
		TotalScoreChallenge.text = App.Game.LocalDataBase.GetBestChallengeScore().ToString();
		#endif

	}

	public override void FadeOut()
	{
		ProcessCloseUI ();
	}

	void CloseUI()
	{
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.ProfileUI);
	}

	void ProcessCloseUI()
	{
		App.Game.GUIFrameMgr.Active (GUIFrameID.RotateMusicSelectUI);
		DeActive ();
	}
}
