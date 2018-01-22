using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RenderPoolManager {

	private class PoolItem
	{
		private string name;
		private Object prefab;
		private Queue<GameObject> queue = new Queue<GameObject>();

		public PoolItem(string name,Object loadedRes)
		{
			this.name = name;
			if(null != loadedRes)
				prefab = loadedRes;
			else
				prefab = GameObject.Instantiate(Resources.Load(name));
		}
		
		public GameObject Spawn()
		{
			if (queue.Count != 0)
			{
				//Logger.Log("spawn object: " + name);
				var item = queue.Dequeue();
				return item;
			}
			else
			{
				//Logger.Log("spawn object: " + name);
				GameObject item = null;
				if (prefab != null)
				{
					item = Object.Instantiate(prefab) as GameObject;
				}
				
				if(item != null)
				{
					item.name = name;
				}
				
				return item;
			}
		}
		
		public void Despawn(GameObject item)
		{
			if (item != null)
			{
				// Remove the parent to avoid be destoryed by parent.
				item.transform.parent = null;
				item.transform.localPosition = Vector3.zero;
				item.transform.localRotation = Quaternion.identity;
				item.transform.localScale = Vector3.one;

				var effects = item.GetComponentsInChildren<ParticleSystem>();
				foreach(var effect in effects)
				{
					effect.Stop();
				}

				item.SetActive(false);
				queue.Enqueue(item);
			}
		}
		
		public void DestroySelf()
		{
			prefab = null;
			
			foreach (var item in queue)
			{
				Object.Destroy(item);
			}
			
			queue.Clear();
		}
	}
	
	private static RenderPoolManager instance;
	private Dictionary<string, PoolItem> pools = new Dictionary<string, PoolItem>();
	
	public GameObject Spawn(string name,Object loadedRes = null)
	{
		PoolItem pool;
		pools.TryGetValue(name, out pool);
		if (pool == null)
		{
			pool = new PoolItem(name,loadedRes);
			pools[name] = pool;
		}
		
		return pool.Spawn();
	}
	
	public void Despawn(GameObject item, string name)
	{
		PoolItem pool;
		pools.TryGetValue(name, out pool);
		if (pool == null)
		{
			pool = new PoolItem(name,null);
			pools[name] = pool;
		}
		
		pool.Despawn(item);
	}
	
	public bool ContrainPool(string name)
	{
		return pools.ContainsKey(name);
	}
	
	public void Clear()
	{	
		foreach (var pool in pools.Values)
		{
			pool.DestroySelf();
		}
		
		pools.Clear();
	}
	
	public static RenderPoolManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new RenderPoolManager();
			}
			
			return instance;
		}
	}
	
	private RenderPoolManager()
	{
		
	}
	
	public void Preload(string resName, int count = 1)
	{
		List<GameObject> preloadList = new List<GameObject>();
		for (int i = 0; i < count; i++) {
			GameObject preloadObj = Create(resName);
			preloadObj.SetActive(false);
			preloadList.Add(preloadObj);
		}
		
		foreach (var preloadObj in preloadList) {
			Remove(preloadObj);
		}
	}
	
	public GameObject Create(string resName)
	{
		return Create(resName, Vector3.zero);
	}
	
	public GameObject Create(string resName, Vector3 position)
	{
		return Create(resName, position, null);
	}
	
	public GameObject Create(string resName, Vector3 position, Transform parent)
	{
		if (string.IsNullOrEmpty(resName))
		{
			return null;
		}
		
		GameObject gameObject = Spawn(resName);
		
		if (gameObject != null)
		{
			gameObject.SetActive(false);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = position;
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(true);

			var effects = gameObject.GetComponentsInChildren<ParticleSystem>();
			foreach(var effect in effects)
			{
				effect.Play();
			}

			return gameObject;
		}
		else
		{
			Debug.LogWarning("not found: \"" + resName + "\"");
			return null;
		}
	}

	public GameObject CreateWithLoadedRes(string resName, GameObject loadedRes ,Vector3 position, Transform parent)
	{
		if (string.IsNullOrEmpty(resName))
		{
			return null;
		}

		GameObject gameObject = Spawn(resName,loadedRes);

		if (gameObject != null)
		{
			gameObject.SetActive(false);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = position;
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(true);

			var effects = gameObject.GetComponentsInChildren<ParticleSystem>();
			foreach(var effect in effects)
			{
				effect.Play();
			}

			return gameObject;
		}
		else
		{
			Debug.LogWarning("not found: \"" + resName + "\"");
			return null;
		}
	}
	
	public void Remove(GameObject gameObj)
	{
		if(null == gameObj)
		{
			return;
		}
		
		Despawn(gameObj, gameObj.name);
	}
}