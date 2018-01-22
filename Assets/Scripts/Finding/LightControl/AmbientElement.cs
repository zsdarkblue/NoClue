using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AmbientElement
{
	float initParam;
	float skyChangingSpeed;

	float currentParam;

	bool isChanging;
	float reduWeightInNight;
	float targetParam;
	public AmbientElement(float reduWeight)
	{
		reduWeightInNight = reduWeight;
		initParam = RenderSettings.ambientIntensity;
		skyChangingSpeed = initParam;
		currentParam = initParam;
		targetParam = initParam;
		isChanging = false;
	}

	public void TurnOffImmediate()
	{
		isChanging = false;
		currentParam = 0;
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
			targetParam = initParam * reduWeightInNight;
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

		RenderSettings.ambientIntensity = currentParam;
	}
}