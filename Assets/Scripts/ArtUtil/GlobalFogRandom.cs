using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GlobalFogRandom : MonoBehaviour {

	public GlobalFog fog;
	public float Min = 20f;
	public float Max = 40f;
	public float changeSpeed = 2f;
	public float StaticTime = 2f;

	float end;
	float speed;
	bool changing = false;
	// Use this for initialization
	void Start () {
		ChangeFog ();
	}


	void Update()
	{
		if (!changing)
			return;

		fog.height += Time.deltaTime * speed;
		if (speed > 0) {
			if (fog.height > end) {
				ChengeFinish ();
			}
		}
		else{
			if (fog.height < end) {
				ChengeFinish ();
			}
		}
	}

	void ChengeFinish()
	{
		changing = false;
		Invoke ("ChangeFog", StaticTime);
	}

	void ChangeFog()
	{
		end = Random.Range (Min,Max);
		speed = (end - fog.height) > 0 ? changeSpeed : -changeSpeed;
		changing = true;
	}
}
