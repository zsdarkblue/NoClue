using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.GameState;
using UnityStandardAssets.ImageEffects;
using BeatsFever;

#if !Steamworks_Off
using Steamworks;
#endif

public enum GameMode{
	Null,
	Story,
	Challenge,
}

public class DeviceContainer : MonoBehaviour {
	public GameObject Eye;
	public Material hammerMaterial;
	public GameObject LeftHammer;
	public GameObject RightHammer;
	public GameObject ShieldRoot;
	public AudioSource MainAudioSource;
	public AudioSource ClockAudioSource;

	public AudioSource LobbyMusic;
	public AudioSource GameMusic;

	public Shader normalShader;
	public Shader selectedShader;

	public UILabel TimerUI;
	public UILabel LeftCounterUI;
	public UISprite TimerIcon;
	public UISprite LeftCounterIcon;

	public ColorCorrectionCurves colorCorrectionCurves;

	public DayNightSwitcher DayNightSwitcher_Current;
	public ScenePickItemContainer PickItemContainer_Current;

	public Light LeftLight;
	public Light RightLight;

	private FlashlightElement flashLeftLight;
	private FlashlightElement flashRightLight;

	public PopUpFaceCameraItem GoodCatchItem;
	public PopUpFaceCameraItem BadCatchItem;

	public GameObject HintObject;
	public UILabel HintCount;
	public int CurrentLeftHintCount;

	public GameMode gameMode;
	public bool IsDuringFinding = false;

	public MultilanguageMgr.LanguageID Language;

	//public ColorCorrectionCurveContainer colorCorrectionCurveContainer;
	///public VignetteAndChromaticAberration VignetteAndChromaticAberration;
	//public GlobalFog GlobalFog;
	//public BloomOptimized BloomOptimized;

	public bool isGamePause = false;

	float shieldRadius = 1;
	bool isCheckingUserHeight = false;
	bool isLaunchedByOculus = false;
	// Use this for initialization

	private float eyeHightOffset = -0.063f;
	void Awake () {
		flashLeftLight = new FlashlightElement (LeftLight);
		flashRightLight = new FlashlightElement (RightLight);

		instance = this;
		SetHammerColorByCommboIndex (-1);

		#if !Steamworks_Off
		if (UnityEngine.VR.VRDevice.model.ToLower().Contains ("oculus")) {
			isLaunchedByOculus = true;
			ShieldRoot.transform.localPosition += new Vector3 (0,0,-0.25f);
			eyeHightOffset = -0.1f;
			Debug.Log ("Oculus is running");
		} else {
			isLaunchedByOculus = false;
		}
		#else
		ShieldRoot.transform.localPosition += new Vector3 (0,0,-0.25f);
		eyeHightOffset = -0.1f;
		#endif

		#if OculusHome
		isLaunchedByOculus = true;
		#endif

		HintObject.SetActive (false);
	}

	public void SetCameraEffectActive(bool active)
	{
		if (ClockAudioSource.enabled != active) {
			ClockAudioSource.enabled = active;
			if (active) {
				if(!IsInvoking("Flashing"))
					InvokeRepeating ("Flashing", 0.1f, 1);
			} else {
				CancelInvoke ("Flashing");
				colorCorrectionCurves.enabled = false;
			}
		}
	}

	public void PlayMusic(bool isLobby)
	{
		if (isLobby) {
			GameMusic.Stop ();
			LobbyMusic.Play ();
		} else {
			GameMusic.Play ();
			LobbyMusic.Stop ();
		}
	}

	void Flashing()
	{
		colorCorrectionCurves.enabled = !colorCorrectionCurves.enabled;
	}

	public bool IsLaunchedByOculus()
	{
		return isLaunchedByOculus;
	}

	public void SwitchFlashLight(bool isOn)
	{
		if (isOn) {
			flashLeftLight.TurnOn ();
			flashRightLight.TurnOn ();
		} else {
			flashLeftLight.TurnOff ();
			flashRightLight.TurnOff ();
		}
	
	}

