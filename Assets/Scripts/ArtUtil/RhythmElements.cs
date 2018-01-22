using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using BeatsFever;

public class RhythmElements : MonoBehaviour {

	public enum EffectType
	{
		StandardShaderHDR,
		LightIntensity,
		GlobalFog,
	}

	enum ColorType
	{
		R,
		G,
		B,
	}

	public float StartSeconds = 0;
	public float PlaySeconds = 300;
	public int SpectrumDataIndex = 36;
	public float AddParam = 0;
	public float MultiplyParam = 1;
	public EffectType effectType;
	bool isPlayFinished = false;
	bool alreadyPlayedMask = false;

	public float Max = 0;
	public float Min = 0;
	public bool UseTween = true;
	public float TweenVelocity = 10f;
	//HDR color
	public Material HDR_mat;
	ColorType emissionColorType = ColorType.R;  //biggest rgb is the number of emission
	Color initEmissionColor;
	float initFloatValue;
	//light intensity
	Light light;

	//global Fog
	public GlobalFog fog;

	private float previousValue = 0;
	private float currentValue = 0;

	// Use this for initialization
	void Start () {

		//if(App.Instance != null)
		App.Game.RhythmMgr.OnMusicReset += OnMusicReset;


		if (effectType == EffectType.StandardShaderHDR) {
			if (HDR_mat == null) {
				Debug.LogError("Missing HDR material, gameobject name:"+gameObject.name);
			}
			initEmissionColor = HDR_mat.GetColor ("_EmissionColor");
			if (initEmissionColor.r > initEmissionColor.g && initEmissionColor.r > initEmissionColor.b) {
				emissionColorType = ColorType.R;
				previousValue = initEmissionColor.r;
			}
			else if (initEmissionColor.g > initEmissionColor.r && initEmissionColor.g > initEmissionColor.b) {
				emissionColorType = ColorType.G;
				previousValue = initEmissionColor.g;
			}
			else {
				emissionColorType = ColorType.B;
				previousValue = initEmissionColor.b;
			}
		}
		else if(effectType == EffectType.LightIntensity)
		{
			light = GetComponent<Light> ();
			initFloatValue = light.intensity;
			previousValue = initFloatValue;
		}
		else if(effectType == EffectType.GlobalFog)
		{
			initFloatValue =  fog.height;
			previousValue = initFloatValue;
		}
	}

	void OnMusicReset()
	{
		CancelInvoke ();
		FinishPlay ();

		currentValue = 0;
		isPlayFinished = false;
		alreadyPlayedMask = false;
	}

	// Update is called once per frame
	void Update () {
		if (isPlayFinished)
			return;
		
		if (App.Game.RhythmMgr.AudioSource == null)
			return;

		if (!App.Game.RhythmMgr.AudioSource.isPlaying)
			return;
		
		if (App.Game.RhythmMgr.AudioSource.time <= StartSeconds)
			return;


		if (!alreadyPlayedMask) {
			alreadyPlayedMask = true;
			Invoke ("FinishPlay", PlaySeconds);
		}

		UpdateEffect ();
	}

	void FinishPlay()
	{
		if (effectType == EffectType.StandardShaderHDR) {
			HDR_mat.SetColor ("_EmissionColor", initEmissionColor);
		}
		else if (effectType == EffectType.LightIntensity) {
			light.intensity = initFloatValue;
		}
		else if (effectType == EffectType.GlobalFog) {
			fog.height = initFloatValue;
		}

		isPlayFinished = true;
		alreadyPlayedMask = false;
	}

	void UpdateEffect()
	{
		if (App.Game.RhythmMgr != null && App.Game.RhythmMgr.AudioSource != null && !App.Game.RhythmMgr.AudioSource.isPlaying) {
			CancelInvoke ();
			FinishPlay ();
		}
		
		float finialValue = RhythmMgr.SpectrumData [SpectrumDataIndex] * MultiplyParam + AddParam;
		if ((Max != 0 || Min != 0) && Max > Min) {
			if (finialValue > Max) {
				finialValue = Max;
			}

			if (finialValue < Min) {
				finialValue = Min;
			}
		}

		if (UseTween) {
			float newValue = Mathf.Lerp (previousValue, finialValue, Time.deltaTime * 21.8f);
			if (newValue > previousValue) {
				previousValue = newValue;
				currentValue = previousValue;
			}
			else {
				currentValue = previousValue;
				currentValue = Mathf.Lerp (previousValue, 0.1f, Time.deltaTime * TweenVelocity);
			} 
		}
		else {
			currentValue = finialValue;
		}


		if (effectType == EffectType.StandardShaderHDR) {
			Color currentColor = initEmissionColor;
			if (emissionColorType == ColorType.R) {
				currentColor.r = currentValue;
				currentColor.g = currentValue * initEmissionColor.g / initEmissionColor.r;
				currentColor.b = currentValue * initEmissionColor.b / initEmissionColor.r;
			}
			else if (emissionColorType == ColorType.G) {
				currentColor.g = currentValue;
				currentColor.r = currentValue * initEmissionColor.r / initEmissionColor.g;
				currentColor.b = currentValue * initEmissionColor.b / initEmissionColor.g;
			}
			else {
				currentColor.b = currentValue;
				currentColor.r = currentValue * initEmissionColor.r / initEmissionColor.b;
				currentColor.g = currentValue * initEmissionColor.g / initEmissionColor.b;
			}
			HDR_mat.SetColor ("_EmissionColor", currentColor);
		}
		else if (effectType == EffectType.LightIntensity) {
			light.intensity = currentValue;
		}
		else if (effectType == EffectType.GlobalFog) {
			fog.height = currentValue;
		}


		previousValue = currentValue;
	}

	void OnDestroy()
	{
		if(App.Instance != null)
			App.Game.RhythmMgr.OnMusicReset -= OnMusicReset;
	}
}
