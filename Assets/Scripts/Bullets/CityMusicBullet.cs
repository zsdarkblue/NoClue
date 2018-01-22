using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CityMusicBullet : MusicBulletBase {

	public override void InitShader()
	{
		bulletMat.shader = Normal;
		if (isSlide) {
			bulletMat.SetColor ("_Color", new Color (1, 1, 1));
			bulletMat.SetColor ("_EmissionColor", new Color (1.6f, 1.6f,1.6f));
		}
		else {
			bulletMat.SetColor ("_Color", new Color (120/255f, 92/ 255f, 33/ 255f));
			bulletMat.SetColor ("_EmissionColor", new Color (0f, 1.959f,1.06f));
		}
	}

	public override void ChangeShader()
	{
		bulletMat.shader = FadeIn;

		if (isSlide) {
			bulletMat.SetColor ("_bLayerColorA", new Color (0/255f, 245 / 255f, 255/ 255f));
			bulletMat.SetColor ("_bLayerColorB", new Color (231/255f, 245 / 255f, 255 / 255f));
		}
		else {
			bulletMat.SetColor ("_bLayerColorA", new Color (180/255f, 0 / 255f, 126 / 255f));
			bulletMat.SetColor ("_bLayerColorB", new Color (231/255f, 186 / 255f, 64 / 255f));
		}

		bulletMat.SetColor ("_bLayerColorC", new Color (15/255f, 255 / 255f, 17 / 255f));
		bulletMat.SetVector ("_Inter", new Vector4 (0.1f, 200, 100, 40));

		bulletMat.SetFloat ("_FresPow", 14f);
		bulletMat.SetFloat ("_FresMult", 1);
		bulletMat.SetFloat ("_FresPowOut", 10);
		bulletMat.SetFloat ("_FresMultOut", 0.5f);
		bulletMat.SetFloat ("_InvFade", 3);
		bulletMat.SetFloat ("_Fade", 1);
	}
}