	void UpdateUI()
	{
		if (TimerUI != null) {
			if (IsDuringFinding) {
				TimerIcon.enabled = true;
				TimerUI.text = App.Game.ScoreMgr.LeftSeconds.ToString ();
			} else {
				TimerIcon.enabled = false;
				TimerUI.text = "";
			}
		}

		if (LeftCounterUI != null) {

			if (IsDuringFinding) {
				LeftCounterIcon.enabled = true;
				LeftCounterUI.text = App.Game.ScoreMgr.RightPickCount.ToString () + " / " + ScoreMgr.ChangeItemTotalCount;
			} else {
				LeftCounterIcon.enabled = false;
				LeftCounterUI.text = "";
			}
		}
	}

	public void ShowPopUpItem(GameObject target, bool isGoodCatch)
	{
		if (isGoodCatch) {
			GoodCatchItem.PopUp (target);
		} else {
			BadCatchItem.PopUp (target);
		}
	}

	void Update()
	{
		flashLeftLight.Update ();
		flashRightLight.Update ();

		UpdateUI ();

//		if (DeviceContainer.Instance.IsLaunchedByOculus ()) {
//			if (OVRInput.GetDown(OVRInput.RawButton.Start)) {
//				if (App.Game.GameStateMgr.ActiveState is BeatState) {
//					if (!(App.Game.GameStateMgr.ActiveState as BeatState).IsRetureToLobbyING) {
//						App.InputMgr.MenuButtonClick ();
//					}
//				} 
//			}
//		}
	}

	public Vector3 GetShieldRootCenterPosition()
	{
		return ShieldRoot.transform.position;
	}

	public Vector3 GetShieldGlassPosition()
	{
		return ShieldRoot.transform.position + shieldRadius * new Vector3(0,1.5f,1);
	}

	public float GetShieldRadius()
	{
		return shieldRadius;
	}

	public void SetHammerColorByCommboIndex(int colorIndex)
	{
		if (2 == colorIndex) {
			hammerMaterial.SetColor ("_EmissionColor", new Color(1,0.109f,0.650f));
			CancelInvoke ("ResetHammerColor");
			Invoke ("ResetHammerColor", 0.1f);
		}
		else if(1 == colorIndex)
		{
			hammerMaterial.SetColor ("_EmissionColor", new Color(0.760f,0.078f,0.411f));
			CancelInvoke ("ResetHammerColor");
			Invoke ("ResetHammerColor", 0.1f);
		}
		else if(0 == colorIndex)
		{
			hammerMaterial.SetColor ("_EmissionColor", new Color(0.596f,0.227f,0.909f));
			CancelInvoke ("ResetHammerColor");
			Invoke ("ResetHammerColor", 0.1f);
		}
		else if (-1 == colorIndex) {
			hammerMaterial.SetColor ("_EmissionColor", new Color(0.0271f,0.588f,0.625f));
		}
	}

	void ResetHammerColor()
	{
		hammerMaterial.SetColor ("_EmissionColor", new Color(0.0271f,0.588f,0.625f));
	}

	public void ChangeCamera(string scene)
	{
		StartCoroutine(ChangeCameraAfterTime(scene));
	}

	IEnumerator ChangeCameraAfterTime(string scene,float delay = 0.5f)
	{
		yield return new WaitForSeconds (delay);

		if (scene.ToLower().Equals ("lobby")) {
//			GlobalFog.height = 71.6f;
//			GlobalFog.heightDensity = 1.2f;
//			GlobalFog.startDistance = 30f;
//			GlobalFog.distanceFog = false;
//			GlobalFog.excludeFarPixels = false;
//			GlobalFog.useRadialDistance = true;
//
//			BloomOptimized.threshold = 0.204f;
//			BloomOptimized.intensity = 0.701f;
//			BloomOptimized.blurSize = 3.19f;
//
			//GlobalFogRhytemElemets.enabled = true;
		}
		else if (scene.ToLower().Equals ("tokyo")) {
//			GlobalFog.height = 25.6f;
//			GlobalFog.heightDensity = 0.47f;
//			GlobalFog.startDistance = 520.3f;
//			GlobalFog.distanceFog = true;
//			GlobalFog.excludeFarPixels = false;
//			GlobalFog.useRadialDistance = true;
//
//			BloomOptimized.threshold = 0.204f;
//			BloomOptimized.intensity = 1.42f;
//			BloomOptimized.blurSize = 2.3f;

			//GlobalFogRhytemElemets.enabled = true;
		}
		else if (scene.ToLower().Equals ("lodon")) {
//			GlobalFog.height = 41.11f;
//			GlobalFog.heightDensity = 1.2f;
//			GlobalFog.startDistance = 0f;
//			GlobalFog.distanceFog = true;
//			GlobalFog.excludeFarPixels = true;
//			GlobalFog.useRadialDistance = false;
//
//			BloomOptimized.threshold = 0.155f;
//			BloomOptimized.intensity = 1.932f;
//			BloomOptimized.blurSize = 1.98f;

			//GlobalFogRhytemElemets.enabled = true;
		}
		else if (scene.ToLower().Equals ("rio")) {
//			GlobalFog.height = 53.92f;
//			GlobalFog.heightDensity = 0.77f;
//			GlobalFog.startDistance = 38.1f;
//			GlobalFog.distanceFog = true;
//			GlobalFog.excludeFarPixels = true;
//			GlobalFog.useRadialDistance = true;
//
//			BloomOptimized.threshold = 0.14f;
//			BloomOptimized.intensity = 2.5f;
//			BloomOptimized.blurSize = 1.91f;

			//GlobalFogRhytemElemets.enabled = false;
		}
	}

