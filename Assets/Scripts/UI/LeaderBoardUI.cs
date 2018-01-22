using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;
using BeatsFever.UI;
using System;
#if !Steamworks_Off
using Steamworks;
#endif

public class LeaderBoardItem
{
	public LeaderBoardItem(int _rank,string _name,int _score)
	{
		rank = _rank;
		name = _name;
		score = _score;
	}
	public int rank;
	public string name;
	public int score;
}


public class LeaderBoardUI : FrameBaseUI {

	public UILabel userRank;
	public UILabel userName;
	public UILabel userScore;

	public UILabel userRankChallenge;
	public UILabel userNameChallenge;
	public UILabel userScoreChallenge;

	public GameObject GridRoot;
	public GameObject LeaderItem;

	public GameObject GridRootChallenge;

	public GameObject PageRoot;
	public GameObject LoadingObject;
	public GameObject LoadingObjectChallenge;

	public GameObject[] PageIndactors;

	private List<GameObject> gridItemsStory = new List<GameObject>();
	private List<GameObject> gridItemsChallenge = new List<GameObject>();
	private int pageTotalItemCount = 8;
	private int currentPageIndex = 0;
	private int totalPageCount = 0;

	int maxItemCount = 0;

	bool isFreezPageRefresh = true;

	public static void ClearCachedData()
	{
		leaderBoardsStory.Clear ();
		leaderBoardsChallenge.Clear ();
	}
	static List<LeaderBoardItem> leaderBoardsStory = new List<LeaderBoardItem>();
	static List<LeaderBoardItem> leaderBoardsChallenge = new List<LeaderBoardItem>();
	static string cachedUserRankStory;
	static string cachedUserNameStory;
	static string cachedUserScoreStory;

	static string cachedUserRankChallenge;
	static string cachedUserNameChallenge;
	static string cachedUserScoreChallenge;
	#if !Steamworks_Off

	private SteamLeaderboard_t steamLeaderboard_StoryScore;
	protected CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_Story;  
	protected CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_User_Story;  

	private SteamLeaderboard_t steamLeaderboard_ChallengeScore;
	protected CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_Challenge;  
	protected CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_User_Challenge;

	public override void FadeIn()
	{
		maxItemCount = 0;
		for (int i = 0; i < pageTotalItemCount; i++) {
			GameObject leaderItem = Instantiate(LeaderItem) as GameObject;
			leaderItem.SetActive (false);
			leaderItem.transform.parent = GridRoot.transform;
			leaderItem.transform.localScale = Vector3.one;
			leaderItem.transform.localRotation = Quaternion.identity;
			leaderItem.transform.localPosition = Vector3.zero;

			leaderItem.transform.localPosition = new Vector3 (0, i * (-99), 0);

			gridItemsStory.Add (leaderItem);
		}

		for (int i = 0; i < pageTotalItemCount; i++) {
			GameObject leaderItem = Instantiate(LeaderItem) as GameObject;
			leaderItem.SetActive (false);
			leaderItem.transform.parent = GridRootChallenge.transform;
			leaderItem.transform.localScale = Vector3.one;
			leaderItem.transform.localRotation = Quaternion.identity;
			leaderItem.transform.localPosition = Vector3.zero;

			leaderItem.transform.localPosition = new Vector3 (0, i * (-99), 0);

			gridItemsChallenge.Add (leaderItem);
		}

		maxItemCount = Mathf.Max (leaderBoardsStory.Count,leaderBoardsChallenge.Count);

		if (maxItemCount == 0) {
			m_LeaderboardScoresDownloaded_Story = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded_Story);
			m_LeaderboardScoresDownloaded_User_Story = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded_User_Story);

