using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{
	public class StatsBase {

		public StatsBase(string _statName)  
		{  
			apiName = _statName;
		}

		public string apiName;
		public int value;

		public void AddValueToStat(int addValue)
		{
			#if !Steamworks_Off
			RefreshDataFromSteam ();
			SteamUserStats.SetStat(apiName,value + addValue);
			SteamAPIMgr.Instance.needStoreNewStats = true;
			#endif
		}

		public bool UpdateValueIfBigger(int newValue)
		{
			RefreshDataFromSteam ();
			if (newValue > value) {
				value = newValue;

				#if !Steamworks_Off
				SteamUserStats.SetStat(apiName,value);
				#endif

				SteamAPIMgr.Instance.needStoreNewStats = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void ForceUpdateValue(int newValue)
		{
			RefreshDataFromSteam ();
			value = newValue;

			#if !Steamworks_Off
			SteamUserStats.SetStat(apiName,value);
			#endif

			SteamAPIMgr.Instance.needStoreNewStats = true;
		}

		public void RefreshDataFromSteam()
		{
			#if !Steamworks_Off
			SteamUserStats.GetStat(apiName, out value);
			#endif

		}
	}
}
