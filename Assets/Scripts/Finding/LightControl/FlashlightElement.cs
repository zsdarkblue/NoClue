using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlashlightElement
{
	float initParam;
	Light light;
	float changingSpeed;

	float currentParam;

	bool isChanging;

	public FlashlightElement(Light flashLight)
	{
		light = flashLight;

		initParam = flashLight.intensity;

		light.intensity = 0;
		currentParam = 0;
		changingSpeed = -initParam;

		isChanging = false;
	}

	public void TurnOffImmediate()
	{
		isChanging = false;
		currentParam = 0;
		light.intensity = 0;
	}

	public void TurnOff()
	{
		if (changingSpeed > 0) {
			changingSpeed = (-changingSpeed);
		}
		isChanging = true;
	}

	public void TurnOn()
	{
		if (changingSpeed < 0) {
			changingSpeed = (-changingSpeed);
		}
		isChanging = true;
	}

	public void Update()
	{
		if (!isChanging)
			return;

		currentParam += changingSpeed * Time.deltaTime;
		if (changingSpeed > 0) {
			if (currentParam > initParam) {
				currentParam = initParam;
				isChanging = false;
			}

		} else {
			if (currentParam < 0) {
				currentParam = 0;
				isChanging = false;
			}
		}

		light.intensity = currentParam;
	}
}