			m_LeaderboardScoresDownloaded_Challenge = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded_Challenge);
			m_LeaderboardScoresDownloaded_User_Challenge = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded_User_Challenge);

			steamLeaderboard_StoryScore = SteamAPIMgr.Instance.GetLeaderBoardHandle (GameMode.Story);
			steamLeaderboard_ChallengeScore = SteamAPIMgr.Instance.GetLeaderBoardHandle (GameMode.Challenge);

			DownloadLeaderboardEntriesStory();  
			DownloadLeaderboardEntriesChallenge(); 

			userRank.text = "";
			userName.text = "";
			userScore.text = "";

			userRankChallenge.text = "";
			userNameChallenge.text = "";
			userScoreChallenge.text = "";
	
			FreezPage ();

			LoadingObject.SetActive (true);
			LoadingObjectChallenge.SetActive (true);
		}
		else {
		
			totalPageCount = ((maxItemCount - 1) / pageTotalItemCount);
			currentPageIndex = 0;
			for (int i = 0; i < PageIndactors.Length; ++i) {
				PageIndactors [i].SetActive (i <= totalPageCount);
			}

			PageRoot.transform.localPosition = new Vector3 (430 - (totalPageCount - 1) * 35,-507,0);

			RefreshPage ();

			userRank.text = cachedUserRankStory;
			userName.text = cachedUserNameStory;
			userScore.text = cachedUserScoreStory;

			userRankChallenge.text = cachedUserRankChallenge;
			userNameChallenge.text = cachedUserNameChallenge;
			userScoreChallenge.text = cachedUserScoreChallenge;

			LoadingObject.SetActive (false);
			LoadingObjectChallenge.SetActive (false);
		}

	}


	public void RefreshPageIndactors(int index)
	{
		for (int i = 0; i < PageIndactors.Length; ++i) {
			PageIndactors [i].GetComponent<UISprite>().enabled = (i == index);
		}
	}

	public void RefreshPage()
	{
		FreezPage ();

		RefreshPageIndactors (currentPageIndex);
		int startIndex = currentPageIndex * pageTotalItemCount;

		for (int i = 0; i < pageTotalItemCount; ++i) {
			GameObject item = gridItemsStory [i];
			int currentIndex = i + startIndex;
			if (currentIndex >= leaderBoardsStory.Count) {
				item.SetActive (false);
				continue;
			}
			else {
				item.SetActive (true);
			}

			var leaderItem = leaderBoardsStory[currentIndex];

			RefreshItemUI (i,item,leaderItem,true);
		}

		for (int i = 0; i < pageTotalItemCount; ++i) {
			GameObject item = gridItemsChallenge [i];
			int currentIndex = i + startIndex;
			if (currentIndex >= leaderBoardsChallenge.Count) {
				item.SetActive (false);
				continue;
			}
			else {
				item.SetActive (true);
			}

			var leaderItem = leaderBoardsChallenge[currentIndex];

			RefreshItemUI (i,item,leaderItem,false);
		}
	}

	public void RefreshItemUI(int index,GameObject gameObject,LeaderBoardItem leaderItem,bool isStory)
	{
		gameObject.transform.FindChild ("name").GetComponent<UILabel> ().text = leaderItem.name;
		gameObject.transform.FindChild ("rank").GetComponent<UILabel> ().text = leaderItem.rank.ToString();
		gameObject.transform.FindChild ("score").GetComponent<UILabel> ().text = leaderItem.score.ToString("###,###");

		if (currentPageIndex == 0) {
			if (index == 0) {
				gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (1,0.109f,0.65f);
			}
			else if(index == 1)
			{
				gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (1,0.0667f,0.243f);
			}
			else if(index == 2)
			{
				gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.925f,0.514f,0.074f);
			}
			else
			{
				if (isStory) {
					if (index % 2 != 0) {
						gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.247f,0.325f,0.203f);
					}
					else {
						gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.196f,0.258f,0.165f);
					}
				} else {
					if (index % 2 != 0) {
						gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.321f,0.168f,0.314f);
					}
					else {
						gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.255f,0.133f,0.255f);
					}
				}
			
			}

		}
		else {
			if (index % 2 == 0) {
				gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.223f,0.196f,0.267f);
			}
			else {
				gameObject.transform.FindChild ("back").GetComponent<UISprite> ().color = new Color (0.122f,0.102f,0.157f);
			}
		}
	}

	public string GetTime(uint time)
	{
		DateTime dataTime = new DateTime (1970,1,1,0,0,0,DateTimeKind.Utc);
		return dataTime.Year + "/" + dataTime.Month + "/" + dataTime.Day + " @ " + dataTime.Hour + ":" + dataTime.Minute;
	}

	public void NextPage(int toLeft)
	{
		App.SoundMgr.Play (AudioResources.menuUIHit);
		if (isFreezPageRefresh)
			return;

		if (toLeft == 1) {
			currentPageIndex--;
		}
		else {
			currentPageIndex++;
		}

		if (currentPageIndex < 0)
			currentPageIndex = totalPageCount;
		else if(currentPageIndex > totalPageCount)
			currentPageIndex = 0;

		RefreshPage ();
	}

	private void UnFreez()
	{
		isFreezPageRefresh = false;
	}

	void FreezPage()
	{
		isFreezPageRefresh = true;
		Invoke ("UnFreez", 0.2f);
	}

	public override void FadeOut()
	{
		DeActive ();
	}

	public void OnBackClick()
	{
		if (isFreezPageRefresh)
			return;
		
		App.SoundMgr.Play (AudioResources.menuUIHit);
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.LeaderBoardUI);

		App.Game.GUIFrameMgr.Active (GUIFrameID.RotateMusicSelectUI);
		//App.Game.GUIFrameMgr.GetFrame (GUIFrameID.RotateMusicSelectUI).CallWapperFunction ("ResumeMusicSelected");
	}


	//排行榜
	//请求下载最新排行榜数据 回调
	private void OnLeaderboardScoresDownloaded_Story(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)  
	{  
		//判断是不是同一个排行榜
		if (pCallback.m_hSteamLeaderboard == steamLeaderboard_StoryScore)  
		{
			LoadingObject.SetActive (false);
			if (pCallback.m_cEntryCount > 0)  
			{
				for (int i = 0; i < pCallback.m_cEntryCount; i++)  
				{
					LeaderboardEntry_t leaderboardEntry;  
					int[] details = new int[pCallback.m_cEntryCount];  
					//获取下载好的排行榜信息
					SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, i, out leaderboardEntry, details, pCallback.m_cEntryCount);  
					leaderBoardsStory.Add (new LeaderBoardItem (leaderboardEntry.m_nGlobalRank,SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser),leaderboardEntry.m_nScore));

//					for(int j = 0; j < 13;j++)
//					{
//						leaderBoardsStory.Add (new LeaderBoardItem (leaderboardEntry.m_nGlobalRank,SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser),leaderboardEntry.m_nScore));
//					}

					if (leaderBoardsStory.Count >= 100)
						break;
				}

				totalPageCount = ((Mathf.Max(leaderBoardsChallenge.Count,leaderBoardsStory.Count) - 1) / pageTotalItemCount);
				currentPageIndex = 0;
				for (int i = 0; i < PageIndactors.Length; ++i) {
					PageIndactors [i].SetActive (i <= totalPageCount);
				}

				PageRoot.transform.localPosition = new Vector3 (430 - (totalPageCount - 1) * 35,-507,0);

				RefreshPage ();
			}  
			else  
			{  
				FreezPage ();
				Debug.LogError("排行榜数据为空！");  
			}  
		}  
	}  

	private void OnLeaderboardScoresDownloaded_User_Story(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)  
	{  
		//判断是不是同一个排行榜
		if (pCallback.m_hSteamLeaderboard == steamLeaderboard_StoryScore)  
		{
			if (pCallback.m_cEntryCount > 0)  
			{
				LeaderboardEntry_t leaderboardEntry;  
				int[] details = new int[pCallback.m_cEntryCount];  
				//获取下载好的排行榜信息
				SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, 0, out leaderboardEntry, details, pCallback.m_cEntryCount);  

				userRank.text = leaderboardEntry.m_nGlobalRank.ToString ();
				userScore.text = leaderboardEntry.m_nScore.ToString ("###,###");
				userName.text = SteamFriends.GetPersonaName ();

				cachedUserRankStory = userRank.text;
				cachedUserNameStory = userName.text;
				cachedUserScoreStory = userScore.text;
			}  
			else  
			{  
				Debug.LogError("排行榜数据为空！");  
			}  
		}  
	}  

	public void DownloadLeaderboardEntriesStory()  
	{  
		if (steamLeaderboard_StoryScore.m_SteamLeaderboard != 0)  
		{  
			var handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_StoryScore, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 100);  
			m_LeaderboardScoresDownloaded_Story.Set(handle);  

			CSteamID[] users = new CSteamID[1];
			users [0] = SteamUser.GetSteamID ();
			var handle2 = SteamUserStats.DownloadLeaderboardEntriesForUsers(steamLeaderboard_StoryScore,users,1);  
			m_LeaderboardScoresDownloaded_User_Story.Set(handle2);  
		}  
	} 

	private void OnLeaderboardScoresDownloaded_Challenge(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)  
	{  
		//判断是不是同一个排行榜
		if (pCallback.m_hSteamLeaderboard == steamLeaderboard_ChallengeScore)  
		{
			LoadingObjectChallenge.SetActive (false);
			if (pCallback.m_cEntryCount > 0)  
			{
				for (int i = 0; i < pCallback.m_cEntryCount; i++)  
				{
					LeaderboardEntry_t leaderboardEntry;  
					int[] details = new int[pCallback.m_cEntryCount];  
					//获取下载好的排行榜信息
					SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, i, out leaderboardEntry, details, pCallback.m_cEntryCount);  
					leaderBoardsChallenge.Add (new LeaderBoardItem (leaderboardEntry.m_nGlobalRank,SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser),leaderboardEntry.m_nScore));

//					for(int j = 0; j < 33;j++)
//					{
//						leaderBoardsChallenge.Add (new LeaderBoardItem (leaderboardEntry.m_nGlobalRank,SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser),leaderboardEntry.m_nScore));
//					}

					if (leaderBoardsChallenge.Count >= 100)
						break;
				}

				totalPageCount = ((Mathf.Max(leaderBoardsChallenge.Count,leaderBoardsStory.Count) - 1) / pageTotalItemCount);
				currentPageIndex = 0;
				for (int i = 0; i < PageIndactors.Length; ++i) {
					PageIndactors [i].SetActive (i <= totalPageCount);
				}

				PageRoot.transform.localPosition = new Vector3 (430 - (totalPageCount - 1) * 35,-507,0);

				RefreshPage ();
			}  
			else  
			{  
				FreezPage ();
				Debug.LogError("排行榜数据为空！");  
			}  
		}  
	}  

	private void OnLeaderboardScoresDownloaded_User_Challenge(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)  
	{  
		//判断是不是同一个排行榜
		if (pCallback.m_hSteamLeaderboard == steamLeaderboard_ChallengeScore)  
		{
			if (pCallback.m_cEntryCount > 0)  
			{
				LeaderboardEntry_t leaderboardEntry;  
				int[] details = new int[pCallback.m_cEntryCount];  
				//获取下载好的排行榜信息
				SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, 0, out leaderboardEntry, details, pCallback.m_cEntryCount);  

				userRankChallenge.text = leaderboardEntry.m_nGlobalRank.ToString ();
				userScoreChallenge.text = leaderboardEntry.m_nScore.ToString ("###,###");
				userNameChallenge.text = SteamFriends.GetPersonaName ();

				cachedUserRankChallenge = userRankChallenge.text;
				cachedUserNameChallenge = userNameChallenge.text;
				cachedUserScoreChallenge = userScoreChallenge.text;
			}  
			else  
			{  
				Debug.LogError("排行榜数据为空！");  
			}  
		}  
	}  

	public void DownloadLeaderboardEntriesChallenge()  
	{  
		if (steamLeaderboard_ChallengeScore.m_SteamLeaderboard != 0)  
		{  
			var handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_ChallengeScore, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 100);  
			m_LeaderboardScoresDownloaded_Challenge.Set(handle);  

			CSteamID[] users = new CSteamID[1];
			users [0] = SteamUser.GetSteamID ();
			var handle2 = SteamUserStats.DownloadLeaderboardEntriesForUsers(steamLeaderboard_ChallengeScore,users,1);  
			m_LeaderboardScoresDownloaded_User_Challenge.Set(handle2);  
		}  
	} 
	#endif
}
