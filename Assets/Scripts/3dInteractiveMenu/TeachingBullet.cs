using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;

public class TeachingBullet : MonoBehaviour {

	public enum TeachType
	{
		Null,
		GoodPre,
		GoodAfter,
		Prefact,
	}

	public int Index = 0;
	private Vector3 hitIndicatePosition;
	private GameObject teachHammer;
	private TeachType teachType = TeachType.Null;
	Material bulletMat;
	// Use this for initialization
	void Start () {
		bulletMat = transform.GetChild (0).GetComponent<MeshRenderer> ().material;
		GetComponent<SphereCollider> ().enabled = false;
	}

	public void StartShow(GameObject _teachHammer)
	{
		teachType = TeachType.Prefact;
		if (Index == 0) {
			teachType = TeachType.GoodPre;
		}
		else if (Index == 1) {
			teachType = TeachType.Prefact;
		}
		else if (Index == 2) {
			teachType = TeachType.GoodAfter;
		}
		
		teachHammer = _teachHammer;
		float flyDistance = 3;

		var center = DeviceContainer.Instance.GetShieldRootCenterPosition ();
		var radius = DeviceContainer.Instance.GetShieldRadius ();
		var boardPos = transform.position;
		var dir = boardPos -center;
		Ray ray = new Ray (center, dir);

		//move to glass surface
		gameObject.transform.position = ray.GetPoint(radius);
		//face to surface's normal
		gameObject.transform.LookAt (ray.GetPoint(radius + flyDistance));
		//rotate to face new dir where this bullet will come
		transform.Rotate(new Vector3(-10,0,0));
		Ray flyRay = new Ray (gameObject.transform.position, gameObject.transform.forward);
		Ray reverseRay = new Ray (gameObject.transform.position, -gameObject.transform.forward);
		gameObject.transform.position = flyRay.GetPoint(flyDistance);

		Vector3 targetPos = Vector3.zero;
		if (teachType == TeachType.GoodPre) {
			targetPos = flyRay.GetPoint(0.1f);
		}
		else if (teachType == TeachType.Prefact) {
			targetPos = flyRay.GetPoint(0);
		}
		else if (teachType == TeachType.GoodAfter) {
			targetPos = flyRay.GetPoint(-0.11f);
		}


		hitIndicatePosition = ray.GetPoint (radius);

		iTween.MoveTo(gameObject, iTween.Hash("position",targetPos,
			"easeType", "linear",
			"time",1,
			"lookTarget",targetPos,
			"oncomplete","OnFinish"));
	}
	public void OnFinish()
	{
		teachHammer.transform.position = transform.position + new Vector3 (-0.2f, 0.2f, 0);
		teachHammer.SetActive (true);
		teachHammer.GetComponentInChildren<HolographicFade> ().StartFade (0,0.786f,1);
		GetComponent<SphereCollider> ().enabled = true;
	}

	void OnTriggerEnter(Collider other) {
		App.Game.GuideMgr.OnBulletKnock(Index);
		var explosion = RenderPoolManager.Instance.Create("Explosion",this.gameObject.transform.position);
		PoolDeleter.Attach (explosion, 2f);

		App.SoundMgr.Play (AudioResources.menuUIHit);

		var script = other.GetComponent<Drumstick> ();
		if (null != script) {
			script.ShakeDrumstick ();
		}
			
		if (teachType == TeachType.GoodPre) {
			App.Game.GUIFrameMgr.GetFrame (GUIFrameID.ShieldTextUI).Root.GetComponent<ShieldTextUI> ().ShowGreat (0,gameObject.transform.position);
		}
		else if (teachType == TeachType.Prefact) {
			App.Game.GUIFrameMgr.GetFrame (GUIFrameID.ShieldTextUI).Root.GetComponent<ShieldTextUI> ().ShowPrefact (0,gameObject.transform.position);
		}
		else if(teachType == TeachType.GoodAfter) {
			App.Game.GUIFrameMgr.GetFrame (GUIFrameID.ShieldTextUI).Root.GetComponent<ShieldTextUI> ().ShowGreat (0,gameObject.transform.position);
		}
	}

	void OnDestroy()
	{
		Object.Destroy (bulletMat);
	}
}
