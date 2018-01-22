using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace BeatsFever.UI
{
    class GuiAssetRegister
    {
        public static void RegisterGuiFrameCommonAssetes()
        {
			RegisterCommonFrame(GUIFrameID.MenuUI, "MenuUI");
			RegisterCommonFrame(GUIFrameID.CitySelectUI, "CitySelectUI");
			RegisterCommonFrame(GUIFrameID.MusicSelectUI, "MusicSelectUI");
			RegisterCommonFrame(GUIFrameID.ResultUI, "ResultUI");
			RegisterCommonFrame(GUIFrameID.RotateMusicSelectUI, "RotateMusicSelectUI");
			RegisterCommonFrame(GUIFrameID.ShieldTextUI, "ShieldTextUI");
			RegisterCommonFrame(GUIFrameID.ShieldProtectUI, "ShieldProtectUI");
			RegisterCommonFrame(GUIFrameID.CreditsUI, "CreditsUI");
			RegisterCommonFrame(GUIFrameID.PauseUI, "PauseUI");
			RegisterCommonFrame(GUIFrameID.ProfileUI, "ProfileUI");
			RegisterCommonFrame(GUIFrameID.AchievementUI, "AchievementUI");
			RegisterCommonFrame(GUIFrameID.LeaderBoardUI, "LeaderBoardUI");
			RegisterCommonFrame(GUIFrameID.GuideUI, "GuideUI");

        }
    

        private static void RegisterCommonFrame(int id, string assetName)
        {
            GUIFrameBuilder builder = App.Game.GUIFrameMgr.GUIFrameBuilder;
            builder.RegisterGuiAsset(id, assetName);
        }
    }
}
