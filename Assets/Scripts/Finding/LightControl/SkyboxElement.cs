using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkyboxElement
{
	float initParam;
	Renderer skyRenderer;
	float skyChangingSpeed;

	float currentParam;

	bool isChanging;
	float reduWeightInNight;
	float targetParam;
	public SkyboxElement(GameObject skyboxObject,float reduWeight)
	{
		reduWeightInNight = reduWeight;
		skyRenderer = skyboxObject.GetComponentInChildren<Renderer> ();

		initParam = 1;
		skyChangingSpeed = initParam;
		currentParam = initParam;
		targetParam = initParam;
		isChanging = false;

		skyRenderer.sharedMaterial.color = new Color (initParam,initParam,initParam);
	}

	public void TurnOffImmediate()
	{
		isChanging = false;
		currentParam = 0;
		skyRenderer.sharedMaterial.color = new Color (currentParam,currentParam,currentParam);
	}

	public void TurnOff()
	{
		if (skyChangingSpeed > 0) {
			skyChangingSpeed = (-skyChangingSpeed);
		}
		isChanging = true;
	}

	public void TurnOn(bool isDay)
	{
		if (skyChangingSpeed < 0) {
			skyChangingSpeed = (-skyChangingSpeed);
		}

		if (isDay) {
			targetParam = initParam;
		} else {
			targetParam = Mathf.Min(0.2f,initParam * reduWeightInNight);
		}
		isChanging = true;
	}

	public void Update()
	{
		if (!isChanging)
			return;

		currentParam += skyChangingSpeed * Time.deltaTime;
		if (skyChangingSpeed > 0) {
			if (currentParam > targetParam) {
				currentParam = targetParam;
				isChanging = false;
			}

		} else {
			if (currentParam < 0) {
				currentParam = 0;
				isChanging = false;
			}
		}

		skyRenderer.sharedMaterial.color = new Color (currentParam,currentParam,currentParam);
	}
}