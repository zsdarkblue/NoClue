using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.GameState;

public class LobbyContainer : SceneGameObjectContainer {
	public GameObject StandTable;
	// Use this for initialization
	void Start () {
	
	}

	public void SetStandTableActive(bool active)
	{
		StandTable.SetActive (active);
	}
}
