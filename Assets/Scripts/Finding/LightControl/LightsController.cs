using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BeatsFever;

public class LightElement
{
	float initLightIntensity;
	Light lightCompent;
	float lightChangingSpeed;

	bool isChanging;

	public LightElement(Light light,float reduWeight)
	{
		lightCompent = light;
		light.intensity = light.intensity * reduWeight;
		initLightIntensity = light.intensity;
		lightChangingSpeed = initLightIntensity;
		isChanging = false;
	}
		

	public void TurnOffImmediate()
	{
		isChanging = false;
		lightCompent.intensity = 0;
	}

	public void TurnOff()
	{
		if (lightChangingSpeed > 0) {
			lightChangingSpeed = (-lightChangingSpeed);
		}
		isChanging = true;
	}

	public void TurnOn()
	{
		if (lightChangingSpeed < 0) {
			lightChangingSpeed = (-lightChangingSpeed);
		}
		isChanging = true;
	}

	public void Update()
	{
		if (!isChanging)
			return;

		lightCompent.intensity += lightChangingSpeed * Time.deltaTime;
		if (lightChangingSpeed > 0) {
			if (lightCompent.intensity > initLightIntensity) {
				lightCompent.intensity = initLightIntensity;
				isChanging = false;
			}

		} else {
			if (lightCompent.intensity < 0) {
				lightCompent.intensity = 0;
				isChanging = false;
			}
		}
	}
}

public class LightsController : MonoBehaviour {
	public bool OffLightWhenAwake;
	List<LightElement> lights = new List<LightElement>();
	// Use this for initialization
	public void Init(float reduWeight) {
		var sceneLights = GetComponentsInChildren<Light> ();
		foreach (var light in sceneLights) {
			LightElement element = new LightElement (light,reduWeight);
			lights.Add (element);
		}

		if (OffLightWhenAwake) {
			foreach (var light in lights) {
				light.TurnOffImmediate ();
			}
			gameObject.SetActive (true);
		}
	}

	public void SwitchLights(bool on)
	{
		foreach (var light in lights) {
			if (on) {
				light.TurnOn ();
			} else {
				light.TurnOff ();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var light in lights) {
			light.Update ();
		}
	}
}
