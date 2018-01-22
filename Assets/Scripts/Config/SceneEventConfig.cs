using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace BeatsFever.Config
{
	public class SceneEventData {
		public SceneEventData(string text)
		{
			InitFromString (text);
		}

		void InitFromString(string text)
		{
			string[] segments = text.Split (',');
			for (int i = 0; i < segments.Length; ++i) {
				string value = segments [i];

				if (0 == i) {
					eventID = int.Parse(segments [i]);
						
				}
				if (1 == i) {
					string[] configTimes = segments [i].Split (';');
					for (int timeIndex = 0; timeIndex < configTimes.Length; ++timeIndex) {
						int time = int.Parse (configTimes [timeIndex]);
						times.Add (time);
					}
				}
			}
		}
		public int eventID;
		public List<int> times = new List<int>();
	}

	public class SceneEventConfig {

		public Dictionary<string,Dictionary<int,SceneEventData>> SceneEventDic = new Dictionary<string, Dictionary<int, SceneEventData>>();

		public Dictionary<int,SceneEventData> GetSceneEventDatas(string musicName)
		{
			if (!SceneEventDic.ContainsKey (musicName)) {
				LoadConfig (musicName);
			}

			if (SceneEventDic.ContainsKey (musicName)) {
				return SceneEventDic [musicName];
			}
			else {
				return null;
			}
		}

		void LoadConfig(string musicName)
		{
			TextAsset text = Resources.Load ("MusicEventData/" +  musicName) as TextAsset;
			if (null == text)
				return;
			
			string fileString = text.text;
			string[] sArray = Regex.Split (fileString,"\n",RegexOptions.Multiline);

			Dictionary<int,SceneEventData> datas = new Dictionary<int,SceneEventData> ();
			for (int i = 1; i < sArray.Length; ++i) {
				if (!string.IsNullOrEmpty (sArray [i])) {
					var eventData = new SceneEventData(sArray [i]);
					datas.Add (eventData.eventID,eventData);
				}
			}

			SceneEventDic.Add (musicName, datas);
		}
	}
}
