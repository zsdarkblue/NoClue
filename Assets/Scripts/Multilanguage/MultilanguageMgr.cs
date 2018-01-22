using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using System.IO;
using System;


namespace BeatsFever
{
    public class MultilanguageMgr
    {
        public enum LanguageID : int
        {
            Undefined = -2,
            Auto = -1,
            zh_CN = 0,
            zh_TW = 1,
            en_US = 2,
            ru = 3,
			th = 4,
            fr = 5,
            ger = 6,
			kor = 7,
            es = 8,
            it = 9,
            dk = 10,
            pt = 11,
            tr = 12,
            ae = 13,
            ir = 14,
            id = 15,
            my = 16,
            vn = 17,
            fi = 18,
            jp = 19,
        }

        public static int CurrentLanguageIndex { get; private set; }
        public static LanguageID CurrentLanguageID { get { return (LanguageID)CurrentLanguageIndex; } }
        public static Font PreferredFont { get; private set; }

        private const string LanguageIDPrefKey = "LanguageID";
        private const string FontResourcePathPrefix = "Font/FZPHTFW_";
        private const LanguageID FallbackLanguageID = LanguageID.en_US;


        static MultilanguageMgr()
        {
            CurrentLanguageIndex = -1;

            systemLanguageDict = new Dictionary<SystemLanguage, LanguageID>();
            systemLanguageDict[SystemLanguage.Arabic] = LanguageID.ae;
            systemLanguageDict[SystemLanguage.English] = LanguageID.en_US;
            systemLanguageDict[SystemLanguage.Korean] = LanguageID.kor;
            systemLanguageDict[SystemLanguage.Russian] = LanguageID.ru;
            systemLanguageDict[SystemLanguage.Thai] = LanguageID.th;
            systemLanguageDict[SystemLanguage.French] = LanguageID.fr;
            systemLanguageDict[SystemLanguage.German] = LanguageID.ger;
            systemLanguageDict[SystemLanguage.Spanish] = LanguageID.es;
            systemLanguageDict[SystemLanguage.Italian] = LanguageID.it;
            systemLanguageDict[SystemLanguage.Finnish] = LanguageID.fi;
            systemLanguageDict[SystemLanguage.Danish] = LanguageID.dk;
            systemLanguageDict[SystemLanguage.Portuguese] = LanguageID.pt;
            systemLanguageDict[SystemLanguage.Turkish] = LanguageID.tr;
            systemLanguageDict[SystemLanguage.Indonesian] = LanguageID.id;
            systemLanguageDict[SystemLanguage.Vietnamese] = LanguageID.vn;
			systemLanguageDict[SystemLanguage.Japanese] = LanguageID.jp;
			systemLanguageDict[SystemLanguage.Chinese] = LanguageID.zh_CN;

			//test
			//PlayerPrefs.SetInt(LanguageIDPrefKey, (int)LanguageID.Auto);
        }


        private static string[] Languages = 
        {
            "string_Simplified_CN",   // 0
            "string_Traditional_CN",  // 1
            "string_EN",              // 2
            "string_RU",              // 3
			"string_TH",              // 4
            "string_FR",              // 5
            "string_DE",              // 6
			"string_KOR",             // 7
            "string_ES",              // 8
            "string_IT",              // 9
            "string_DK",              // 10
            "string_PT",              // 11
            "string_TR",              // 12
            "string_AE",              // 13
            "string_IR",              // 14
            "string_ID",              // 15
            "string_MY",              // 16
            "string_VN",              // 17
            "string_FI",              // 18
            "string_JP",              // 19
        };


        private static Dictionary<string, string> languageDict = new Dictionary<string, string>();
        private static Dictionary<LanguageID, string> languageNameDict;
        private static Dictionary<SystemLanguage, LanguageID> systemLanguageDict;


