using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.GameState;
#if !Steamworks_Off
using Valve.VR;
#endif


public class Drumstick : MonoBehaviour {

	#if !Steamworks_Off
	public SteamVR_TrackedObject tracked;
	private SteamVR_Controller.Device device;
	#endif

	private bool isShaking = false;
	public bool isLeft = true;

	#if OculusHome
	OVRHapticsClip hapticsClip;
	#endif

	void Awake()
	{
		#if OculusHome
		byte[] samples = new byte[32];
		for (int i = 0; i < samples.Length; ++i) {
			samples [i] = 128;
		}

		hapticsClip = new OVRHapticsClip (samples,samples.Length);
		#endif
	}

	void OnTriggerEnter(Collider other) {
		ProcessHit (other);
	}

	void OnTriggerStay (Collider other) {
		ProcessHit (other);
	}

	void ProcessHit(Collider other)
	{
		var script = other.GetComponent<MusicBulletBase> ();
		if (null == script)
			return;
		
		var result = script.WeaponDetection ();
		if (RhythmMgr.BeatHitType.Null == result)
			return;

		if (result == RhythmMgr.BeatHitType.Good) {
			#if !Steamworks_Off
			StartCoroutine (Shake(RhyThmDefine.DrumstickShakeSeconds_Good));
			#else
			ShakeOculus ();
			#endif
		}
		else if (result == RhythmMgr.BeatHitType.Prefect) {
			#if !Steamworks_Off
			StartCoroutine (Shake(RhyThmDefine.DrumstickShakeSeconds_Prefact));
			#else
			ShakeOculus ();
			#endif
		}
		script.OnFinish ();
	}

	void Update()
	{
		#if !Steamworks_Off
		if (null == device) {
			device = SteamVR_Controller.Input ((int)tracked.index);
			return;
		}
			
		if (DeviceContainer.Instance.IsLaunchedByOculus ()) {
			if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
				if (App.Game.GameStateMgr.ActiveState is BeatState) {
					if (!(App.Game.GameStateMgr.ActiveState as BeatState).IsRetureToLobbyING) {
						App.InputMgr.MenuButtonClick ();
					}
				} 
			}
		}
		else
		{
			if (device.GetTouchDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
				if (App.Game.GameStateMgr.ActiveState is BeatState) {
					if (!(App.Game.GameStateMgr.ActiveState as BeatState).IsRetureToLobbyING) {
						App.InputMgr.MenuButtonClick ();
					}
				} 
			}
		}
		#endif
	}


	public void ShakeOculus()
	{
		#if Steamworks_Off
		if (isLeft) {
			OVRHaptics.LeftChannel.Mix (hapticsClip);
		} else {
			OVRHaptics.RightChannel.Mix (hapticsClip);
		}
		#endif
	}
	
	public void ShakeDrumstick()
	{
		#if !Steamworks_Off
		StartCoroutine (Shake(RhyThmDefine.DrumstickShakeSeconds_Prefact));
		#else
		ShakeOculus ();
		#endif
	}

	IEnumerator Shake(float seconds)
	{
		#if !Steamworks_Off
		if(null == device)
		device = SteamVR_Controller.Input ((int)tracked.index);

		isShaking = true;
		CancelInvoke ();
		Invoke ("StopShake", seconds);
		while (isShaking) {
		device.TriggerHapticPulse (RhyThmDefine.DrumstickShakePower);
		yield return new WaitForEndOfFrame ();
		}
		#else
		yield return new WaitForEndOfFrame ();
		#endif
	}

	void StopShake()
	{
		isShaking = false;
	}
}
