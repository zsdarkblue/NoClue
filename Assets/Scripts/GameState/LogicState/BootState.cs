using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using BeatsFever.UI;
using BeatsFever.UnityEx;
using System;

#if OculusHome
using Oculus.Platform;
#endif

namespace BeatsFever.GameState
{
    public class BootState : GameState
    {
		public BootState(int id) : base(id){}
        
        public override void Enter(GameStateParam param)
        {
            Debug.Log("Start Booting...");

			#if UNITY_EDITOR
			GamePrepare ();
			App.MainGo.StartCoroutineExt(EnterLobby());
			#else
				#if OculusHome
				Core.Initialize ("1272704959484607");
				Entitlements.IsUserEntitledToApplication ().OnComplete(OculusCallBack);
				#else
				GamePrepare ();
				App.MainGo.StartCoroutineExt(EnterLobby());
				#endif
			#endif

        }

		#if OculusHome
		void OculusCallBack(Message msg)
		{
			if (!msg.IsError) {
				GamePrepare ();
				App.MainGo.StartCoroutineExt(EnterLobby());
			} else {
				UnityEngine.Application.Quit ();
			}
		}
		#endif

		private void GamePrepare()
		{
			#if Steamworks_Off
			SteamAPIMgr.DisableSteamWorks = true;
			#endif

			App.Game.ConfigureMgr.InitGameConfig ();
			App.Game.LocalDataBase.InitLevelRecordData ();
			GuiAssetRegister.RegisterGuiFrameCommonAssetes ();
			GUIFrameFact.BuildUndeadGUI ();
			GUIFrameFact.BuildGlobalUI ();

			SteamAPIMgr.Instance.Init ();
			SteamAPIMgr.Instance.RequestCurrentState ();

			SpecialDayCheck ();

			App.Game.ScoreMgr.ResetAll ();
		}

		private void SpecialDayCheck()
		{
			DateTime dataTime = DateTime.Now;
			if (dataTime.Month == 2 && dataTime.Day >= 11 && dataTime.Day <= 17) {
				RhythmBulletBase.BulletPrefabName = "bullet_love";
			}
		}


		private IEnumerator EnterLobby()
		{
			//Steamworks.SteamUserStats.ResetAllStats(true);

			yield return new WaitForSeconds(0.2f);
			var lobbyParam = new LobbyStateParam ();
			lobbyParam.Scene = "lobby";
			App.Game.GameStateMgr.SwitchTo (GameStateID.ST_Lobby, lobbyParam);
		}

        public override void Update()
        {

        }

        public override void Exit()
        {
           
        }
    }
}