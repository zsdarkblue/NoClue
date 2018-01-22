using UnityEngine;
using System.Collections;
using BeatsFever;

public class RestartBullet : MonoBehaviour {

	Transform eyeTransform;
	Vector3 targetPos;
	float lookAtTime = 0;
	private GameObject uiObject;

	enum RestartBulletState
	{
		WaitForLookAt,
		KeepLooking,
		Coming,
		GoingBack,
		WaitForClick,
	}

	RestartBulletState state = RestartBulletState.WaitForLookAt;
	public void Init(Transform eyeTrans,Vector3 _targetPos)
	{
		eyeTransform = eyeTrans;
		targetPos = _targetPos;

		float flyDistance = 3;
		var center = DeviceContainer.Instance.GetShieldRootCenterPosition ();
		var radius = DeviceContainer.Instance.GetShieldRadius ();
		var boardPos = targetPos;
		var dir = boardPos -center;
		Ray ray = new Ray (center, dir);

		//move to glass surface
		gameObject.transform.position = ray.GetPoint(radius);
		//face to surface's normal
		gameObject.transform.LookAt (ray.GetPoint(radius + flyDistance));
		//rotate to face new dir where this bullet will come
		transform.Rotate(new Vector3(-20,20,0));
		Ray flyRay = new Ray (gameObject.transform.position, gameObject.transform.forward);
		Ray reverseRay = new Ray (gameObject.transform.position, -gameObject.transform.forward);
		gameObject.transform.position = flyRay.GetPoint(flyDistance);

//		uiObject = ShieldUI.Instance.GetRestartUI ();
//		uiObject.SetActive (true);
//		uiObject.transform.localScale = Vector3.one * 3f;

		GetComponent<HolographicFlashing> ().StartFlash (0, 1.6f,0.3f, 3f);
	}

	void Update()
	{
		if (null == eyeTransform) {
			return;
		}

		if (uiObject != null) {
			uiObject.transform.position = transform.position - Vector3.up * 0.2f;
		}

		if (state == RestartBulletState.WaitForLookAt) {
			float dot = Vector3.Dot ((gameObject.transform.position - eyeTransform.position).normalized, eyeTransform.forward);
			if (dot > RhyThmDefine.EyeFocusDotLimit) {
				lookAtTime = 0;
				state = RestartBulletState.KeepLooking;
			}
		}
		else if (state == RestartBulletState.KeepLooking) {
			float dot = Vector3.Dot ((gameObject.transform.position - eyeTransform.position).normalized, eyeTransform.forward);
			if (dot > RhyThmDefine.EyeFocusDotLimit) {
				lookAtTime += Time.deltaTime;
				if (lookAtTime > RhyThmDefine.EyeFocusSuccessTimeLimit) {

					float tweenTime = 1.5f;
					iTween.MoveTo(gameObject, iTween.Hash("position",targetPos,
						"easeType", "linear",
						"time",tweenTime,
						"oncomplete","OnFinish"));

					iTween.ScaleTo (uiObject, iTween.Hash("scale",Vector3.one,
						"easeType", "linear",
						"time",tweenTime));

					state = RestartBulletState.Coming;
				}
			}
			else {
				state = RestartBulletState.WaitForLookAt;
			}

		}
		else if (state == RestartBulletState.GoingBack) {

		}
		else if (state == RestartBulletState.Coming) {

		}
		else if (state == RestartBulletState.WaitForClick) {

		}
	}

	void OnFinish()
	{
		GetComponent<HolographicFlashing> ().Stop ();
		state = RestartBulletState.WaitForClick;
	}

	void OnTriggerEnter(Collider other) {
		if (state != RestartBulletState.WaitForClick)
			return;

		var explosion = RenderPoolManager.Instance.Create("Explosion",this.gameObject.transform.position);
		PoolDeleter.Attach (explosion, 2);

		App.SoundMgr.Play (AudioResources.menuUIHit);

		var script = other.GetComponent<Drumstick> ();
		if (null != script) {
			script.ShakeDrumstick ();
		}

		GameObject.Destroy (this.gameObject);
	}
}
