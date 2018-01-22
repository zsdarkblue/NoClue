using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishPathData{
	public FishPathData(int _index,Vector3 _position){
		index = _index;
		position = _position;
	}

	public int index;
	public Vector3 position;
}

public class FishGroupSwimEvent : MusicEvent {

	Vector3[] path;

	public List<FishPathData> datas = new List<FishPathData>();

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
		GameObject fishGroup = Instantiate (Resources.Load ("FishesGroup"),path[0],Quaternion.identity) as GameObject;
		fishGroup.GetComponentInChildren<FishLeaderSwimScript> ().StartSwimAtPath (path);
	}
}
