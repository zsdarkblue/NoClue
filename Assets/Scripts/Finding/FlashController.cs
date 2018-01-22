using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashController : MonoBehaviour {
	public Light light;
	float timer = 0;
	float flashTime = 0.05f;
	bool isFlashing = false;

	// Use this for initialization
	void Start () {
		light.enabled = false;
	}

	public void Flash()
	{
		light.enabled = true;
		isFlashing = true;
		timer = 0;
	}

	// Update is called once per frame
	void Update () {
		if (isFlashing) {
			timer += Time.deltaTime;
			if (timer > flashTime) {
				EndFlash ();
				isFlashing = false;
			}
		}
	}

	void EndFlash()
	{
		light.enabled = false;
	}
}
