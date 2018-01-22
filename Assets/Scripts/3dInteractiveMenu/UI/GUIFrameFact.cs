
namespace BeatsFever.UI
{
    internal static class GUIFrameFact
    {
        public static void BuildFrame<T>(int id) where T : GUIFrame
        {
            App.Game.GUIFrameMgr.GUIFrameBuilder.BuildGuiFrame<T>(id);
        }

		public static void BuildFrame(int id)
		{
			App.Game.GUIFrameMgr.GUIFrameBuilder.BuildGuiFrame<GUIFrame>(id);
		}

        public static void DestroyFrame(int id)
        {
            GUIFrameMgr mgr = App.Game.GUIFrameMgr;
            GUIFrame frame = mgr.GetFrame(id);
            if (frame != null)
            {
                mgr.UnRegister(frame.ID);
                frame.Release();
            }
        }

        public static void BuildUndeadGUI()
        {
           // BuildFrame<LoadingFrame>(GUIFrameID.LoadingFrame);
        }

        public static void BuildGlobalUI()
        {
			BuildFrame<GUIFrame>(GUIFrameID.MenuUI);
			BuildFrame<GUIFrame>(GUIFrameID.CitySelectUI);
			BuildFrame<GUIFrame>(GUIFrameID.MusicSelectUI);
			BuildFrame<GUIFrame>(GUIFrameID.ResultUI);
			BuildFrame<GUIFrame>(GUIFrameID.RotateMusicSelectUI);
			BuildFrame<GUIFrame>(GUIFrameID.ShieldTextUI);
			BuildFrame<GUIFrame>(GUIFrameID.ShieldProtectUI);
			BuildFrame<GUIFrame>(GUIFrameID.CreditsUI);
			BuildFrame<GUIFrame>(GUIFrameID.PauseUI);
			BuildFrame<GUIFrame>(GUIFrameID.ProfileUI);
			BuildFrame<GUIFrame>(GUIFrameID.AchievementUI);
			BuildFrame<GUIFrame>(GUIFrameID.LeaderBoardUI);
			BuildFrame<GUIFrame>(GUIFrameID.GuideUI);
        }
			
        public static void DestroyGlobalUI()
        {
          	
        }
    }
}
