using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace BeatsFever.Config
{
	public class StoryMusicData {
		public StoryMusicData(string text)
		{
			InitFromString (text);
		}

		void InitFromString(string text)
		{
			string[] segments = text.Split (',');
			for (int i = 0; i < segments.Length; ++i) {
				string value = segments [i];
				if (string.IsNullOrEmpty (value))
					continue;

				int intValue;
				if (0 == i) {
					if (int.TryParse (segments [i],out intValue))
						ID = intValue;
				}
				if (1 == i) {
					if (int.TryParse (segments [i],out intValue))
						PreviousID = intValue;
				}
				else if (2 == i) {
					if (int.TryParse (segments [i],out intValue))
						NextID = intValue;
				}
				else if (3 == i) {
					if (int.TryParse (segments [i], out intValue)) {
						IsOpenToUser = intValue == 1 ? true : false;
					}
				}
				else if (4 == i) {
					CityName = segments [i];
				}
				else if (5 == i) {
					SceneName = segments [i];
				}
				else if (6 == i) {
					MusicRes = segments [i];
				}
				else if (7 == i) {
					MusicShowName = segments [i];
				}
				else if (8 == i) {
					if (int.TryParse (segments [i],out intValue))
						Difficulty = intValue;
				}
				else if (9 == i) {
					if (int.TryParse (segments [i],out intValue))
						Rank = intValue;
				}
				else if (10 == i) {
					if (int.TryParse (segments [i],out intValue))
						MaxScore = intValue;
				}
				else if (11 == i) {
					AuthorName = segments [i];
				}
				else if (12 == i) {
					if (int.TryParse (segments [i],out intValue))
						CoverTexIndex = intValue;
				}
			}
		}
		public int ID;
		public int PreviousID = -1;
		public int NextID = -1;
		public bool IsOpenToUser = false;

		public int Difficulty;
		public int Rank;
		public int MaxScore;

		public string CityName;
		public string SceneName;
		public string MusicRes;
		public string MusicShowName;
		public string AuthorName;
		public int CoverTexIndex;
	}

	public class StoryConfig {

		public Dictionary<string,List<StoryMusicData>> CityMusicDic = new Dictionary<string, List<StoryMusicData>> ();
		public Dictionary<int,StoryMusicData> MusicDataDic = new Dictionary<int, StoryMusicData>();

		public int GetTotalLevelCount()
		{
			return MusicDataDic.Count;
		}

		public void LoadStoryConfig()
		{
			TextAsset text = Resources.Load ("musicConfig") as TextAsset;
			string fileString = text.text;
			string[] sArray = Regex.Split (fileString,"\n",RegexOptions.Multiline);

			List<StoryMusicData> datas = new List<StoryMusicData> ();
			for (int i = 1; i < sArray.Length; ++i) {
				if (!string.IsNullOrEmpty (sArray [i])) {
					datas.Add (new StoryMusicData(sArray [i]));
				}
			}

			foreach (var data in datas) {
				MusicDataDic.Add (data.ID,data);

				if (CityMusicDic.ContainsKey (data.CityName)) {
					CityMusicDic [data.CityName].Add (data);
				}
				else {
					List<StoryMusicData> musicDataList = new List<StoryMusicData> ();
					musicDataList.Add (data);
					CityMusicDic.Add (data.CityName,musicDataList);
				}
			}

			foreach (var key in CityMusicDic.Keys) {
				CityMusicDic [key].Sort (new CompareChain<StoryMusicData>().Add(item => item.ID));
			}
		}

		public StoryMusicData GetMusicConfig(int id)
		{
			return MusicDataDic[id];
		}

		public StoryMusicData GetNextLevel(int id)
		{
			int next = MusicDataDic [id].NextID;
			if (next > 0) {
				return MusicDataDic [next];
			}
			else {
				return null;
			}
		}
	}
}
