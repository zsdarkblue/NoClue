using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif
using System;

namespace BeatsFever{
	public class AchievementOpenDayCompare : AchievementBase {
		private int compareYear;
		private int compareMonth;
		private int compareDay;
		public AchievementOpenDayCompare(string apiName,int year,int month,int day) : base(apiName){
			compareYear = year;
			compareMonth = month;
			compareDay = day;
		}

		public override bool CheckUnlockRequirement()
		{
			DateTime dataTime = DateTime.Now;
			bool passTest = false;
			if (compareYear > 0) {
				if(dataTime.Year == compareYear && dataTime.Month == compareMonth && dataTime.Day == compareDay)
					passTest = true;
			} else {
				if(dataTime.Month == compareMonth && dataTime.Day == compareDay)
					passTest = true;
			}
			if (passTest) {
				UnlockAchievement ();
				return true;
			}
			else {
				return false;
			}
		}
	}
}
