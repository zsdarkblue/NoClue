using UnityEngine;
using System.Collections;

public class RhythmBulletBase {

	public static string BulletPrefabName = "bullet";

	public int HitTime;
	public RhythmBulletBase(int hitTime)
	{
		HitTime = hitTime;
	}

	public virtual void ReleaseBullet(float currentTime)
	{

	}
}
