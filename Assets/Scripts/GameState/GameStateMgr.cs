using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatsFever.GameState
{
    public class GameStateMgr
    {
        private GameState activeState;
        private int oldActiveStateID;
        private Dictionary<int, GameState> stDic = new Dictionary<int, GameState>();
        
        public GameState ActiveState { get { return this.activeState; } }

        public int OldActiveState { get { return this.oldActiveStateID; } }
        
        public void Start()
        {
   
        }
   
        public void Update()
        {
            if(activeState != null)
            {
                activeState.Update();
            }
        }
   
        public void OnGUI()
        {
            if(activeState != null)
            {
                activeState.OnGUI();
            }
        }
        
        public void ShutDown()
        {
            if(activeState != null)
            {
                activeState.Exit();
                activeState = null;
            }

            foreach (GameState Gst in stDic.Values)
            {
                Gst.Release();
            }
            
            stDic.Clear();
        }
        
        public void Register(GameState St)
        {
            if(St == null)
            {
                return;
            }
            
            if(stDic.ContainsKey(St.ID))
            {
                Debug.LogError("St Duplicate:" + St.ID);
                return;
            }
    
            St.SetMgr(this);
            stDic.Add(St.ID, St); 
        }
    
        public void UnRegister(int Id)
        {
            if(!stDic.ContainsKey(Id))
            {
                return;
            }
    
            stDic[Id].SetMgr(null);
            stDic.Remove(Id);
        }
    
        public GameState GetState(int Id)
        {
            if(!stDic.ContainsKey(Id))
            {
                return null;
            }
            
            return stDic[Id];
        }

        public void SwitchTo(int next, GameStateParam param, bool autoSwitch = true)
        {
            if(!stDic.ContainsKey(next))
            {
                Debug.LogError("invalid next st: " + next);
                return;
            }

            var nextState = stDic[next];
            if (autoSwitch)
            {
                if (nextState.NeedSwitchingScene && param.Scene != null)
                {
                    ForceSwitch(next, param);
                    return;
                }
            }

            if (activeState != null && activeState.ID == next)
            {
                return;
            }
            
            if(activeState != null)
            {
                oldActiveStateID = activeState.ID;
                activeState.Exit();
            }

            activeState = nextState;
    
            if(activeState != null)
            {
                activeState.Enter(param);

                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        public void ForceSwitch(int next, GameStateParam param)
        {
            var switchParam = new SwitchingStateParam();
            switchParam.TargetStateId = next;
            switchParam.TargetSceneParam = param;
            switchParam.Style = SwitchingStateParam.SwitchingStyle.Fan;
            SwitchTo(GameStateID.ST_Switching, switchParam);
        }
    }
}