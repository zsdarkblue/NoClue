using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatsFever;
using BeatsFever.GameState;


public class HandRayEventSender : MonoBehaviour {

	#if !Steamworks_Off
	public SteamVR_TrackedObject tracked;
	private SteamVR_Controller.Device device;
	#endif
	public Transform RayObject;

	BoxCollider boxCollider;
	private Collider activeCollider;
	private bool isShaking = false;
	float rayScale = 0.003f;

	public FlashController flashController;

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

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider> ();
	}

	void LaunchRay()
	{
		Ray eyeRay = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (eyeRay, out hit, 1000f,LayerMask.GetMask("Default"))) {
			PickBehaviour pickItem = hit.collider.GetComponent<PickBehaviour> ();
			if (pickItem != null) {

				if (pickItem is PickItemBehaviour) {
					if (pickItem.IgnorePick)
						return;

					if (DeviceContainer.Instance.IsDuringFinding) {
						//flashController.Flash ();

						bool success = pickItem.TriggerPlayerPickAction ();
						if (success) {
							App.Game.ScoreMgr.UpdateScore (ScoreMgr.ScoreType.Right);
							DeviceContainer.Instance.ShowPopUpItem (pickItem.gameObject, true);
						} else {
							App.Game.ScoreMgr.UpdateScore (ScoreMgr.ScoreType.Wrong);
							DeviceContainer.Instance.ShowPopUpItem (pickItem.gameObject, false);
							ShakeDrumstick ();
						}
					}
				} else {
					pickItem.TriggerPlayerPickAction ();
				}
			}
		} 
	}

	void Update()
	{
		#if !Steamworks_Off
		if (null == device) {
			device = SteamVR_Controller.Input ((int)tracked.index);
			return;
		}
		#endif

		if (DeviceContainer.Instance.DayNightSwitcher_Current != null && DeviceContainer.Instance.PickItemContainer_Current != null) {

			#if !Steamworks_Off
			if(DeviceContainer.Instance.IsLaunchedByOculus())
			{
				if (device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_A)) {
					DeviceContainer.Instance.DayNightSwitcher_Current.LaunchSwitchProcess ();
				}
			}
			else
			{
				if (device.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad)) {
					DeviceContainer.Instance.DayNightSwitcher_Current.LaunchSwitchProcess ();
				}
			}

			#elif OculusHome
			if (OVRInput.GetDown(OVRInput.RawButton.A) || 
				OVRInput.GetDown(OVRInput.RawButton.B) ||
				OVRInput.GetDown(OVRInput.RawButton.X) ||
				OVRInput.GetDown(OVRInput.RawButton.Y) ) {
				DeviceContainer.Instance.DayNightSwitcher_Current.LaunchSwitchProcess ();
			}
			#endif

			if (DeviceContainer.Instance.DayNightSwitcher_Current.IsSwitching) {
				if (RayObject.gameObject.activeSelf) {
					RayObject.gameObject.SetActive (false);
				}
				ExitAllPickItems();
				return;
			}
		}
			

		if (!RayObject.gameObject.activeSelf) {
			RayObject.gameObject.SetActive (true);
		}

		Ray eyeRay = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (eyeRay, out hit, 1000f,LayerMask.GetMask("Default"))) {
			PickBehaviour pickBehaviour = hit.collider.GetComponent<PickBehaviour> ();
			if (pickBehaviour != null) {
				if (activeCollider == null) {
					activeCollider = hit.collider;
					pickBehaviour.OnRayEnter ();

					RayObject.localPosition = Vector3.forward * hit.distance * 0.5f;
					RayObject.localScale = new Vector3 (rayScale, rayScale, hit.distance);
				} else {
					if (activeCollider != hit.collider) {
						activeCollider.GetComponent<PickBehaviour> ().OnRayExit ();
						hit.collider.GetComponent<PickBehaviour> ().OnRayEnter ();
						activeCollider = hit.collider;

						RayObject.localPosition = Vector3.forward * hit.distance * 0.5f;
						RayObject.localScale = new Vector3 (rayScale, rayScale, hit.distance);
					}
				}
			} else {
				ExitAllPickItems ();
			}
		} else {
			ExitAllPickItems();
		}

		#if !Steamworks_Off
		if(DeviceContainer.Instance.IsLaunchedByOculus())
		{
			if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
				LaunchRay ();			
			}
		}
		else
		{
			if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
				LaunchRay ();			
			}
		}

		#elif OculusHome
		if ((isLeft && OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)) ||
			(!isLeft && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))) {
			LaunchRay ();			
		}
		#endif

	}

	void ExitAllPickItems()
	{
		if (activeCollider != null) {
			activeCollider.GetComponent<PickBehaviour>().OnRayExit ();
			activeCollider = null;

			RayObject.localPosition = Vector3.forward * 10;
			RayObject.localScale = new Vector3 (rayScale, rayScale, 20);
		} 

	}

	public void ShakeDrumstick()
	{
		#if !Steamworks_Off
		StartCoroutine (Shake(0.2f));
		#else
		ShakeOculus ();
		#endif

	}

	public void ShakeOculus()
	{
		#if OculusHome
		if (isLeft) {
			OVRHaptics.LeftChannel.Mix (hapticsClip);
		} else {
			OVRHaptics.RightChannel.Mix (hapticsClip);
		}
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
			device.TriggerHapticPulse (500);
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
