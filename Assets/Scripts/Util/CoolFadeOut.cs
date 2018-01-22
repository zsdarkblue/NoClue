using UnityEngine;
using System.Collections;

public class CoolFadeOut : MonoBehaviour {
	float m_fDestruktionSpeed = 0.25f;
	public Material m_Mat;
	float m_fTime;
	bool fadeing = false;

	void Start () {
		m_fTime = 0;
		m_Mat.SetFloat("_Amount", 0);
		m_Mat.SetColor("_Color", new Color (0, 0, 0));
	}

	public void FadeOut()
	{
		fadeing = true;
	}

	void Update () {
		if (fadeing) {
			m_fTime += Time.deltaTime * m_fDestruktionSpeed;
			var r = Mathf.Min(m_fTime * 60,1);
			var b = Mathf.Min(m_fTime * 40,0.666f);
			m_Mat.SetColor("_Color",  new Color (r, 0, b));
			m_Mat.SetFloat("_Amount", m_fTime);

			if (m_fTime >= 1.5f)
				fadeing = false;
		}
	}
}