        public static string GetLanguageName(LanguageID languageID)
        {
            if (languageNameDict == null)
            {
                languageNameDict = new Dictionary<LanguageID, string>();
                languageNameDict[LanguageID.en_US] = MultilanguageMgr.GetMutiText("text_1799");
                languageNameDict[LanguageID.fr] = MultilanguageMgr.GetMutiText("text_1800");
                languageNameDict[LanguageID.ger] = MultilanguageMgr.GetMutiText("text_1801");
                languageNameDict[LanguageID.ae] = MultilanguageMgr.GetMutiText("text_1923");   // alabo
                languageNameDict[LanguageID.ir] = MultilanguageMgr.GetMutiText("text_1924");   // bosi
                languageNameDict[LanguageID.dk] = MultilanguageMgr.GetMutiText("text_1925");   // danmai
                languageNameDict[LanguageID.fi] = MultilanguageMgr.GetMutiText("text_1926");   // fenlan
                languageNameDict[LanguageID.my] = MultilanguageMgr.GetMutiText("text_1927");   // malai
                languageNameDict[LanguageID.pt] = MultilanguageMgr.GetMutiText("text_1928");   // putaoya
                languageNameDict[LanguageID.tr] = MultilanguageMgr.GetMutiText("text_1929");   // tuerqi
                languageNameDict[LanguageID.es] = MultilanguageMgr.GetMutiText("text_1930");   // xibanya
                languageNameDict[LanguageID.it] = MultilanguageMgr.GetMutiText("text_1931");   // yidali
                languageNameDict[LanguageID.id] = MultilanguageMgr.GetMutiText("text_1932");   // yinni
                languageNameDict[LanguageID.vn] = MultilanguageMgr.GetMutiText("text_1933");   // yuenan
                languageNameDict[LanguageID.zh_CN] = MultilanguageMgr.GetMutiText("text_1935");  // jiantizhongwen
                languageNameDict[LanguageID.zh_TW] = MultilanguageMgr.GetMutiText("text_1936");  // fantizhongwen
                languageNameDict[LanguageID.th] = MultilanguageMgr.GetMutiText("text_1937");   // taiguo
                languageNameDict[LanguageID.ru] = MultilanguageMgr.GetMutiText("text_1938");   // eluosi
                languageNameDict[LanguageID.kor] = MultilanguageMgr.GetMutiText("text_1939");  // hanguo
                languageNameDict[LanguageID.jp] = MultilanguageMgr.GetMutiText("text_2006");   // riben
            }

            string languageName = "";
            return languageNameDict.TryGetValue(languageID, out languageName) ? languageName : languageID.ToString();
        }

		public static int GetZoneLanguageID()
		{
			if(SystemLanguage.Chinese == Application.systemLanguage)
			{
				return 0;
			}
			else
			{
				return 2;
			}
		}

        private static LanguageID GetSystemLanguageID()
        {
            LanguageID languageID = LanguageID.en_US;
            // fix for 3.3.5:
#if HIGHLORD_ENABLE_AUTO_LANGUAGE_SELECTION
            Logger.LogYellow("Detect system language: " + Application.systemLanguage);
            if (!systemLanguageDict.TryGetValue(Application.systemLanguage, out languageID))
            {
                languageID = LanguageID.en_US;
            }
#endif
            return languageID;
        }


        private const string LanguageDirectoryName = "Lang";


        public static List<int> GetLocallyExistedLanguageSet()
        {
            var langSet = new List<int>();
            foreach (var langID in Enum.GetValues(typeof(LanguageID)))
            {
                int langIndex = (int)(LanguageID)langID;
                if (langIndex >= 0)
                {
                    if (   TryToLoadLanguageAsset(langIndex) != null
                        || HasLanguageBaseFile((LanguageID)langID))

                    {
                        langSet.Add(langIndex);
                    }
                }
            }

            return langSet;
        }


        public static string GetLanguageBaseFilePath(LanguageID languageID)
        {
            return Path.Combine(Path.Combine(Application.persistentDataPath, LanguageDirectoryName), 
                                "Lang_" + System.Enum.GetName(typeof(MultilanguageMgr.LanguageID),
                                                              languageID));
        }


