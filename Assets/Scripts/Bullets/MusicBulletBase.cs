using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;

public class MusicBulletBase : MonoBehaviour {

	public Shader Normal;
	public Shader FadeIn;

	protected Material bulletMat;
	protected bool isSlide;

	protected GameObject BulletRenderObject;
	protected int slideIndex;
	protected float startTime;
	protected float prefactTime;
	protected float moveTotalTime;
	protected Vector3 hitIndicatePosition;

	float baseRenderBulletScale = 16f;
	float leftPrefactHitTime;
	const bool activeSpectrumShow = false;
	IndicateEffect indicateScript;

	Vector3 previousLocalPosition = Vector3.zero;
	Vector3 leftLocalPosition;
	public enum EState
	{
		Null,
		IdleFly,
		GenIndicate,
		EarlyHit,
		PrefectHit,
		DelayHit,
		Miss,
	}

	public EState state = EState.Null;

	private List<Vector3> lineEffectPos = new List<Vector3> ();
	float lineTime;

	public void SetIndicateLine(List<Vector3> _lineEffectPos,float gapTime)
	{
		lineEffectPos = _lineEffectPos;
		lineTime = gapTime;
	}

	public virtual Vector3 StartMove(float remainTime,float x,float y,float delayTime = 0,bool _isSlide = false,int _slideIndex = 0,float scaleSpeed = 0.025f)
	{
		slideIndex = _slideIndex;
		isSlide = _isSlide;
		BulletRenderObject = gameObject.transform.GetChild (0).gameObject;
		bulletMat = BulletRenderObject.GetComponent<MeshRenderer> ().material;
		BulletRenderObject.transform.localRotation = Quaternion.identity;
		float scale = baseRenderBulletScale * (1 - slideIndex * scaleSpeed);
		if (scale < 5) {
			scale = 5;
		}
		BulletRenderObject.transform.localScale = scale * Vector3.one;

		InitShader ();
		float flyDistance = remainTime * RhyThmDefine.BulletSpeed;

		var center = DeviceContainer.Instance.GetShieldRootCenterPosition ();
		var radius = DeviceContainer.Instance.GetShieldRadius ();
		var boardPos = DeviceContainer.Instance.GetShieldGlassPosition() + new Vector3 (x, y, 0);
		var dir = boardPos -center;
		Ray ray = new Ray (center, dir);

		//move to glass surface
		gameObject.transform.position = boardPos;
		//face to surface's normal
		gameObject.transform.LookAt (ray.GetPoint(radius + flyDistance));
		//rotate to face new dir where this bullet will come
		transform.Rotate(new Vector3(-(y + 0.5f)*20,x*60,0));
		Ray flyRay = new Ray (gameObject.transform.position, gameObject.transform.forward);
		Ray reverseRay = new Ray (gameObject.transform.position, -gameObject.transform.forward);
		gameObject.transform.position = flyRay.GetPoint(flyDistance);

		var innerDistance =  RhyThmDefine.GoodHitTimeZoneBack / remainTime * flyDistance;
		var targetPos = reverseRay.GetPoint(innerDistance);

		hitIndicatePosition = ray.GetPoint (radius);
		startTime = Time.time;
		prefactTime = startTime + remainTime;
		moveTotalTime = prefactTime + RhyThmDefine.PrefactHitTimeZoneBack + RhyThmDefine.GoodHitTimeZoneBack;

		gameObject.transform.LookAt (targetPos);

		if (delayTime > 0) {
			BulletRenderObject.GetComponent<MeshRenderer> ().enabled = false;
			Invoke ("ShowRenderBullet", delayTime);
		}

		state = EState.IdleFly;

		return hitIndicatePosition;
	}
		
	void ShowRenderBullet()
	{
		BulletRenderObject.GetComponent<MeshRenderer> ().enabled = true;
	}

	public void Update()
	{
		if (state == EState.Null)
			return;

		leftPrefactHitTime = prefactTime - Time.time;
		if (Time.time < moveTotalTime) {
			if(state != EState.Miss)
				transform.position += transform.forward * (Time.deltaTime * RhyThmDefine.BulletSpeed);
		}

		if(state == EState.IdleFly)
		{
			if (leftPrefactHitTime < RhyThmDefine.IndicateStartTime) {
				state = EState.GenIndicate;
				if(0 == slideIndex)
					GenIndicateEffect ();
				BulletRenderObject.transform.localPosition = Vector3.zero;
			}
			else {
				UpdateSpectrumEffect ();
			}
		}
		else if(state == EState.GenIndicate && (leftPrefactHitTime <  RhyThmDefine.PrefactHitTimeZoneFront +  RhyThmDefine.GoodHitTimeZoneFront))
		{
			state = EState.EarlyHit;
		}
		else if (state == EState.EarlyHit) {

			if (leftPrefactHitTime < RhyThmDefine.PrefactHitTimeZoneFront) {
				state = EState.PrefectHit;
				ChangeShader ();
			}
		}
		else if(state == EState.PrefectHit)
		{
			if (leftPrefactHitTime < -RhyThmDefine.PrefactHitTimeZoneBack) {
				state = EState.DelayHit;
			}
		}
		else if(state == EState.DelayHit)
		{
			if (leftPrefactHitTime < - (RhyThmDefine.PrefactHitTimeZoneBack + RhyThmDefine.GoodHitTimeZoneBack)) {
				state = EState.Miss;

				//App.Game.ScoreMgr.UpdateScore (ScoreMgr.ScoreType.Miss,slideIndex, 0, transform.position);
			}
		}
		else if(state == EState.Miss)
		{
			OnFinish ();
			state = EState.Null;
		}
	}


