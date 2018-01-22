#define HIGHLORD_PLAY_IN_SWITCHING_ANIMATION
#define HIGHLORD_SWITCH_LEVEL_ASYNC

using System.Collections;
using BeatsFever.UnityEx;
using UnityEngine;
using BeatsFever.ResMgr;

namespace BeatsFever.GameState
{
    public class SwitchingState : GameState
    {
        private const string SwitchingClipName = "Switching";

        private LevelListener listener;
        private static GameObject maskObj;
        private bool doneLoaded;
        private bool doneFadeIn;
        private bool startedFadeOut;

        private float startLoadingTime;
        private static Animation maskAnimation;
        private bool manualClose;

        private int targetStateId;
        private GameStateParam targetSceneParam;

        public override bool NeedSwitchingScene
        {
            get { return false; }
        }

        public SwitchingState(int id) : base(id)
        {
        }

		static public bool IsMaskActive()
		{
			if(maskObj == null)
			{
				return false;
			}

			if(maskObj.activeSelf)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
        
        public override void Enter(GameStateParam param)
        {
            Debug.Log("Entering switching scene.");
            startLoadingTime = Time.realtimeSinceStartup;

            //App.Game.LogicSceneMgr.OperationMgr = null;

            var stateParam = (SwitchingStateParam)param;
            this.targetStateId = stateParam.TargetStateId;
            this.targetSceneParam = stateParam.TargetSceneParam;
            doneLoaded = false;
            doneFadeIn = false;
            startedFadeOut = false;
            if (maskObj != null)
            {
                GameObject.Destroy(maskObj);
            }

            var maskPrefab = App.ResourceMgr.LoadRes(stateParam.PrefabPath);
            if (maskPrefab != null)
            {
                maskObj = (GameObject)GameObject.Instantiate(maskPrefab);
                //maskAnimation = maskObj.transform.GetComponentsInChildren<Animation>(true)[0];
                GameObject.DontDestroyOnLoad(maskObj);
				maskObj.transform.position = DeviceContainer.Instance.Eye.transform.position;

                //maskAnimation.Play(SwitchingClipName);
            }
        }

        
        public override void Update()
        {
            if (!startedFadeOut)
            {
                if (!doneFadeIn)
                {
                    doneFadeIn = true;
                    if (targetSceneParam.Scene != null)
                    {
                        listener = new LevelListener();
                        listener.callback = OnLevelLoaded;

#if HIGHLORD_SWITCH_LEVEL_ASYNC
                        App.LevelMgr.LoadLevel(targetSceneParam.Scene, listener);
#else
                        float startLoadLevelTime = Time.realtimeSinceStartup;
                        Application.LoadLevel(TargetSceneName);
                        Logger.Log("Load Level elapsed time: " + (Time.realtimeSinceStartup - startLoadLevelTime));
                        OnLevelLoaded(TargetSceneName);
#endif
                    }
                    else
                    {
                        doneLoaded = true;  // nothing to load
                    }
                }
                else if (doneLoaded)
                {
                    if (!startedFadeOut)
                    {
                       	//Debug.Log("Elapsed time until start fade-out: " + (Time.realtimeSinceStartup - startLoadingTime));

                        App.Game.GameStateMgr.SwitchTo(targetStateId, targetSceneParam, false);
                        startedFadeOut = true;
                    }
                }
            }
        }
        
        public override void Exit()
        {
           	//Debug.Log("Scene switching time: " + (Time.realtimeSinceStartup - startLoadingTime));

            if(!manualClose)
            {
				if (null != maskObj) {
					maskObj.StartCoroutineExt(ExitCoroutine());				
				}
            }
        }

        private static IEnumerator ExitCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            GameObject.Destroy(maskObj);
        }

        private void OnLevelLoaded(string nameScene)
        {
            //Debug.Log("Elapsed time until OnLevelLoaded: " + (Time.realtimeSinceStartup - startLoadingTime));
            doneLoaded = true;
        }

        public static void CloseSwitchingManually()
        {
            if(null != maskObj)
            {
                GameObject.Destroy(maskObj);
            }
        }
    }
}