        public static bool HasLanguageBaseFile(LanguageID langID)
        {
            return File.Exists(GetLanguageBaseFilePath(langID));
        }
        

//        public static PBConf DeserializeConfigBytes(byte[] bytes)
//        {
//            PBConf conf = null;
//            using (var stream = new MemoryStream(bytes))
//            {
//                try
//                {
//                    var configSerializer = new ConfigSerializer();
//                    conf = (PBConf)configSerializer.Deserialize(stream, null, typeof(PBConf));
//                }
//                catch (System.Exception ex)
//                {
//                    Logger.LogError("Error on deserialize language configuration bytes: " + ex.Message);
//                    conf = null;
//                }
//            }
//            
//            return conf;
//        }


        public static void EnsureLanguageDirectory()
        {
            string path = Path.Combine(Application.persistentDataPath, LanguageDirectoryName);
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (IOException ex)
            {
                //Logger.LogWarning("Failed to create language folder: " + path + ", message: " + ex.Message);
            }
        }


        public static void ResetLanguageIDSetting(LanguageID languageID)
        {
            StoreLanguageIDSetting(languageID);
            Clear();
            InitMultiConf();
        }


        public static void StoreLanguageIDSetting(LanguageID languageID)
        {
			//Debug.LogError("StoreLanguageIDSetting:"+languageID);
            PlayerPrefs.SetInt(LanguageIDPrefKey, (int)languageID);
        }

        public static LanguageID LoadLanguageIDSetting()
        {
			LanguageID languageIDSetting = (LanguageID)PlayerPrefs.GetInt(LanguageIDPrefKey, (int)LanguageID.Auto);
			//Debug.LogError("LoadLanguageIDSetting:"+languageIDSetting);
			if (   languageIDSetting == LanguageID.Auto           // fix for 3.3.5 user local cached data
			    || languageIDSetting == LanguageID.Undefined)
			{
				languageIDSetting = LanguageID.en_US;
			}
			return languageIDSetting;
        }

		public static void TryDoFristLoginLanguageSwitch(List<int> autoL10nSet)
		{
			LanguageID languageIDSetting = (LanguageID)PlayerPrefs.GetInt(LanguageIDPrefKey, (int)LanguageID.Auto);
			if(languageIDSetting == LanguageID.Auto)
			{
				LanguageID languageID = LanguageID.en_US;
				if (systemLanguageDict.TryGetValue(Application.systemLanguage, out languageID))
				{
					if(autoL10nSet.Contains((int)languageID))
					{
						if (MultilanguageMgr.TrySwitchingToLanguage(languageID))
						{
							MultilanguageMgr.StoreLanguageIDSetting(languageID);
						}
						else
						{
							MultilanguageMgr.StoreLanguageIDSetting(LanguageID.en_US);
						}
					}
					else
					{
						MultilanguageMgr.StoreLanguageIDSetting(LanguageID.en_US);
					}
				}
				else
				{
					MultilanguageMgr.StoreLanguageIDSetting(LanguageID.en_US);
				}
			}
		}


        public static LanguageID GetLanguageSettingType()
        {
#if TAIWAN_NAME
            return LanguageID.zh_TW;
#elif LANG_RU
            return LanguageID.ru;
#elif LANG_KOR
			return LanguageID.kor;
#elif LANG_EN
            return LanguageID.en_US;
#elif LANG_ZH_CN
            return LanguageID.zh_CN;
#elif LANG_TH
			return LanguageID.th;
//#elif LANG_UNIVERSAL && !UNITY_IPHONE
//            return LanguageID.en_US;
#else
            var languageID = LoadLanguageIDSetting();
            return languageID == LanguageID.Auto ? GetSystemLanguageID() : languageID;
#endif
        }

//		public static string GetCurMultilangStringCode ()
//		{
//            LanguageID langID = GetLanguageSettingType();
//            string langString = "";
//            if (System.Enum.IsDefined(typeof(config.PBConfStringExtraConf.PBExtraL10n), (PBConfStringExtraConf.PBExtraL10n)langID))
//            {
//                PBConfStringExtraConf.PBExtraL10n langEnum = (PBConfStringExtraConf.PBExtraL10n)langID;
//                langString = System.Enum.GetName(typeof(PBConfStringExtraConf.PBExtraL10n), langEnum);
//            }
//
//            return langString;
//		}
//

