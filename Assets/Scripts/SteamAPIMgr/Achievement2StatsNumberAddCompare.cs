using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{
	public class Achievement2StatsNumberAddCompare : AchievementBase {
		private string statApiName1;
		private string statApiName2;
		public Achievement2StatsNumberAddCompare(string apiName,string _statApiName1,string _statApiName2,int _compareValue) : base(apiName){
			statApiName1 = _statApiName1;
			statApiName2 = _statApiName2;
			compareValue = _compareValue;
		}

		public override bool CheckUnlockRequirement()
		{
			int currentValue1 = 0;
			int currentValue2 = 0;
			//will get wrong number when you set and get too quickly........

			#if !Steamworks_Off
			SteamUserStats.GetStat(statApiName1, out currentValue1);
			SteamUserStats.GetStat(statApiName2, out currentValue2);
			currentValue = currentValue1 + currentValue2;
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
