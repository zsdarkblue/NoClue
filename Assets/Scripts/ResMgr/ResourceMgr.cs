using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatsFever.ResMgr
{
    public class ResourceMgr
    {
        public static string InpackBundlePath;
        public static string LocalResPath;
        public static string LocalBundlePath;
        public static string RemoteBundlePath;
        public static string UserSettingFile;
		public static string RussiaShareFile;

        private static string bundleUrl0, bundleUrl1;
        private static int nextBundelUrlIndex;


        public static string BundleToRemoteUrl(string bundleName)
        {
            return RemoteBundlePath + bundleName;
        }

        public static void SetRemotePath(string url0, string url1)
        {
			bundleUrl0 = url0;//"ftp://192.160.1.200/";//url0;
			bundleUrl1 = url1;//"ftp://192.160.1.200/";//
            nextBundelUrlIndex = 0;

            ChangeRemotePath();
        }

        public static void ChangeRemotePath()
        {
            string url = nextBundelUrlIndex == 0 ? bundleUrl0 : bundleUrl1;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                case RuntimePlatform.OSXWebPlayer:
                {
                    RemoteBundlePath = url + "webplayer/";
                    break;
                }
                case RuntimePlatform.IPhonePlayer:
                {
                    RemoteBundlePath = url + "iphone/";
                    break;
                }
                case RuntimePlatform.Android:
                {
                    RemoteBundlePath = url + "android/";
                    break;
                }
                default:
                {
                    RemoteBundlePath = url + "webplayer/";
                    break;
                }
            }

            nextBundelUrlIndex++;
            if (nextBundelUrlIndex > 1)
            {
                nextBundelUrlIndex = 0;
            }
        }

        public static void InitLocalPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                {
                    LocalResPath = Application.dataPath + "/../res/";

                    break;
                }
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                {
                    LocalResPath = Application.dataPath + "/../res/";

                    break;
                }
                case RuntimePlatform.WindowsWebPlayer:
                case RuntimePlatform.OSXWebPlayer:
                {
                    LocalResPath = Application.dataPath + "/../res/";

                    break;
                }
                case RuntimePlatform.IPhonePlayer:
                {
                    LocalResPath = Application.persistentDataPath + "/";

                    break;
                }
                case RuntimePlatform.Android:
                {
                    LocalResPath = Application.persistentDataPath + "/";

                    break;
                }
                default:
                {
                    LocalResPath = Application.dataPath + "/";
                    break;
                }
            }

            LocalBundlePath = "file://" + LocalResPath;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    InpackBundlePath = Application.streamingAssetsPath + "/";
                    break;
                }
                default:
                {
                    InpackBundlePath = "file://" + Application.streamingAssetsPath + "/";
                    break;
                }
            }
        }

        public void Start()
        {

        }

        public void Shutdown()
        {

        }

        public void ClearAllCachedAsset()
        {

        }

        public Object LoadRes(string resName)
        {
            return Resources.Load(resName);
        }
    }
}