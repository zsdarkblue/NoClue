using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{
	public class AchievementPlayedMinsCompare : AchievementBase {
		public AchievementPlayedMinsCompare(string apiName,int _compareValue) : base(apiName){
			compareValue = _compareValue;
		}

		public override bool CheckUnlockRequirement()
		{
			currentValue = SteamAPIMgr.Instance.PlayedMins;
			if (currentValue >= compareValue) {
				UnlockAchievement ();
				return true;
			}
			else {
				return false;
			}
		}
	}
}
