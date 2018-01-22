using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;
using BeatsFever.UI;
using System;

public class AchievementUI : FrameBaseUI {

	public UILabel totalAchievementDesc;
	public UISlider totalProgress;

	public GameObject GridRoot;
	public GameObject AchievementItem;

	public GameObject PageRoot;
	public GameObject[] PageIndactors;

	private List<GameObject> gridItems = new List<GameObject>();
	private int pageTotalItemCount = 12;
	private int currentPageIndex = 0;
	private int totalPageCount = 0;

	bool isFreezPageRefresh = true;

	List<AchievementBase> achievements = new List<AchievementBase>();
	List<AchievementBase> achievementsLocked = new List<AchievementBase>();

	#if !Steamworks_Off
	public override void FadeIn()
	{
		for (int i = 0; i < pageTotalItemCount; i++) {
			GameObject recordItem = Instantiate(AchievementItem) as GameObject;
			recordItem.SetActive (false);
			recordItem.transform.parent = GridRoot.transform;
			recordItem.transform.localScale = Vector3.one;
			recordItem.transform.localRotation = Quaternion.identity;
			recordItem.transform.localPosition = Vector3.zero;

			//recordItem.transform.localPosition = new Vector3 ((i % 2) * 1000, (i/2) * (-120), 0);
			recordItem.transform.localPosition = new Vector3 ((i % 2) * 1440, (i/2) * (-180), 0);
			gridItems.Add (recordItem);
		}

		foreach (var achievement in SteamAPIMgr.Instance.achievements.Values) {
			if (achievement.isAchieved) {
				achievements.Add (achievement);
			}
			else {
				achievementsLocked.Add (achievement);
			}
		}
			
		float progress = achievements.Count / (float)SteamAPIMgr.Instance.achievements.Count;
		totalProgress.value = progress;
		totalAchievementDesc.text = achievements.Count.ToString() + " of " + SteamAPIMgr.Instance.achievements.Count.ToString() + " (" + (int)(progress * 100) + "%)";

		totalPageCount = ((SteamAPIMgr.Instance.achievements.Count - 1) / pageTotalItemCount);

		for (int i = 0; i <= totalPageCount; ++i) {
			PageIndactors [i].SetActive (true);
		}

		PageRoot.transform.localPosition = new Vector3 (430 - (totalPageCount - 1) * 35,-507,0);

		achievements.AddRange (achievementsLocked);
		currentPageIndex = 0;

		RefreshPage ();
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
			GameObject item = gridItems [i];
			int currentIndex = i + startIndex;
			if (currentIndex >= achievements.Count) {
				item.SetActive (false);
				continue;
			}
			else {
				item.SetActive (true);
			}

			var achievement = achievements[currentIndex];

			item.transform.FindChild ("name").GetComponent<UILabel> ().text = achievement.name;
			item.transform.FindChild ("desc").GetComponent<UILabel> ().text = achievement.description;
			item.transform.FindChild ("progressBar").gameObject.SetActive (!achievement.isAchieved);
			item.transform.FindChild ("progressText").gameObject.SetActive (!achievement.isAchieved);

			if (achievement.isAchieved) {
				item.transform.FindChild ("icon").GetComponent<UISprite> ().spriteName = achievement.apiName;
				item.transform.FindChild ("unlockTime").GetComponent<UILabel> ().text = GameUtil.GetTimeDescByUtcSeconds (achievement.unlockTime);
			}
			else {
				item.transform.FindChild ("icon").GetComponent<UISprite> ().spriteName = achievement.apiName + "_unlock";
				item.transform.FindChild ("unlockTime").GetComponent<UILabel> ().text = "";

				if (achievement.compareValue == 0) {
					item.transform.FindChild ("progressBar").gameObject.SetActive (false);
					item.transform.FindChild ("progressText").gameObject.SetActive (false);
				}
				else {
					item.transform.FindChild ("progressBar").GetComponent<UISlider> ().value = achievement.currentValue / (float)achievement.compareValue;
					item.transform.FindChild ("progressText").GetComponent<UILabel> ().text =  achievement.currentValue.ToString() + " / "  + achievement.compareValue.ToString();
				}
			}
		}
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
		App.Game.GUIFrameMgr.DeActive (GUIFrameID.AchievementUI);

		App.Game.GUIFrameMgr.Active (GUIFrameID.RotateMusicSelectUI);
		//App.Game.GUIFrameMgr.GetFrame (GUIFrameID.RotateMusicSelectUI).CallWapperFunction ("ResumeMusicSelected");
	}
	#endif
}
