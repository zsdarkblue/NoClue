using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.GameState;
public class CityContainer : SceneGameObjectContainer {
	public GameObject Doom;
	public GameObject DoomGround;
	public GameObject SceneEventRoot;
	// Use this for initialization
	void Start () {
	
	}

	public void FadeInScene()
	{
		StartCoroutine (FadeOutDoom ());
	}

	IEnumerator FadeOutDoom()
	{
		yield return new WaitForSeconds (1f);

		App.SoundMgr.Play (AudioResources.fadeInScene);
		Doom.GetComponent<CoolFadeOut> ().FadeOut ();
		DoomGround.GetComponent<CoolFadeOut> ().FadeOut ();
		yield return new WaitForSeconds (3f);
		Doom.SetActive(false);
		DoomGround.SetActive (false);

	}
}
