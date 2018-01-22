using UnityEngine;
using System.Collections;

public class DotRhythm : RhythmBulletBase {
	public OnsetData data;
	public DotRhythm(OnsetData _data) : base(_data.timeLine)
	{
		data = _data;
	}

	public override void ReleaseBullet(float currentTime)
	{
		var go = RenderPoolManager.Instance.Create(RhythmBulletBase.BulletPrefabName);
		var timeGap = data.timeLine * 0.001f - currentTime;
		var remainTime = timeGap;
		go.GetComponent<MusicBulletBase> ().StartMove (remainTime, data.positionX ,data.positionY);
	}
}
