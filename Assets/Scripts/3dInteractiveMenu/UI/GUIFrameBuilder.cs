using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using BeatsFever.ResMgr;
using BeatsFever;

namespace BeatsFever.UI
{
    public class GUIFrameBuilder
    {
        public delegate void LoadFinishCallback(string name,int id);

        private const string i18nPrefix = "i#";
    
        public class FrameGoInfo
        {
            public string asset;

            public FrameGoInfo(string asset)
            {
                this.asset = asset;
            }
        }

        private Dictionary<int, FrameGoInfo > assetDic = new  Dictionary<int, FrameGoInfo>();

        public void Start()
        {
        }

        public void Shutdown()
        {
            assetDic.Clear();
        }
    
        public void RegisterGuiAsset(int id, string asset)
        {
            if (assetDic.ContainsKey(id))
            {
				Debug.LogError("duplicate id when RegisterGuiAsset, frame name: " + id + "  asset name: " + asset);
                return;
            }

            assetDic.Add(id, new FrameGoInfo(asset));
        }
        
        public void BuildGuiFrame<T>(int id) where T : GUIFrame
        {
            if(App.Game.GUIFrameMgr.IsRegister(id))
            {
				Debug.LogWarning("duplicate create frame: " + id);
                return;
            }
            
            var args = new object[] { id };
            T frame = Activator.CreateInstance(typeof(T), args) as T;
            App.Game.GUIFrameMgr.Register(frame);
        }
        
        public FrameGoInfo GetGuiAssetInfo(int id)
        {
            if(!assetDic.ContainsKey(id))
            {
				Debug.LogWarning("can not find GetGuiAssetInfo: " + id);
                return null;
            }
            return assetDic[id];
        }

        public static void TransformLanguage(GameObject root)
        {
			if(root == null)
			{
				return;
			}

			UILabel[] labels = root.GetComponentsInChildren<UILabel>(true);
			foreach (UILabel label in labels)
			{
				if(label.name.StartsWith(i18nPrefix))
				{   
					label.text = MultilanguageMgr.GetMutiText(label.name.Substring(i18nPrefix.Length));
				}
			}
        }
    }
}