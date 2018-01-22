using System;
using System.Collections.Generic;

namespace BeatsFever.GameState
{
    public abstract class GameStateParam
    {
        public string Scene;
    }

    class SwitchingStateParam : GameStateParam
    {
        public enum SwitchingStyle : int
        {
           Gate = 0,
           Cloud,
           Normal,
           Fan,
        }

        private readonly string[] SwitchingPrefabs = new string[] {
            "SceneSwitchingPrefab",  // abadoned, refer to fan effect as fallback
			"SceneSwitchingPrefab",  // abadoned, refer to fan effect as fallback
			"SceneSwitchingPrefab",  // abadoned, refer to fan effect as fallback
			"SceneSwitchingPrefab",  // abadoned, refer to fan effect as fallback
		};

        public string PrefabPath
        {
            get
            {
                return SwitchingPrefabs[(int)Style];
            }
        }

        public SwitchingStyle Style = SwitchingStyle.Gate;
        public int TargetStateId;
        public GameStateParam TargetSceneParam;
    }
    
    class BootStateParam : GameStateParam
    {

    }

	class LobbyStateParam : GameStateParam
	{
		public int LastLevelId;
	}

	class BeatStateParam : GameStateParam
	{
		public string LevelName;
	}

	class RoomMiddleStateParam : GameStateParam
	{
		public string NextLevelName;
	}
}