	void UpdateSpectrumEffect()
	{
		if (!activeSpectrumShow || slideIndex == 0)
			return;

		float bassSensibility = 40f;
		float minParticleSensibility = 1.5f;
		float bassHeight = 1.5f;
		float globalScale = 3f;
		float smoothVelocity = 10f;
		var spectrumLeftValue = RhythmMgr.SpectrumData [slideIndex] * bassSensibility;
		float newLocalY;

		if (spectrumLeftValue >= minParticleSensibility) {

			newLocalY = Mathf.Lerp (previousLocalPosition.y, spectrumLeftValue * bassHeight * globalScale, Time.deltaTime * 21.8f);
		} else {
			newLocalY = Mathf.Lerp (previousLocalPosition.y, spectrumLeftValue * globalScale * 0.5f, Time.deltaTime * 21.8f);
		}


		if (newLocalY > previousLocalPosition.y) {
			previousLocalPosition.y = newLocalY;
			leftLocalPosition = previousLocalPosition;
		} else {
			leftLocalPosition = previousLocalPosition;
			leftLocalPosition.y = Mathf.Lerp (previousLocalPosition.y, 0.1f, Time.deltaTime * smoothVelocity);
		} 

		BulletRenderObject.transform.localPosition = Vector3.up *newLocalY * 0.3f;
	}

	public virtual void InitShader()
	{

	}

	public virtual void ChangeShader()
	{

	}

	public void GenIndicateEffect()
	{
		var go = RenderPoolManager.Instance.Create("indicateEffect",hitIndicatePosition);
		indicateScript = go.GetComponent<IndicateEffect> ();
		indicateScript.StartShow (prefactTime,prefactTime +  RhyThmDefine.GoodHitTimeZoneBack,slideIndex);
		indicateScript.SetIndicateLine (lineEffectPos,lineTime);
		go.transform.LookAt (DeviceContainer.Instance.GetShieldRootCenterPosition ());
	}

	public RhythmMgr.BeatHitType WeaponDetection()
	{
		if (state == EState.IdleFly) {
			return RhythmMgr.BeatHitType.Null;
		}
		else if (state == EState.GenIndicate) {
			return RhythmMgr.BeatHitType.Null;
		}
		else if (state == EState.EarlyHit) {
			if (0 == slideIndex) {
				//App.SoundMgr.Play (AudioResources.se_great);
			}
			var leftSecondsForPrefact = prefactTime - Time.time;
			var leftSecondsForPrefactFront = leftSecondsForPrefact - RhyThmDefine.PrefactHitTimeZoneFront;
			var gapProcress = leftSecondsForPrefactFront / RhyThmDefine.GoodHitTimeZoneFront;

			//App.Game.ScoreMgr.UpdateScore (ScoreMgr.ScoreType.Right,slideIndex,gapProcress,transform.position);


			return RhythmMgr.BeatHitType.Good;

		}
		else if (state == EState.PrefectHit) {
			if (0 == slideIndex) {
				indicateScript.ShowPrefact ();
				//App.SoundMgr.Play (AudioResources.se_perfect);
			}
			float gapProcress = 0;
			if (prefactTime - Time.time > 0) {
				var leftSecondsForPrefact = prefactTime - Time.time;
				gapProcress = leftSecondsForPrefact / RhyThmDefine.PrefactHitTimeZoneFront;
			}
			else {
				var passedSecondsForPrefact = Time.time - prefactTime;
				gapProcress = passedSecondsForPrefact / RhyThmDefine.PrefactHitTimeZoneBack;
			}

			//App.Game.ScoreMgr.UpdateScore (ScoreMgr.ScoreType.Wrong,slideIndex,gapProcress,transform.position);
			return RhythmMgr.BeatHitType.Prefect;

		}
		else if (state == EState.DelayHit) {
			if (0 == slideIndex) {
				//App.SoundMgr.Play (AudioResources.se_great);
			}
			var passedSecondsForPrefact = Time.time - prefactTime;
			var passedSecondsForPrefactBack = passedSecondsForPrefact - RhyThmDefine.PrefactHitTimeZoneBack;
			var gapProcress = passedSecondsForPrefactBack / RhyThmDefine.GoodHitTimeZoneFront;

			//App.Game.ScoreMgr.UpdateScore (ScoreMgr.ScoreType.Right,slideIndex,gapProcress,transform.position);

			return RhythmMgr.BeatHitType.Good;

		}
		else {
			return RhythmMgr.BeatHitType.Null;
		}

	}

	public void OnFinish()
	{
		lineEffectPos.Clear ();
		state = EState.Null;

		var explosion = RenderPoolManager.Instance.Create("Explosion",this.gameObject.transform.position);

		bool needShowDistortionEffect = (slideIndex == 0);
		for (int i = 0; i < explosion.transform.childCount; ++i) {
			explosion.transform.GetChild (i).gameObject.SetActive (needShowDistortionEffect);
		}

		PoolDeleter.Attach (explosion, 5);

		RenderPoolManager.Instance.Remove (this.gameObject);
	}

	void OnDestroy()
	{
		Object.Destroy (bulletMat);
	}
}
