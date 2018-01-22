using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace BeatsFever{
	
	public class RhythmMgr {

		public delegate void OnMusicResetHandler();
		public OnMusicResetHandler OnMusicReset;
		public enum BeatHitType
		{
			Null,
			Good,
			Prefect,
		}

		public static float[] SpectrumData = new float[4096];

		Dictionary<int,OnsetData> allOnsetDatas = new Dictionary<int, OnsetData>();

		private Queue<RhythmBulletBase> originalRhythmDataQueue = new Queue<RhythmBulletBase>();
		private Queue<RhythmBulletBase> activeBulletDataQueue = new Queue<RhythmBulletBase>();
		private float bpm;
		public float BulletPreShowSeconds;
		public AudioSource AudioSource;

		public void Start()
		{
			//set up AudioSource
			AudioSource = DeviceContainer.Instance.MainAudioSource;
		}

		public void RefreshAudioData(float _currentBPM)
		{
			bpm = _currentBPM;
			BulletPreShowSeconds = bpm * RhyThmDefine.BulletShowPreTimeParam;
		}
		public float GetBPM()
		{
			return bpm;
		}

		string[] ReadDataFromCSVFile(string fileName)
		{
			string fileString = "";
			#if UNITY_EDITOR
			string path = Application.dataPath + Path.DirectorySeparatorChar + "RhyThmDataFile" + Path.DirectorySeparatorChar + fileName + ".txt";
			StreamReader sr = new StreamReader(path);
			fileString = sr.ReadToEnd ();
			sr.Close ();
			#else
			TextAsset text = Resources.Load (fileName) as TextAsset;
			fileString = text.text;
			#endif

			string[] sArray = Regex.Split (fileString,"\n",RegexOptions.Multiline);
			return sArray;
		}

		public void RebuildRhyOriginalRhythmDataQueue()
		{
			List<DotRhythm> dotItems = new List<DotRhythm>();
			Dictionary<int,SlideRhythm> slideItems = new Dictionary<int, SlideRhythm> ();
			originalRhythmDataQueue.Clear ();
			foreach (OnsetData data in allOnsetDatas.Values)
			{
				if (data.slideID < 0) {
					dotItems.Add (new DotRhythm (data));
				}
				else {
					if (slideItems.ContainsKey (data.slideID)) {
						slideItems [data.slideID].AddNewData (data);
					}
					else {
						slideItems [data.slideID] = new SlideRhythm (data);
					}
				}
			}


			List<RhythmBulletBase> rhythmList = new List<RhythmBulletBase>();
			foreach (var slide in slideItems.Values) {
				if (slide.datas.Count > 1) {
					slide.PrepareSlide ();
					rhythmList.Add (slide);
				}
				else {
					dotItems.Add (new DotRhythm (slide.datas[0]));
				}
			}

			foreach (var dot in dotItems) {
				rhythmList.Add (dot);		
			}

			rhythmList.Sort(new CompareChain<RhythmBulletBase>().Add(item => item.HitTime));

			for (int i = 0; i < rhythmList.Count; ++i) {
				originalRhythmDataQueue.Enqueue (rhythmList [i]);
			}
		}

		public void BuildRhyDataFromFile(string fileName)
		{
			allOnsetDatas.Clear ();
			var datas = ReadDataFromCSVFile (fileName);
			for (int i = 1; i < datas.Length; ++i) {
				if (!string.IsNullOrEmpty (datas [i])) {
					OnsetData onsetData = new OnsetData(datas [i]);
					allOnsetDatas.Add (onsetData.id,onsetData);			
				}
			}

			RebuildRhyOriginalRhythmDataQueue ();
		}

		private static RhythmMgr instance;
		public static RhythmMgr Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new RhythmMgr();
				}

				return instance;
			}
		}

		public void ResetRhythmData(float targetTime = 0)
		{
			activeBulletDataQueue = new Queue<RhythmBulletBase> (originalRhythmDataQueue);

			if (targetTime > 0) {
				while(true)
				{
					var item = activeBulletDataQueue.Peek ();
					if ((targetTime) * 1000 > item.HitTime) {
						activeBulletDataQueue.Dequeue ();
					}
					else {
						break;
					}
				}
			}
		}

		public Queue<RhythmBulletBase> DepCopyOriginalRhythmDataQueue()
		{
			return new Queue<RhythmBulletBase> (originalRhythmDataQueue);
		}

		public void ResetMusicEffectsAndEvents()
		{
			if (OnMusicReset != null) {
				OnMusicReset ();
			}
		}

		public RhythmBulletBase GetReadyThythm(float musicTime)
		{
			if (activeBulletDataQueue.Count == 0)
				return null;

			var item = activeBulletDataQueue.Peek ();
			if ((musicTime + BulletPreShowSeconds) * 1000 > item.HitTime) {
				return activeBulletDataQueue.Dequeue ();
			}
			return null;
		}


		//**************************************************** music maker ****************************************************
		public void SetAudioSourceForMusicMaker(AudioSource _audioSource)
		{
			AudioSource = _audioSource;
		}


		void SaveData(string fileName)
		{
			string path = Application.dataPath + Path.DirectorySeparatorChar + "RhyThmDataFile" + Path.DirectorySeparatorChar + fileName + ".txt";
			StreamWriter sw = new StreamWriter (path,false);
			sw.WriteLine ("beats fever music data");

			List<int> keys = new List<int> ();
			foreach (var key in allOnsetDatas.Keys) {
				keys.Add (key);
			}

			keys.Sort ();
			foreach (var key in keys) {
				var data = allOnsetDatas [key];
				string s = data.id.ToString() + "," + 
					data.timeLine.ToString() + "," +
					data.positionX.ToString("0.000") + "," +
					data.positionY.ToString("0.000") + "," +
					//				(data.positionX  * RhyThmDefine.GlassCoordinateScale ).ToString() + "," +
					//				((data.positionY + RhyThmDefine.GlassCoordinateYoffset) * RhyThmDefine.GlassCoordinateScale ).ToString() + "," +
					data.slideID.ToString();

				sw.WriteLine (s);
			}

			sw.Close ();
		}


		public void RemoveOnsetData(int id)
		{
			allOnsetDatas.Remove (id);
			RebuildRhyOriginalRhythmDataQueue ();
			SaveData (AudioSource.clip.name);
		}

		public void ChangeOnsetData(int id,OnsetData onsetData)
		{
			allOnsetDatas [id] = onsetData;
			RebuildRhyOriginalRhythmDataQueue ();
			SaveData (AudioSource.clip.name);
		}
	}
}

