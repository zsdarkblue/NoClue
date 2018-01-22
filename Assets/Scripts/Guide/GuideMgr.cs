using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatsFever.GameState;
using BeatsFever.UnityEx;
using BeatsFever.UI;

namespace BeatsFever.Guide
{
	public enum GuideState
	{
		Null,
		FindStandTable,
		KnockBullets,
		GuideOver,
	}

    public class GuideMgr
    {
		//投资人车轮战演示开关
		public bool ForceGuide = true;

		public GameObject Eye;
		public GameObject StandTable;
		float checkStandPositionDistance = 0.18f;

		GuideState state = GuideState.Null;
		GameObject teachHammer;
		GameObject[] teachingBullets;

		public void CheckStandPosition(GameObject standTable)
		{
			state = GuideState.FindStandTable;
			StandTable = standTable;
		}

		public void SetGuideState(GuideState newState)
		{
			state = newState;
		}

		void CheckStandPosition()
		{
			if (null == Eye) {
				Eye = DeviceContainer.Instance.Eye;
			}

			Vector2 eyePos = new Vector2 (Eye.transform.position.x, Eye.transform.position.z);
			Vector2 tablePos = new Vector2 (StandTable.transform.position.x, StandTable.transform.position.z);

			if (Vector2.Distance (eyePos, tablePos) < checkStandPositionDistance) {
				state = GuideState.KnockBullets;
				(App.Game.GameStateMgr.ActiveState as LobbyState).HideUserStandIndicator ();
				LaunchShield ();
			}
		}

		void LaunchShield()
		{
			App.Game.GUIFrameMgr.Active (GUIFrameID.ShieldProtectUI);
			App.Game.GUIFrameMgr.Active (GUIFrameID.ShieldTextUI);
			App.MainGo.StartCoroutineExt(CreateTeachingBullets());
		}

		public IEnumerator CreateTeachingBullets()
		{
			yield return new WaitForSeconds(0.5f);
		}

		public void OnBulletKnock(int index)
		{
			GameObject.Destroy(teachingBullets [index]);

			if (index + 1 >= teachingBullets.Length) {
				App.MainGo.StartCoroutineExt(KnockAllBullets());
				GameObject.Destroy (teachHammer);
			}
			else {
				teachHammer.SetActive(false);
				App.MainGo.StartCoroutineExt (ShowNextBullet (teachingBullets [index + 1]));
			}
		}

		IEnumerator ShowNextBullet(GameObject bullet)
		{
			yield return new WaitForSeconds (0.5f);
			//use shield position to make pos right
			bullet.transform.position = bullet.transform.position + DeviceContainer.Instance.GetShieldGlassPosition();
			bullet.SetActive (true);	
			var script = bullet.GetComponent<TeachingBullet>();
			script.StartShow (teachHammer);
		}

		IEnumerator KnockAllBullets()
		{
			DeviceContainer.Instance.FinishCheckUserHeight ();
			yield return new WaitForSeconds (2f);

			App.Game.LocalDataBase.SetGuideOver (true);
			state = GuideState.GuideOver;
			(App.Game.GameStateMgr.ActiveState as LobbyState).ShowLobbyMenu ();
		}

        public void Update()
        {
			if (state == GuideState.FindStandTable) {
				CheckStandPosition ();
			}
        }
        
		public void Start()
		{
			
		}

        public void ShutDown()
        {

        }
    }
}