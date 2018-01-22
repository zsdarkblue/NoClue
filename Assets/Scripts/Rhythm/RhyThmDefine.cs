using UnityEngine;
using System.Collections;

public class RhyThmDefine {
	//bullet
	//final pre second is BulletShowPreTimeParam * bpm, first song bmp is 88
	public const float BulletShowPreTimeParam = 0.04f;
	public const float BulletSpeed = 5f;

	//glass coordinate
	public const float GlassCoordinateScale = 0.5f;
	public const float GlassCoordinateYoffset = -0.2f;

	//slide
	public const float SlideItemShowDelay = 0.5f;
	public const float SliedPointGapParam = 0.25f;

	//indicate
	public const float IndicateStartTime = 1.5f;

	public const float GoodHitTimeZoneFront = 0.15f;
	public const float PrefactHitTimeZoneFront = 0.03f;
	public const float PrefactHitTimeZoneBack = 0.02f;
	public const float GoodHitTimeZoneBack = 0.01f;

	public const float IndicateDisapperaTime = 0.2f;

	//music event
	public const int CameraFeverLevel1Score = 40;
	public const int CameraFeverLevel2Score = 80;
	public const int CameraFeverLevel3Score = 120;

	public const float CameraFerverLastTime = 1.5f;

	//Drumstick
	public const float DrumstickShakeSeconds_Good = 0.2f;
	public const float DrumstickShakeSeconds_Prefact = 0.2f;
	public const ushort DrumstickShakePower = 2000;

	//3D interactive
	public const float EyeFocusDotLimit = 0.98f;
	public const float EyeFocusSuccessTimeLimit = 0.2f;
}