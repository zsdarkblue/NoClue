using UnityEngine;
using System.Collections;
using BeatsFever.GameState;
using BeatsFever.Guide;
using BeatsFever.ResMgr;
using BeatsFever.UI;

namespace BeatsFever
{
    public class Game
    {
        private GameStateMgr gameStMgr = null;
		private GuideMgr guideMgr = null;
		private LocalDataBase localDataBase = null;
		private GUIFrameMgr guiFrameMgr = null;
		private ConfigureMgr configureMgr = null;
		private AnalyticsMgr analyticsMgr = null;
		private ScoreMgr scoreMgr = null;
		private RhythmMgr rhythmMgr = null;
		private MusicEventMgr musicEventMgr = null;

        public GameStateMgr GameStateMgr { get { return gameStMgr; } }
		public GuideMgr GuideMgr {get { return guideMgr; }}
		public LocalDataBase LocalDataBase {get { return localDataBase; }}
		public GUIFrameMgr GUIFrameMgr {get { return guiFrameMgr; }}
		public ConfigureMgr ConfigureMgr {get { return configureMgr; }}
		public AnalyticsMgr AnalyticsMgr {get { return analyticsMgr; }}
		public ScoreMgr ScoreMgr {get { return scoreMgr; }}
		public RhythmMgr RhythmMgr {get { return rhythmMgr; }}
		public MusicEventMgr MusicEventMgr {get { return musicEventMgr; }}

        public void Start()
        {
            Debug.Log("Game Start...");
     		
			analyticsMgr = new AnalyticsMgr ();
			configureMgr = new ConfigureMgr ();
			localDataBase = new LocalDataBase();
			scoreMgr = new ScoreMgr ();

			rhythmMgr = new RhythmMgr ();
			rhythmMgr.Start ();

			musicEventMgr = new MusicEventMgr ();
			musicEventMgr.Start ();

            gameStMgr = new GameStateMgr();
            gameStMgr.Start();

			guiFrameMgr = new GUIFrameMgr ();
			guiFrameMgr.Start ();

			guideMgr = new GuideMgr ();
			guideMgr.Start ();

			//SkillMaker not run state machine
			if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals ("MusicMaker")) {
				InitState();			
			}
        }
   
        public void Update()
        {
            gameStMgr.Update();
			guideMgr.Update ();
			guiFrameMgr.Update ();
			musicEventMgr.Update ();
        }
   
		public void LateUpdate()
		{
			
		}


        public void ShutDown()
        {
            if(gameStMgr != null)
            {
                gameStMgr.ShutDown();
            }

			if(guideMgr != null)
			{
				guideMgr.ShutDown();
			}

			if(guiFrameMgr != null)
			{
				guiFrameMgr.ShutDown();
			}
        }
        
        private void InitState()
        {
            GameStateFactory.Build(gameStMgr);
            gameStMgr.SwitchTo(GameStateID.ST_Boot, new BootStateParam());
        }
	}
}