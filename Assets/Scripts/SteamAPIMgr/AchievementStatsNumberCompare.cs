using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{
	public class AchievementStatsNumberCompare : AchievementBase {
		private string statApiName;

		public AchievementStatsNumberCompare(string apiName,string _statApiName,int _compareValue) : base(apiName){
			statApiName = _statApiName;
			compareValue = _compareValue;
		}

		public override bool CheckUnlockRequirement()
		{
			#if !Steamworks_Off
			//will get wrong number when you set and get too quickly........
			SteamUserStats.GetStat(statApiName, out currentValue);
			if (currentValue >= compareValue) {
				UnlockAchievement ();
				return true;
			}
			else {
				return false;
			}
			#else
			return false;
			#endif
		}
	}
}
