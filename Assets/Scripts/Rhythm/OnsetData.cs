using UnityEngine;
using System;
using System.Collections;

public class OnsetData : ICloneable
{
	public int id;
	public int timeLine;
	public int slideID;
	public float positionX;
	public float positionY;

	public OnsetData(){}

	public OnsetData(string data)
	{
		string[] segments = data.Split (',');
		for (int i = 0; i < segments.Length; ++i) {

			if (0 == i) {
				//Debug.Log ("@@@@@:" + segments [i]);
				id = int.Parse (segments[i]);
			}
			if (1 == i) {
				timeLine = int.Parse (segments[i]);
			}
			else if (2 == i) {
				positionX = GameUtil.ParseStingToFloat (segments[i]);
				//positionX = GameUtil.ParseStingToFloat (segments[i]) * RhyThmDefine.GlassCoordinateScale;
			}
			else if (3 == i) {
				positionY = GameUtil.ParseStingToFloat (segments[i]);
				//positionY = (GameUtil.ParseStingToFloat (segments[i]) + RhyThmDefine.GlassCoordinateYoffset ) * RhyThmDefine.GlassCoordinateScale;
			}
			else if (4 == i) {
				slideID = int.Parse (segments[i]);
			}
		}
	}

	public OnsetData(int _id,int _milliSeconds,int _slideId, float _hitX,float _hitY)
	{
		id = _id;
		timeLine = _milliSeconds;
		slideID = _slideId;
		positionX = _hitX;
		positionY = _hitY;
	}

	public object Clone()
	{
		OnsetData newData = new OnsetData ();
		newData.id = this.id;
		newData.timeLine = this.timeLine;
		newData.slideID = this.slideID;
		newData.positionX = this.positionX;
		newData.positionY = this.positionY;
		return newData;
	}

}