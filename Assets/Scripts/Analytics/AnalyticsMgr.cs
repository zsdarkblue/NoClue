using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace BeatsFever
{
	public class AnalyticsMgr {
		public void GameStart()
		{
			Analytics.CustomEvent("gameStart",new Dictionary<string,object>{
				{"deviceName",SystemInfo.deviceName},
				{"deviceUniqueIdentifier",SystemInfo.deviceUniqueIdentifier},
				{"deviceModel",SystemInfo.deviceModel},
				{"graphicsDeviceVendor",SystemInfo.graphicsDeviceVendor},
				{"graphicsDeviceName",SystemInfo.graphicsDeviceName}
						});
			Analytics.SetUserId (SystemInfo.deviceUniqueIdentifier);
		}

		public void Guide_StandReady()
		{
			Analytics.CustomEvent("standReady",new Dictionary<string,object>{
				{"userid",SystemInfo.deviceUniqueIdentifier}
			});
		}

		public void Guide_HitTeachBulletOver()
		{
			Analytics.CustomEvent("hitTeachBulletOver",new Dictionary<string,object>{
				{"userid",SystemInfo.deviceUniqueIdentifier}
			});
		}

		public void Guide_HeightCheckOver()
		{
			Analytics.CustomEvent("heightCheckOver",new Dictionary<string,object>{
				{"userid",SystemInfo.deviceUniqueIdentifier}
			});
		}

		public void LevelStart(int levelId,int speed)
		{
			Analytics.CustomEvent("levelStart",new Dictionary<string,object>{
				{"userid",SystemInfo.deviceUniqueIdentifier},
				{"levelId",levelId},
				{"speed",speed}
			});
		}

		public void LevelFinish(int levelId,int score,int maxCombo,int feverNumber,int speed)
		{
			Analytics.CustomEvent("levelFinish",new Dictionary<string,object>{
				{"userid",SystemInfo.deviceUniqueIdentifier},
				{"levelId",levelId},
				{"score",score},
				{"maxCombo",maxCombo},
				{"feverNumber",feverNumber},
				{"speed",speed}
			});
		}
	}
}


