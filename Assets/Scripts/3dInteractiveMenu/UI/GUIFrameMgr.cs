using System;
using System.Collections.Generic;
using UnityEngine;
using BeatsFever.GameState;
namespace BeatsFever.UI
{
    public class GUIFrameMgr
    {
        private class FrmsInfo
        {
            public List<int> Frms = new List<int>();
        }

        private GUIMsgMgr msgMgr = new GUIMsgMgr();
        private GUIFrameBuilder frameBuilder = new GUIFrameBuilder();
        private Dictionary<int, GUIFrame> frameDic = new Dictionary<int, GUIFrame>();
        private List<int> activeList = new List<int>();
        private List<int> activeQueue = new List<int>();
        private List<int> deActiveQueue = new List<int>();
        private List<int> frameNeedRestore = new List<int>();
        private Stack<FrmsInfo> frameNeedRestoreStack = new Stack<FrmsInfo>();
        private UnityEngine.Camera mainAssetBarCamera;
        private List<int> ignorePopFrameIdList = new List<int>();
        public List<int> additionNeedPopFrames = new List<int>();

        public GUIMsgMgr GUIMsgMgr
        {
            get { return msgMgr; }
        }

        public GUIFrameBuilder GUIFrameBuilder
        {
            get { return frameBuilder; }
        }

        public void Start()
        {
            msgMgr.Start();
        }

        public void ShutDown()
        {
            foreach(int id in activeList)
            {
                frameDic[id].DeActive();
            }
            activeList.Clear();

            foreach(GUIFrame frame in frameDic.Values)
            {
                frame.Release();
            }
            frameDic.Clear();

            msgMgr.Shutdown();
        }

        public void CloseAllActiveFrm()
        {
            foreach(int id in activeList)
            {
				frameDic[id].DeActive();
            }

            activeList.Clear();
        }

        public void Update()
        {
            if(SyncActives())
            {
                UpdateActiveList();
            }
            msgMgr.Update();

//            int count = activeList.Count;
//            if (count > 0)
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    int id = activeList[i];
//                    frameDic[id].Update();
//                }
//            }
        }

        public void Register(GUIFrame frame)
        {
            if(frame == null)
            {
                return;
            }
            if(frameDic.ContainsKey(frame.ID))
            {
                return;
            }

            frame.Owner = this;
            frame.RegisterHandlers(msgMgr);
            //frame.WarmPrefab();
            frameDic.Add(frame.ID, frame);
        }

        public void UnRegister(int id)
        {
            if(frameDic.ContainsKey(id))
            {
                if(activeList.Contains(id))
                {
                    frameDic[id].DeActive();
                    activeList.Remove(id);
                }

                frameDic[id].UnRegisterHandlers(msgMgr);
                frameDic[id].Owner = null;
                frameDic.Remove(id);
            }
        }

        public bool IsRegister(int id)
        {
            return frameDic.ContainsKey(id);
        }

        public GUIFrame GetFrame(int id)
        {
            if(!frameDic.ContainsKey(id))
            {
                return null;
            }
            return frameDic[id];
        }

        public void AcitveRestore()
        {
            foreach(int Id in frameNeedRestore)
            {
                Active(Id);
            }

            frameNeedRestore.Clear();
        }

        public void PushOneAdditionFrameToRestoreStack(int frameId)
        {
            additionNeedPopFrames.Add(frameId);
        }

        public void SetOneIgnorePopFrameId(int frameId)
        {
            ignorePopFrameIdList.Add(frameId);
        }

        public void PushStoreFrame()
        {
            FrmsInfo Info = new FrmsInfo();

            List<int> Frms = new List<int>();
            foreach(int Id in activeList)
            {
                if(deActiveQueue.Contains(Id)) 
                {
                    continue;
                }
				if(Id != GUIFrameID.NetWaitUIFrame)
				{
					Frms.Add(Id);
                }
            }

			if(0 == Frms.Count)
			{
				return;
			}

            Info.Frms = Frms;

            frameNeedRestoreStack.Push(Info);
        }

        public void ClearFrameNeedRestoreStack()
        {
            frameNeedRestoreStack.Clear();
        }

