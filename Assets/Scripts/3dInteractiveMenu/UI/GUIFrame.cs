using UnityEngine;
using System.Collections.Generic;
using BeatsFever.ResMgr;

namespace BeatsFever.UI
{
    public enum ENeedResBarType
    {
        eNotNeed,
        eNeed,
        eIgnore,
    }

    public class GUIFrame
    {
        protected int id;
        protected bool active;
        protected GameObject root;
        protected bool isModal = true;
        protected bool ignoreUserInput;
        protected bool needBlurScene = true;
		protected bool haveShowAnimation = true;
        protected int needsRefreshPanelInFrame = 2;   // Set panel's dirty flag in 2 frames after activating to force rebuild drawcall after grid layout
		protected bool closeAniDone;
        protected ENeedResBarType needResBarType = ENeedResBarType.eNotNeed;
		protected bool ignoreShowAniOnce;
        protected bool pauseLogicSceneManager = true;

		private FrameBaseUI baseUIScript;
        private bool warm;
        private int refreshPanelInFrame;

        public GUIFrame(int id)
        {
            this.id = id;
        }

        public int ID
        {
            get { return id; }
        }

        public bool IsActive
        {
            get { return active; }
        }

        public bool IsModal
        {
            get { return isModal; }
        }

		public FrameBaseUI GetWrapperScript()
		{
			return baseUIScript;
		}

        //public bool IsNeedResBar { get { return needResBar; } }
        public ENeedResBarType NeedResBarType
        {
            get { return needResBarType; }
        }

        public bool HaveShowAni
        {
            get { return haveShowAnimation; }
        }

        public bool PauseLogicSceneManager
        {
            get { return pauseLogicSceneManager; }
        }

        public bool IgnoreShowAniOnce
        {
            get { return ignoreShowAniOnce; }
            set { ignoreShowAniOnce = value; }
        }

        public bool IsCloseAniDone
        {
            get { return closeAniDone; }
            set { closeAniDone = value; }
        }

        public bool NeedBlurScene
        {
            get
            {
                return needBlurScene && isModal; // Non-modal UI should never be blured.
            }
        }

        public GUIFrameMgr Owner { get; set; }

        public bool IgnoreUserInput
        {
            get { return ignoreUserInput; }
            set
            {
                ignoreUserInput = value;

                var camera = root.GetComponentInChildren<UICamera>();
                if (camera != null)
                {
                    camera.enabled = !IgnoreUserInput;
                }
            }
        }

        public GameObject Root
        {
            get { return root; }
            private set
            {
                if (root == value)
                {
                    return;
                }

                if (root != null)
                {
					GameObject.Destroy(root);
					//NGUITools.Destroy(root);
                }

                root = value;
                if (root != null)
                {
					root.SetActive (active);
					//NGUITools.SetActive(root,active);
                }
            }
        }

        public void PlayCloseAnimation()
        {
			baseUIScript.FadeOut ();
        }

        public void SetPosition(Vector3 pos)
        {
            if (null == root)
            {
				Debug.LogError("GUI root not exist");
                return;
            }

            root.transform.position = pos;
        }

        public Vector3 GetPosition()
        {
            if (root != null)
            {
                return root.transform.position;
            }

            return Vector3.zero;
        }

        public virtual void Release()
        {
            DeActive();
        }

        public virtual void Active()
        {
            TestLoad();

            if (root == null)
            {
				Debug.LogError("GUIFrame root is null! Type: " + GetType());
                return;
            }

            if (active)
            {
                return;
            }
            else
            {
				root.SetActive (true);

				//NGUITools.SetActive(root,true);
				root.SetActive (true);
				baseUIScript.FadeIn ();
                active = true;
            }

            if (isModal)
            {
                
            }

            refreshPanelInFrame = needsRefreshPanelInFrame;
        }

		public void PlayActiveSound()
		{
			//App.SoundMgr.Play(LittleEmpire2.Base.Sound.AudioResources.clickbutton);
		}

        public void TestLoad()
        {
            if (root == null)
            {
                LoadPrefab();
            }
        }

        public virtual void DeActive()
        {
            if (root != null)
            {
				GameObject.Destroy(root);
                //NGUITools.Destroy(root);
                root = null;
            }

            active = false;
        }
			
        //do not invoke it directly
        internal virtual void RegisterHandlers(GUIMsgMgr mgr)
        {
            if (mgr == null)
            {
				Debug.Log("msg mgr is null");
            }
        }

        internal virtual void UnRegisterHandlers(GUIMsgMgr mgr)
        {
            if (mgr == null)
            {
				Debug.Log("msg mgr is null");
            }
        }
//
//        public virtual void Update()
//        {
//            if (!active)
//            {
//                return;
//            }
//
//            #region For new NGUI version, we need to set panel dirty manually after it is activated to force recalculation of panel DC.
//
//            if (refreshPanelInFrame > 0)
//            {
//                if (--refreshPanelInFrame == 0)
//                {
//                    if (root != null)
//                    {
//                        NGUITools.ExecuteAll<UIPanel>(root, "SetDirty");
//                    }
//                }
//            }
//
//            #endregion
//        }

        public virtual void CallWapperFunction(string funName, object param = null)
        {
            if (null == Root)
            {
				Debug.LogWarning("CallWapperFunction -- Root not found! " + GetType() + "  " + funName);
                return;
            }

            if (!Root.activeSelf)
            {
				root.SetActive (true);

                //NGUITools.SetActive(root, true);
            }

            if (null == param)
            {
                Root.SendMessage(funName, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Root.SendMessage(funName, param, SendMessageOptions.RequireReceiver);
            }
        }

        public void WarmPrefab()
        {
            if (!warm)
            {
                GUIFrameBuilder.FrameGoInfo info = App.Game.GUIFrameMgr.GUIFrameBuilder.GetGuiAssetInfo(id);
                App.ResourceMgr.LoadRes(info.asset);

                warm = true;
            }
        }

        private void LoadPrefab()
        {
            GameObject root = InitiateObj();
            GUIFrameBuilder.TransformLanguage(root);
            Root = root;
			baseUIScript = Root.GetComponent<FrameBaseUI> ();
			baseUIScript.Init (this);
        }

        private GameObject InitiateObj()
        {
            GUIFrameBuilder.FrameGoInfo info = App.Game.GUIFrameMgr.GUIFrameBuilder.GetGuiAssetInfo(id);

            Object obj = App.ResourceMgr.LoadRes(info.asset);
            if (null == obj)
            {
				Debug.LogWarning("can not load prefab, frame id: " + id);
                return null;
            }

            return Object.Instantiate(obj) as GameObject;
        }
    }
}
