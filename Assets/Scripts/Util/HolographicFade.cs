using UnityEngine;
using System.Collections;

public class HolographicFade : MonoBehaviour {

	float startFade = 0;
	float endFade = 1;
	bool fading = false;
	float current = 0;
	float speed;

	public Material mat;

	public void StartFade(float start,float end,float _speed)
	{

		startFade = start;
		endFade = end;
		speed = _speed;
		fading = true;
		current = startFade;

		mat.SetFloat ("_Fade", startFade);	
	}

	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (fading) {
			current += Time.deltaTime * speed;
			if (current > endFade) {
				current = endFade;
				fading = false;
			}

			mat.SetFloat ("_Fade", current);	
		}
	}
}
