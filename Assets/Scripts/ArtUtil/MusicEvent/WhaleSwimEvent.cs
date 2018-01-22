using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhaleSwimEvent : MusicEvent {

	Vector3[] path;

	private bool startAlpha1;
	private bool startAlpha2;
	private Material mat1;
	private Material mat2;
	public List<FishPathData> datas = new List<FishPathData>();

	GameObject portal;
	void Awake()
	{
		BuildPath ();
	}

	void BuildPath()
	{
		var childs = GetComponentsInChildren<Transform> ();
		foreach (var child in childs) {
			int index;
			if (int.TryParse (child.name, out index)) {
				datas.Add (new FishPathData (index,child.position));
			}
		}
		datas.Sort(new CompareChain<FishPathData>().Add(item => item.index));
		path = new Vector3[datas.Count];
		for (int i = 0; i < datas.Count; ++i) {
			path [i] = datas [i].position;
		}
	}

	public override void LaunchEvent()
	{
		portal = Instantiate (Resources.Load ("portal"), path [0], Quaternion.identity) as GameObject;

		mat1 = portal.transform.FindChild ("portal/Core").gameObject.GetComponent<MeshRenderer> ().material;
		mat2 = portal.transform.FindChild ("portal/Fringe").gameObject.GetComponent<MeshRenderer> ().material;

		portal.transform.LookAt (DeviceContainer.Instance.GetShieldRootCenterPosition());
		//portal.transform.position = path [0] + portal.transform.TransformPoint(new Vector3(0,0,50));
		Invoke ("LaunchWhale", 3);
	}


	void LaunchWhale()
	{
		GameObject fishGroup = Instantiate (Resources.Load ("whale"),path[0],Quaternion.identity) as GameObject;
		fishGroup.GetComponent<FishLeaderSwimScript> ().StartSwimAtPath (path);

		startAlpha1 = true;
		Invoke ("HideDoor", 2f);
	}

	void HideDoor()
	{
		startAlpha2 = true;
		var color = mat2.GetColor("_TintColor");
		color.a = 0;
		mat2.SetColor("_TintColor",color);
		Invoke ("HidePortal", 5f);
	}

	void HidePortal()
	{
		Object.Destroy (mat1);
		Object.Destroy (mat2);
		GameObject.Destroy (portal);
	}

	void Update()
	{
		if (startAlpha1) {
			var color = mat1.GetColor("_TintColor");
			color.a = color.a - Time.deltaTime * 0.25f;
			if (color.a < 0) {
				color.a = 0;
				startAlpha1 = false;
			}
			mat1.SetColor("_TintColor",color);
		}

		if (startAlpha2) {
			var color = mat2.GetColor("_TintColor");
			color.a = color.a - Time.deltaTime * 0.5f;
			if (color.a < 0) {
				color.a = 0;
				startAlpha2 = false;
			}
			mat2.SetColor("_TintColor",color);
		}
	}
}
