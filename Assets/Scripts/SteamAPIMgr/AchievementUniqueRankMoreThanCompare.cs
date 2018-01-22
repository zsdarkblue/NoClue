using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{
	public class AchievementUniqueRankMoreThanCompare : AchievementBase {
		private int compareRank;
		public AchievementUniqueRankMoreThanCompare(string apiName,int _rank,int _compareValue) : base(apiName){
			compareValue = _compareValue;
			compareRank = _rank;
		}

//		public override bool CheckUnlockRequirement()
//		{
//			int passedCount = 0;
//			for (int i = 0; i < SteamAPIMgr.Instance.TotalLevelCount; i++) {
//				int levelScore = SteamAPIMgr.Instance.GetStat (StatsName.BestRecordScoreLevel + i.ToString ()).value;
//				if (levelScore > 0) {
//					var conf = App.Game.ConfigureMgr.GetStoryConfig ().GetMusicConfig (i);
//
//				}
//			}
//
//		//	Debug.Log ("@@@@ " + apiName + "   rank:" + compareRank + "  passedCount:" + passedCount + "   compareCount:" + compareValue);
//			currentValue = passedCount;
//			if (currentValue >= compareValue) {
//				UnlockAchievement ();
//				return true;
//			}
//			else {
//				return false;
//			}
//		}
	}
}
