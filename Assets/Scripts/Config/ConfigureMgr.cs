using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BeatsFever.Config;

namespace BeatsFever
{
    public class ConfigureMgr
    {
		private StoryConfig storyConfig = new StoryConfig();
		private SceneEventConfig sceneEventConfig = new SceneEventConfig();

        public void InitGameConfig()
        {
			storyConfig.LoadStoryConfig ();
        }


		public StoryConfig GetStoryConfig()
		{
			return storyConfig;
		}

		public Dictionary<int,SceneEventData> GetSceneEventDatas(string musicName)
		{
			return sceneEventConfig.GetSceneEventDatas (musicName);
		}
    }
}