using UnityEngine;
using System.Collections;

public class TextureOffsetUpdater : MonoBehaviour {

	public Material mat;
	public float scrollSpeed = 0.5f;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		float offset = -Time.time * scrollSpeed;
		mat.SetTextureOffset ("_MainTex", new Vector2 (offset, 0));
	}
}