	public void InitShieldRootPosition()
	{
		if (App.Game.LocalDataBase.IsUserShieldHeightSet ()) {
			ShieldRoot.transform.position = new Vector3 (ShieldRoot.transform.position.x, App.Game.LocalDataBase.GetUserShieldHeight (), ShieldRoot.transform.position.z);
		}
		else {
			StartCheckUserHeight ();
		}
	}

	public void StartCheckUserHeight()
	{
		isCheckingUserHeight = true;
	}

	public void FinishCheckUserHeight()
	{
		isCheckingUserHeight = false;
		App.Game.LocalDataBase.SetUserShieldHeight (ShieldRoot.transform.position.y);
	}

	public void SetShieldActive(bool active)
	{
		ShieldRoot.SetActive (active);
	}

	public void PlayShieldAnimation()
	{
		SetShieldActive (true);
		var scanObject = ShieldRoot.transform.GetChild (0).gameObject;
		scanObject.GetComponent<AudioSource> ().Play ();
		scanObject.GetComponent<MeshRenderer> ().enabled = false;

		var tween = ShieldRoot.AddComponent<TweenScale> ();
		tween.from = new Vector3 (ShieldRoot.transform.localScale.x,ShieldRoot.transform.localScale.y,1);
		tween.to = new Vector3 (ShieldRoot.transform.localScale.x,ShieldRoot.transform.localScale.y,ShieldRoot.transform.localScale.z);
		tween.duration = 0.2f;
		tween.PlayForward ();
		tween.AddOnFinished (()=> {
			scanObject.GetComponent<MeshRenderer> ().enabled = true;
		});
	}

	private static DeviceContainer instance;
	public static DeviceContainer Instance
	{
		get
		{
			return instance;
		}
	}

	int lastTestId = -1;
	int lastTestScore;
	int lastTestSeconds;
	int lastMaxScore;
	/*
	void OnGUI()
	{
		if(GUILayout.Button("Play One Music"))
		{
			lastTestId = Random.Range (0, 20);
			var conf = App.Game.ConfigureMgr.GetStoryConfig ().GetMusicConfig (lastTestId);
			lastMaxScore = conf.MaxScore;
			lastTestScore = (int)Random.Range (conf.MaxScore * 0.7f,conf.MaxScore * 0.95f);
			lastTestSeconds = Random.Range (200, 400);
			SteamAPIMgr.Instance.StorePlayResult (lastTestId,lastTestScore,lastTestSeconds);
		}

		if (lastTestId >= 0) {
			GUILayout.Label ("level Id:" + lastTestId.ToString() + "  score:"+lastTestScore + "   rank:" + App.Game.ScoreMgr.GetRankByScoreAndMaxScore(lastTestScore,lastMaxScore));
		}

		//GUILayout.BeginArea (new Rect(Screen.width - 300,0,300,800));

		if(GUILayout.Button("Reset Stats And Achievements"))
		{
			SteamUserStats.ResetAllStats (true);
			SteamUserStats.RequestCurrentStats ();
			SteamAPIMgr.Instance.RefreshAllDataFromSteam ();
		}
	}
	*/
}
