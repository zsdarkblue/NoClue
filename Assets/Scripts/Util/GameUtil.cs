using UnityEngine;
using System.Collections;
using System;
public class GameUtil {

	public static float ParseStingToFloat(string text)
	{
		string realText = text;
		bool isMinus = realText.StartsWith ("-");
		float num;
		if (isMinus) {
			realText = realText.Substring (1);
			num = -float.Parse (realText);
		}
		else {
			num = float.Parse (realText);
		}

		return num;
	}
		
	public static string GetRankSpriteName(int rank)
	{
		if (0 == rank) {
			return "Rank_small_D";
		} else if (1 == rank) {
			return "Rank_small_C";
		} else if (2 == rank) {
			return "Rank_small_B";
		} else if (3 == rank) {
			return "Rank_small_A";
		} else if (4 == rank) {
			return "Rank_small_S";
		} else {
			return "null";
		}
	}

	public static string GetTimeDescByUtcSeconds(uint time)
	{
		DateTime dataTime = new DateTime (1970,1,1,0,0,0,DateTimeKind.Utc);
		DateTime dataTimeNew = dataTime.AddSeconds (time);
		string text = "Unlocked " + dataTimeNew.Day + " ";
		if (1 == dataTimeNew.Month) {
			text += "Jan";
		}
		else if (2 == dataTimeNew.Month) {
			text += "Feb";
		}
		else if (3 == dataTimeNew.Month) {
			text += "Mar";
		}
		else if (4 == dataTimeNew.Month) {
			text += "Apr";
		}
		else if (5 == dataTimeNew.Month) {
			text += "May";
		}
		else if (6 == dataTimeNew.Month) {
			text += "Jun";
		}
		else if (7 == dataTimeNew.Month) {
			text += "Jul";
		}
		else if (8 == dataTimeNew.Month) {
			text += "Ayg";
		}
		else if (9 == dataTimeNew.Month) {
			text += "Sep";
		}
		else if (10 == dataTimeNew.Month) {
			text += "Oct";
		}
		else if (11 == dataTimeNew.Month) {
			text += "Nov";
		}
		else if (12 == dataTimeNew.Month) {
			text += "Dec";
		}

		text += (" @ " + dataTimeNew.Hour + ":" + dataTimeNew.Minute); 
		//return dataTime.Year + "/" + dataTime.Month + "/" + dataTime.Day + " @ " + dataTime.Hour + ":" + dataTime.Minute;
		return text;
	}

	public static string GetSceneNameByIndex(int index)
	{
		return "level_" + index.ToString ();
	}

	public static string GetNextSceneName(GameMode mode,int currentIndex)
	{
		if (mode == GameMode.Story) {
			return GetSceneNameByIndex (currentIndex+1);
		} else {
			int random = UnityEngine.Random.Range (1,6);
			Debug.Log ("random:" + random.ToString());
			if (random == currentIndex) {
				var newIndex = currentIndex + 1;
				newIndex %= 6;
				if (newIndex == 0)
					newIndex = 1;
				
				return GetSceneNameByIndex (newIndex);
			} else {
				return GetSceneNameByIndex (random);
			}
		}
	}
}
