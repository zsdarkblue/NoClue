using UnityEngine;
using System.Collections;

public class HolographicFlashing : MonoBehaviour {

	enum ColorType
	{
		R,
		G,
		B,
	}

	ColorType emissionColorType = ColorType.R;  //biggest rgb is the number of emission
	Color initEmissionColor;

	float minEmiss;
	float maxEmiss;
	bool flashing = false;
	float currentValue;
	float speed;
	bool flashForward = true;
	bool isTryStoping;
	float finial;
	public Material HDR_mat;

	public void StartFlash(float min,float max,float _finial,float _speed)
	{
		minEmiss = min;
		maxEmiss = max;
		speed = _speed;
		flashing = true;
		finial = _finial;

		initEmissionColor = HDR_mat.GetColor ("_EmissionColor");
		if (initEmissionColor.r > initEmissionColor.g && initEmissionColor.r > initEmissionColor.b) {
			emissionColorType = ColorType.R;
			currentValue = initEmissionColor.r;
		}
		else if (initEmissionColor.g > initEmissionColor.r && initEmissionColor.g > initEmissionColor.b) {
			emissionColorType = ColorType.G;
			currentValue = initEmissionColor.g;
		}
		else {
			emissionColorType = ColorType.B;
			currentValue = initEmissionColor.b;
		}
	}

	public void Stop()
	{
		isTryStoping = true;
		if (currentValue > finial) {
			minEmiss = finial;
			Debug.Log ("minEmiss :" + minEmiss + "  " + currentValue);
		}
		else {
			maxEmiss = finial;
			Debug.Log ("maxEmiss :" + maxEmiss + "   " + currentValue);

		}
	}

	// Update is called once per frame
	void Update () {
		if (flashing) {
			if (flashForward) {
				currentValue += Time.deltaTime * speed;
				if (currentValue > maxEmiss) {
					currentValue = maxEmiss;
					flashForward = false;

					if (isTryStoping && currentValue == finial) {
						flashing = false;
					}
				}
			}
			else {
				currentValue -= Time.deltaTime * speed;
				if (currentValue < minEmiss) {
					currentValue = minEmiss;
					flashForward = true;

					if (isTryStoping && currentValue == finial) {
						flashing = false;
					}
				}
			}

			SetColorByValue (currentValue);
		}
	}

	void SetColorByValue(float currentValue)
	{
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
}
