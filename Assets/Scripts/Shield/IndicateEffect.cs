using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class IndicateEffect : MonoBehaviour {

	private float startTime;
	private float totalTime;
	private float endTime;
	private float prefactTime;
	private float leftPrefactHitTime;

	public Color StartColor;
	public Color PrefactColor;
	public Color PrefactHitColor;
	public Color MissColor;

	public GameObject touchInObj;
	public GameObject touchOutObj;
	public GameObject crossObj;

	private Material matIn;
	private Material matOut;
	private Material matCorss;

	private int slideIndex;

	private float inStartScale = 0.01f;
	private float outStartScale = 0.01f;
	private float corssStartScale = 4f;//1.3f;
	private float corssStartAlpha = 0.5f;

	float corssPassedTime = 0;
	float corssProcress = 0;

	private bool tweenCorss = false;
	private float tweenCorssStartTime ;
	public enum EState
	{
		Null,
		FadeIn,
		GoodFront,
		PrefactFront,
		PrefactBack,
		GoodBack,
		FadeOut,
		MoveOnPath,
	}
	public EState state = EState.Null;
	private bool isInPrefactState = false;

	private List<Vector3> lineEffectPos = new List<Vector3> ();
	float lineTime;

	public void SetIndicateLine(List<Vector3> _lineEffectPos,float gapTime)
	{
		lineEffectPos = _lineEffectPos;
		lineTime = gapTime;
		if (lineEffectPos.Count > 0) {
			Invoke("ShowScreenTrailLine",0.5f);		
		}
	}

	List<GameObject> linePoints = new List<GameObject>();
	void ShowScreenTrailLine()
	{
		var go = RenderPoolManager.Instance.Create("trailEffect",lineEffectPos[0]);
		iTween.MoveTo(go, iTween.Hash("path",lineEffectPos.ToArray(),
			"easeType", "linear",
			"time",lineTime));
	}


	public void StartShow(float _prefactTime,float _endTime,int _slideIndex = 0)
	{
		state = EState.FadeIn;
		matIn = touchInObj.GetComponent<MeshRenderer>().material;
		matOut = touchOutObj.GetComponent<MeshRenderer>().material;
		matCorss = crossObj.GetComponent<MeshRenderer> ().material;
		SetColor (StartColor);

		touchInObj.transform.localScale = Vector3.one * inStartScale;
		touchOutObj.transform.localScale = Vector3.one * outStartScale;
		crossObj.transform.localScale = Vector3.one * corssStartScale;
		crossObj.SetActive (false);

		slideIndex = _slideIndex;
		if (slideIndex > 0) {
			touchInObj.GetComponent<MeshRenderer> ().enabled = false;
			touchOutObj.GetComponent<MeshRenderer> ().enabled = false;
			crossObj.GetComponent<MeshRenderer> ().enabled = false;
		}

		startTime = Time.time;
		totalTime = _prefactTime - startTime;
		endTime = _endTime;
		prefactTime = _prefactTime;
		state = EState.FadeIn;

		corssPassedTime = 0;
		corssProcress = 0;
	}

	void Update()
	{
		touchInObj.transform.Rotate (Vector3.up * Time.deltaTime * 100f, Space.Self);
		touchOutObj.transform.Rotate (Vector3.up * Time.deltaTime * -200f, Space.Self);


		leftPrefactHitTime = prefactTime - Time.time;
		var passedTime = Time.time - startTime;
		var prefactProcress = passedTime / totalTime;
		touchInObj.transform.localScale = Mathf.Lerp(inStartScale,1,prefactProcress) * Vector3.one;
		touchOutObj.transform.localScale = Mathf.Lerp(outStartScale,1,prefactProcress) * Vector3.one;

		if (state == EState.FadeIn && (leftPrefactHitTime < RhyThmDefine.PrefactHitTimeZoneFront + RhyThmDefine.GoodHitTimeZoneFront)) {

			state = EState.GoodFront;
			CrossReady ();

			Shake (RhyThmDefine.GoodHitTimeZoneFront + RhyThmDefine.PrefactHitTimeZoneFront + RhyThmDefine.PrefactHitTimeZoneBack + RhyThmDefine.GoodHitTimeZoneBack);
		}
		else if (state == EState.GoodFront) {
			if (leftPrefactHitTime < RhyThmDefine.PrefactHitTimeZoneFront) {

				if (lineEffectPos.Count > 0) {
					MoveHitEffectByPath (RhyThmDefine.PrefactHitTimeZoneFront);
					state = EState.MoveOnPath;
				}
				else {
					state = EState.PrefactFront;
					SetColor (PrefactColor);
					if (slideIndex > 0) {
						touchInObj.GetComponent<MeshRenderer> ().enabled = true;
						touchOutObj.GetComponent<MeshRenderer> ().enabled = true;
						crossObj.GetComponent<MeshRenderer> ().enabled = true;
					}
				}
			}
		}
		else if (state == EState.PrefactFront) {
			if (leftPrefactHitTime < -RhyThmDefine.PrefactHitTimeZoneBack) {
				state = EState.GoodBack;
			}
		}
		else if (state == EState.GoodBack) {
			if (Time.time > endTime) {
				state = EState.FadeOut;

				if(!isInPrefactState)
					SetColor (MissColor);
			}
		}
		else if (state == EState.FadeOut) {
			if (leftPrefactHitTime < - (RhyThmDefine.PrefactHitTimeZoneBack + RhyThmDefine.GoodHitTimeZoneBack+ RhyThmDefine.IndicateDisapperaTime)) {
				DestroyIndicate ();
				isInPrefactState = false;
			}
		}

		UpdateTweenEffect ();
	}

	void UpdateTweenEffect()
	{
		if (tweenCorss) {
			float totalTime = RhyThmDefine.PrefactHitTimeZoneFront + RhyThmDefine.GoodHitTimeZoneFront;
			if (Time.time - tweenCorssStartTime > totalTime) {
				tweenCorss = false;
			}

			corssPassedTime += Time.deltaTime;
			corssProcress = corssPassedTime / totalTime;
	
			crossObj.transform.localScale = ((1 - corssProcress) * 3 + 1) * Vector3.one;
			matCorss.color = new Color(StartColor.r,StartColor.g,StartColor.b,corssStartAlpha + corssProcress * 0.5f);
		}
	}

	void MoveHitEffectByPath(float delay)
	{
		iTween.MoveTo(gameObject, iTween.Hash("path",lineEffectPos.ToArray(),
			"easeType", "linear",
			"delay", delay,
			"time",lineTime,
			"oncomplete","OnFinish"));
	}

	public void OnFinish()
	{
		DestroyIndicate ();
	}
	void DestroyIndicate()
	{
		for (int i = 0; i < linePoints.Count; ++i) {
			RenderPoolManager.Instance.Remove(linePoints[i]);
		}
		linePoints.Clear ();
		state = EState.Null;
		RenderPoolManager.Instance.Remove(this.gameObject);
	}

	private void Shake(float time)
	{
		//iTween.ShakeScale (touchInObj, Vector3.one * shakeStrength,time);
		//iTween.ShakeScale (touchOutObj, Vector3.one * shakeStrength,time);
	}
	private void SetColor(Color color)
	{
		matIn.color = color;
		matOut.color = color;
	}

	private void CrossReady()
	{
		crossObj.SetActive (true);
		matCorss.color = new Color(StartColor.r,StartColor.g,StartColor.b,corssStartAlpha);

		tweenCorss = true;
		tweenCorssStartTime = Time.time;
	}

	public void ShowPrefact()
	{
		SetColor (PrefactHitColor);
		isInPrefactState = true;
	}

	void OnDestroy()
	{
		Object.Destroy (matIn);
		Object.Destroy (matOut);
		Object.Destroy (matCorss);
	}
}