        public static void Clear()
        {
            languageDict.Clear();
        }

		private static void LoadPreferredFont()
		{
			string candidateFontName;

#if HIGHLORD_LEVERAGE_SYSTEM_FONT_FOR_ENGLISH
            if (CurrentLanguageID == LanguageID.en_US)
            {
                candidateFontName = "Arial.ttf";
                PreferredFont = Resources.GetBuiltinResource<Font>(candidateFontName);
            }
            else
#endif
            {
                candidateFontName = FontResourcePathPrefix + CurrentLanguageID.ToString();
                PreferredFont = Resources.Load(candidateFontName) as Font;
            }

			//Logger.LogYellow("Try loading candidate font: " + candidateFontName + ", result: " + (PreferredFont == null ? "unavailable" : "available"));
		}

        public static void InitMultiConf()
        {
            //LanguageID currentLanguage = GetLanguageSettingType();
			LanguageID currentLanguage = DeviceContainer.Instance.Language;

			string languageName = "english";
			//Steamworks.SteamApps.GetCurrentGameLanguage ();
	
			if (languageName.Equals ("english")) {
				currentLanguage = LanguageID.en_US;
			} else if (languageName.Equals ("schinese")) {
				currentLanguage = LanguageID.zh_CN;
			} else if (languageName.Equals ("tchinese")) {
				currentLanguage = LanguageID.zh_TW;
			} else if (languageName.Equals ("japanese")) {
				currentLanguage = LanguageID.jp;
			} else if (languageName.Equals ("russian")) {
				currentLanguage = LanguageID.ru;
			} else {
				currentLanguage = LanguageID.en_US;
			}
				
            if (!TrySwitchingToLanguage(currentLanguage))
            {
				Debug.LogWarning(  "Failed to load current language: " + Enum.GetName(typeof(LanguageID), currentLanguage)
                                  + ", fallback to: " + Enum.GetName(typeof(LanguageID), FallbackLanguageID));
                StoreLanguageIDSetting(FallbackLanguageID);
                TrySwitchingToLanguage(GetLanguageSettingType());
            }
        }


        public static bool TrySwitchingToLanguage(LanguageID langID)
        {
            bool success = false;
            int langIndex = (int)langID;

            //Logger.Log("language, index: " + langIndex + "    asset:" + Languages[langIndex]);
            TextAsset asset = TryToLoadLanguageAsset(langIndex);
            if (asset != null)
            {
                ReadMultilanguageFromJson(asset);
                Resources.UnloadAsset(asset);
                success = true;
            }
            else
            {
//                var conf = TryToLoadLanguageBaseFromFile(langID);
//                if (conf != null)
//                {
//                    CurrentLanguageIndex = langIndex;
//                    PerformStringUpdateFromConfig(conf.strExtraConf);
//                    Logger.LogYellow("Loaded from base file.");
//                    success = true;
//                }
            }

            if (success)
            {
                CurrentLanguageIndex = langIndex;
                LoadPreferredFont();
            }

            return success;
        }


