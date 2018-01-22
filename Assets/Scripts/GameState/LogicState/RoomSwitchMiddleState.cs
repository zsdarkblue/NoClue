using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using BeatsFever.UnityEx;
using System.Text.RegularExpressions;
using System;
using BeatsFever.UI;

namespace BeatsFever.GameState
{
	public class RoomSwitchMiddleState : GameState
    {
		public RoomSwitchMiddleState(int id) : base(id){}

		private RoomMiddleStateParam stateParam;
        public override void Enter(GameStateParam param)
        {

			Debug.Log("RoomSwitchMiddleState Enter...");

			stateParam = param as RoomMiddleStateParam;

			App.MainGo.StartCoroutineExt(SwitchToNextRoom(DeviceContainer.Instance.gameMode));
        }

		private IEnumerator SwitchToNextRoom(GameMode mode)
		{
			yield return new WaitForSeconds(0.1f);
			var beatParam = new BeatStateParam ();
			beatParam.Scene = stateParam.NextLevelName;
			App.Game.GameStateMgr.SwitchTo (GameStateID.ST_Beat, beatParam);
		}


        public override void Exit()
        {
			Debug.Log("RoomSwitchMiddleState Exit...");
        }
    }
}