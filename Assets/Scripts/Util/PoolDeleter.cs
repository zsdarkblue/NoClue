using UnityEngine;

public sealed class PoolDeleter : MonoBehaviour
{
	private float delay;

	public static PoolDeleter Attach(GameObject go, float delay = 2)
	{
		var deleter = go.GetComponent<PoolDeleter >();
		if (deleter == null)
		{
			deleter = go.AddComponent<PoolDeleter >();
		}

		deleter.enabled = true;
		deleter.delay = delay;
		return deleter;
	}

	private void OnEnable()
	{
		delay = 2;
	}

	private void OnDisable()
	{
		RenderPoolManager.Instance.Remove(this.gameObject);
	}

	void Update()
	{	
		delay -= Time.deltaTime;
		if (delay <= 0)
		{
			enabled = false;
		}
	}

	public void SetDeleteDelay(float delay)
	{
		this.delay = delay;
	}

}