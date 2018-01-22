using System;
using System.Collections;
using BeatsFever.UnityEx;
using UnityEngine;

namespace BeatsFever.ResMgr
{
    public class LevelListener
    {
        public delegate void OnLevelLoaded(string nameScene);

        public OnLevelLoaded callback;
    }

    public class LevelMgr
    {
        private bool loadingScene;

        public void LoadLevel(string sceneName, LevelListener listener)
        {
            if (loadingScene)
            {
                Debug.LogWarning("other scene is loading...");
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("scene name is null");
                return;
            }
            if (listener == null)
            {
                Debug.LogWarning("listener is null: " + listener);
                return;
            }

            loadingScene = true;
			#if !Steamworks_Off
			SteamVR_LoadLevel.Begin (sceneName);
			#endif
			DeviceContainer.Instance.ChangeCamera (sceneName);
            App.MainGo.StartCoroutineExt(Execute(sceneName, listener));
        }

        public static AsyncOperation LoadAdditiveAsync(string scene)
        {
			return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (scene);
        }
        
        private IEnumerator Execute(string sceneName, LevelListener listener)
        {
			#if !Steamworks_Off
			while (SteamVR_LoadLevel.loading) {
			yield return null;
			}
			#else
			yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (sceneName);
			#endif

            App.Clean();
            listener.callback(sceneName);
            loadingScene = false;
        }
    }
}