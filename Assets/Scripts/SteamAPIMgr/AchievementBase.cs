using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{

	public class AchievementBase {

		public string apiName;  
		public string name;  
		public string description;  
		public bool isAchieved;  
		public uint unlockTime;

		//for campare value achievements
		public int currentValue;
		public int compareValue;

		public Texture2D GetIcon()
		{
			string achievementTextureName = isAchieved ? "unlock" : "lock";
			achievementTextureName = apiName + achievementTextureName;
			Texture2D texture = Resources.Load (achievementTextureName) as Texture2D;
			return texture;
		}

		/// <summary>  
		/// Creates an Achievement. You must also mirror the data provided here in https://partner.steamgames.com/apps/achievements/yourappid  
		/// </summary>  
		/// <param name="achievement">The "API Name Progress Stat" used to uniquely identify the achievement.</param>  
		/// <param name="name">The "Display Name" that will be shown to players in game and on the Steam Community.</param>  
		/// <param name="desc">The "Description" that will be shown to players in game and on the Steam Community.</param>  
		public AchievementBase(string name)  
		{  
			apiName = name;  
			isAchieved = false;  
		}  


		public void RefreshDataFromSteam()
		{
			#if !Steamworks_Off
			bool ret = SteamUserStats.GetAchievementAndUnlockTime(apiName, out isAchieved, out unlockTime);
			if (ret) {
				name = SteamUserStats.GetAchievementDisplayAttribute(apiName, "name");
				description = SteamUserStats.GetAchievementDisplayAttribute(apiName, "desc");
			}
			else {
				Debug.LogError("SteamUserStats.GetAchievement failed for Achievement " + apiName + "\nIs it registered in the Steam Partner site?");
			}
			#endif
		}

		public virtual bool CheckUnlockRequirement()
		{
			return false;
		}

		public void UnlockAchievement()
		{
			#if !Steamworks_Off
			isAchieved = true;
			SteamUserStats.SetAchievement(apiName.ToString());
			SteamAPIMgr.Instance.needStoreNewStats = true;
			Debug.Log (" *************************** UnlockAchievement :" + apiName.ToString());
			#endif
		}
	}

}
