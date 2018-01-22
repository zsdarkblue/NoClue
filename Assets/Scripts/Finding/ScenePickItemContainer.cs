using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatsFever;

public class ScenePickItemContainer : MonoBehaviour {
	List<PickItemBehaviour> pickItemList = new List<PickItemBehaviour>();

	// Use this for initialization
	void Awake () {
		var pickItems = GetComponentsInChildren<PickItemBehaviour> ();
		foreach (var item in pickItems) {
			if (item.SwitchType != SwitchType.Null) {
				pickItemList.Add (item);
			}
		}

		int needRemoveCount = pickItemList.Count - ScoreMgr.ChangeItemTotalCount;
		for (int i = 0; i < needRemoveCount; i++) {
			int dayIndex = Random.Range (0,pickItemList.Count);
			pickItemList [dayIndex].name = "CantChangeItem";
			pickItemList [dayIndex].SwitchType = SwitchType.FrzzedAfterInitItem;
			pickItemList.RemoveAt (dayIndex);
		}

		//test code
		foreach (var item in pickItemList) {
			item.name = "ActiveSwitchItem";
		}

		foreach (var item in pickItems) {
			item.Init ();
		}
	}

	public void RandomFindOne()
	{
		foreach (var item in pickItemList) {
			if (!item.IsFounded ()) {
				item.TriggerPlayerPickAction ();
				App.Game.ScoreMgr.UpdateScore (ScoreMgr.ScoreType.Right);
				DeviceContainer.Instance.ShowPopUpItem (item.gameObject, true);
				break;
			}
		}
	}

	public void PrepareSwitch(bool isSwitchToDay)
	{
		foreach (var item in pickItemList) {
			item.PrepareSwitch (isSwitchToDay);
		}
	}
	public void SwitchDayNight(bool isDay)
	{
		foreach (var item in pickItemList) {
			item.SwitchDayNight (isDay);
		}
	}

	public void EndSwitch(bool isSwitchToDay)
	{
		foreach (var item in pickItemList) {
			item.EndSwitch (isSwitchToDay);
		}
	}
}