        public static TextAsset TryToLoadLanguageAsset(int languageIndex)
        {
            TextAsset asset = null;
            if (languageIndex >= 0 && languageIndex < Languages.Length)
            {
                asset = Resources.Load(Languages[languageIndex]) as TextAsset;
            }
            return asset;
        }


//        public static  PBConf TryToLoadLanguageBaseFromFile(MultilanguageMgr.LanguageID langID)
//        {
//            PBConf conf = null;
//            if (HasLanguageBaseFile(langID))
//            {
//                try
//                {
//                    Logger.LogYellow("Try loading from base file: " + GetLanguageBaseFilePath(langID));
//                    var confBytes = File.ReadAllBytes(GetLanguageBaseFilePath(langID));
//                    if (confBytes != null)
//                    {
//                        conf = DeserializeConfigBytes(confBytes);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Logger.LogWarning(  "Failed to deserialize language configuration of language w/ ID: "
//                                      + Enum.GetName(typeof(MultilanguageMgr.LanguageID), langID)
//                                      + ", message: " + ex.Message);
//                    conf = null;
//                }
//            }
//            
//            return conf;
//        }


//        public static string GetCurrentLanguageText(List<view.PBStringExtraConf.PBExtraMsg> languageList)
//		{
//			string text = "";
//			if(languageList != null)
//			{
//				foreach(var entry in languageList)
//				{
//					if(entry.l10nKey == (int)GetLanguageSettingType())
//					{
//						return entry.value;
//					}
//				}
//			}
//			if(string.IsNullOrEmpty(text))
//			{
//				var entry = languageList.Find(e => e.l10nKey == (int)LanguageID.en_US);
//				if(null != entry)
//				{
//					text = entry.value;
//				}
//			}
//			return text;
//		}

//        public static void PerformStringUpdateFromConfig(List<PBConfStringExtraConf> updateList)
//        {
//            Logger.LogYellow(  "Performing string update from config, language: "
//                             + System.Enum.GetName(typeof(LanguageID), (LanguageID)CurrentLanguageIndex));
//            if (updateList != null)
//            {
//                foreach(var entry in updateList)
//                {
//                    if(entry.gidSpecified)
//                    {
//                        Logger.LogYellow("Updating string, gid: " + entry.gid);
//                        var languageItem = entry.msg.Find(e => e.l10nKey == CurrentLanguageIndex);
//                        if (languageItem != null)
//                        {
//                            languageDict[entry.gid] = languageItem.value;
//                            if (entry.gid == "Version")
//                            {
//                                Logger.Log("Version: " + languageItem.value);
//                            }
//                        }
//                    }
//                }
//            }
//        }

        public static void ReadMultilanguageFromJson(TextAsset asset)
        {
			if(null == asset)
			{
				return;
			}

            var jsonData = JsonMapper.ToObject(asset.ToString());
            for(int i = 0; i < jsonData.Count; ++i)
            {
                var value = jsonData[i][1];
                languageDict[(string)jsonData[i][0]] = (value != null && value.IsString) ? (string)value : "";
            }
        }


        public static List<string> QueryIDsWithPrefix(string prefix)
        {
            return languageDict.Keys.ToList().FindAll(key => key.Length > prefix.Length && key.Substring(0, prefix.Length) == prefix).ToList();
        }


		public static List<string> QueryIDsWithPrefixAndNonNullValue(string prefix)
		{
			return languageDict.Keys.ToList().FindAll(   key => key.Length > prefix.Length
			                                          && key.Substring(0, prefix.Length) == prefix
			                                          && !string.IsNullOrEmpty(languageDict[key])).ToList();
		}

        public static bool ContainsID(string ID)
        {
            return languageDict.Keys.Contains(ID);
        }

        public static string GetMutiText(string ID)
        {
            string text;
            if (!languageDict.TryGetValue(ID, out text))
            {
                text = "Missing text w/ ID: " + ID;
            }
//            else if (CurrentLanguageID == LanguageID.ae || CurrentLanguageID == LanguageID.ir)
//            {
//                text = ArabicSupport.ArabicFixer.Fix(text, false, false);
//            }

            return text;
        }

        public static string Format(string templateID, object[] objects, int i18nCount = 0)
        {
            i18nCount = Mathf.Min(objects.Length, i18nCount);
            for(int i = 0; i < i18nCount; ++i)
            {
                objects[i] = GetMutiText(objects[i].ToString());
            }

            return string.Format(GetMutiText(templateID), objects);
        }
    }
}
