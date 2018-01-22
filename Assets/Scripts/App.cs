using System;
using UnityEngine;
using System.Collections;
using BeatsFever.ResMgr;
using BeatsFever.Sound;

namespace BeatsFever
{
    public class App
    {
        private static App instance;
        private GameObject mainGo;
        private Game game;
        private ResourceMgr resMgr = new ResourceMgr();
        private SoundMgr soundMgr = new SoundMgr();
        private LevelMgr levelMgr = new LevelMgr();
		private InputMgr inputMgr = new InputMgr();
        
        public static App Instance { get { return instance; } }
   
        public static Game Game { get { return instance.game; } }
    
        public static ResourceMgr ResourceMgr { get { return instance.resMgr; } }

        public static SoundMgr SoundMgr { get { return instance.soundMgr; } }

        public static LevelMgr LevelMgr { get { return instance.levelMgr; } }

        public static GameObject MainGo { get { return instance.mainGo; } }
        
		public static InputMgr InputMgr { get { return instance.inputMgr; } }

        public static void Create(GameObject mainGo)
        {
            instance = new App();
            instance.mainGo = mainGo;
        }
        
        public static void Destroy()
        {
            if(instance != null)
            {
                instance.ShutDown();
            }
            instance = null;
        }
     
        public void ReStart()
        {
            ShutDown();    
                 
            Start();
        }
        
        public void Start()
        {
            Debug.Log("App Start...");
			inputMgr.Start ();
			resMgr.Start();
			soundMgr.Start();

            game = new Game();
            game.Start();
        }
   
        public void Update()
        {
            game.Update();
			inputMgr.Update ();
        }
   
        public void LateUpdate()
        {
			game.LateUpdate();
        }

        public static void Clean()
        {
            SoundMgr.Clear();
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
   
        public void ShutDown()
        {
            if(game != null)
            {
                game.ShutDown();
            }
        
            if(soundMgr != null)
            {
                soundMgr.Shutdown();
            }

            if(resMgr != null)
            {
                resMgr.Shutdown();
            }

			if(inputMgr != null)
			{
				inputMgr.Shutdown();
			}
        }
    }
}
