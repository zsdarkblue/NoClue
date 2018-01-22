using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.Config;

public class MusicItem : MonoBehaviour {

	public UILabel MusicName;
	public Transform RankPosMask;
	public GameObject StarRoot;
	public GameObject Star;

	public GameObject PlayButton;
	public GameObject LockItem;

	private StoryMusicData musicConf;
	public void Init(int levelId)
	{

		GetComponentInChildren<QuickMessage> ().param = levelId;
		musicConf = App.Game.ConfigureMgr.GetStoryConfig ().GetMusicConfig (levelId);


		if (musicConf.IsOpenToUser) {
			PlayButton.SetActive (true);
			LockItem.SetActive (false);

			MusicName.text = musicConf.MusicShowName;
			SetDifficult (musicConf.Difficulty);
			SetRank (musicConf.Rank);
		}
		else
		{
			PlayButton.SetActive (false);
			LockItem.SetActive (true);

			MusicName.text = "";
		}
	}

	public void SetDifficult(int difficult)
	{
		for (int i = 0; i < difficult; ++i) {
			GameObject go = Instantiate (Star) as GameObject;
			go.SetActive (true);
			go.transform.parent = StarRoot.transform;
			go.transform.localScale = Vector3.one;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localPosition = new Vector3 (-0.11f * i,0,0);
		}
	}

	public void SetRank(int rank)
	{
		GameObject musicRank = Instantiate(App.ResourceMgr.LoadRes ("musicRank"+rank.ToString())) as GameObject;
		musicRank.transform.parent = this.gameObject.transform;
		musicRank.transform.localScale = Vector3.one * 0.5f;
		musicRank.transform.localRotation = Quaternion.identity;
		musicRank.transform.localPosition = RankPosMask.localPosition;
	}
}