		public void PopStoreFrame()
        {
            if(frameNeedRestoreStack.Count == 0 && additionNeedPopFrames.Count == 0)
            {
                return;
            }


			FrmsInfo Info = frameNeedRestoreStack.Pop();
            if(Info != null)
            {
                foreach(int frameId in Info.Frms)
                {
                    if(ignorePopFrameIdList.Contains(frameId))
                    {
                        continue;
                    }
                    
                    //GetFrame(frameId).IgnoreShowAniOnce = true;
					Active(frameId);
                }
            }

            foreach(int frameId in additionNeedPopFrames)
            {
                if(ignorePopFrameIdList.Contains(frameId))
                {
                    continue;
                }
				Active(frameId);
            }

            ignorePopFrameIdList.Clear();
            additionNeedPopFrames.Clear();

            ClearFrameNeedRestoreStack();
        }

        public GUIFrame Active(int id)
        {
            GUIFrame frame;
			frameDic.TryGetValue(id, out frame);

            if(frame == null)
            {
				Debug.LogError("not registered frame: " + id);
                return null;
            }

            if(deActiveQueue.Contains(id))
            {
                frameDic[id].DeActive(); // The UI should be deactivated first. Then it can be activated again.
                deActiveQueue.Remove(id);
                activeList.Remove(id);
            }

            if(!activeList.Contains(id))
            {
                activeQueue.Add(id);
                frame.TestLoad();
            }

            return frame;
        }

        public void DeActive(int id, bool forceWithOutAni = false)
        {
            GUIFrame frame = GetFrame(id);
            if (frame == null)
            {
                return;
            }

            if(!forceWithOutAni && frame.HaveShowAni && !frame.IsCloseAniDone)
            {
                frame.PlayCloseAnimation();
                return;
            }

            if(!frameDic.ContainsKey(id))
            {
				Debug.Log("not registered frame " + id);
                return;
            }

            if(!activeList.Contains(id))
            {
                if(activeQueue.Contains(id))
                {
                    activeQueue.Remove(id);
                }

                return;
            }

            if(!deActiveQueue.Contains(id))
            {
                deActiveQueue.Add(id);
            }
        }

        public bool IsActive(int id)
        {
            return activeList.Contains(id) || activeQueue.Contains(id);
        }

		public bool IsOtherFrmActive()
		{
			bool Ret = false;
			foreach(int Id in activeList)
			{
				if(Id != GUIFrameID.NetWaitUIFrame)
				{
					Ret = true;
					break;
				}
			}
			return Ret;
		}
		

        public GUIFrame GetTopMostModelFrame()
        {
            if (activeList.Count > 0)
            {
                for (int i = activeList.Count - 1; i >= 0; i--)
                {
                    var frame = frameDic[activeList[i]];
                    if (frame.IsModal)
                    {
                        return frame;
                    }
                }
            }

            return null;
        }
			

        private bool SyncActives()
        {
            bool changed = false;

            if (deActiveQueue.Count > 0)
            {
                var waitToDeactive = deActiveQueue.ToArray();
                foreach (int id in waitToDeactive)
                {
                    deActiveQueue.Remove(id);
                    if (activeList.Remove(id))
                    {
                        if (frameDic[id].IsActive)
                        {
                            frameDic[id].DeActive();
                        }

                        changed = true;
                    }
                }
            }

            if(activeQueue.Count > 0)
            {
                var waitToActive = activeQueue.ToArray();
                foreach(int id in waitToActive)
                {
                    activeQueue.Remove(id);

                    if(!activeList.Contains(id))
                    {
                        var frame = frameDic[id];
                        if (activeList.Count > 0 && activeList[activeList.Count - 1] == GUIFrameID.NetErrorDialogFrame)
                        {
                            // If the last frame is NetErrorDialogFrame, should not active any other frame.
                            frame.DeActive();
                            continue;
                        }

                        activeList.Add(id);

                        if(!frame.IsActive)
                        {
                            // Should set the depth right before active. Otherwise the blur sequence might be wrong.
                            frame.Active();
                        }

                        changed = true;
                    }
                }
            }

            return changed;
        }

        private void UpdateActiveList()
        {
            foreach(int id in activeList)
            {
                GUIFrame frame = frameDic[id];
                if(null == frame.Root)
                {
					//frame root already destoryed for some reason.
					DeActive(id);
                    continue;
                }

				frame.SetPosition(DeviceContainer.Instance.GetShieldGlassPosition());
            }
        }
    }
}
