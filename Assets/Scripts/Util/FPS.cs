using UnityEngine;


public class FPS : MonoBehaviour
{
	private const float updateInterval = 0.5f;
	private float lastInterval;
	private int frames;
	private float fps;
	private Texture2D texture;
	private string prefix;

	// Use this for initialization
	private void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
	}

	private void OnDestroy()
	{
		if (texture != null)
		{
			Resources.UnloadAsset(texture);
			texture = null;
		}
	}

	#if !(RELEASE)
	private void OnGUI()
	{
		GUILayout.Label(" FPS : " + fps.ToString("f2"));

		// Walk around to fix the black flicking on Android
		//if (texture == null)
		//{
		//    texture = Resources.Load("dot") as Texture2D;    
		//}

		//GUILayout.Label(texture);
	}
	#endif

	#if !(RELEASE)
	// Update is called once per frame
	private void Update()
	{
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval)
		{
			fps = frames / (timeNow - lastInterval);
			frames = 0;
			lastInterval = lastInterval + updateInterval;
		}
	}
	#endif
}