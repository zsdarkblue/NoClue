using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwitchType
{
	Null,
	RandomSwitchItem,
	FrzzedAfterInitItem,
}

public class PickItemBehaviour : PickBehaviour {

	public SwitchType SwitchType;
	Material dayCopyMaterial;
	Material nightCopyMaterial;

	GameObject dayObject;
	GameObject nightObject;

	public bool IsFounded()
	{
		return IsFoundByPlayer;
	}

	bool IsNoChildrenObject;
	bool IsFoundByPlayer;
	public void Init() {
		IsFoundByPlayer = false;
		IgnorePick = false;

		List<Transform> canUseChildList = new List<Transform> ();
		int parentId = transform.GetInstanceID ();
		Transform[] children = GetComponentsInChildren<Transform> (true);
		foreach (var child in children) {
			if (child.GetInstanceID () == parentId)
				continue;

			canUseChildList.Add (child);
		}

		if (canUseChildList.Count > 0) {
			//hide parent object
			var renderer = GetComponent<Renderer> ();
			renderer.enabled = false;


			if (SwitchType == SwitchType.Null) {
				int dayIndex = Random.Range (0, canUseChildList.Count);
				dayObject = canUseChildList [dayIndex].gameObject;
				nightObject = dayObject;
			} 
			else if (SwitchType == SwitchType.FrzzedAfterInitItem) {
//				if (Random.value < 0.01f) {
//					dayObject = null;
//				} else {
//					int dayIndex = Random.Range (0, canUseChildList.Count);
//					dayObject = canUseChildList [dayIndex].gameObject;
//					nightObject = dayObject;
//					dayObject.SetActive (true);
//				}

				int dayIndex = Random.Range (0, canUseChildList.Count);
				dayObject = canUseChildList [dayIndex].gameObject;
				nightObject = dayObject;
				dayObject.SetActive (true);
			}
			else if(SwitchType == SwitchType.RandomSwitchItem) {
//				if (Random.value < 0.01f) {
//					dayObject = null;
//				} else {
//					int dayIndex = Random.Range (0,canUseChildList.Count);
//					dayObject = canUseChildList [dayIndex].gameObject;
//					canUseChildList.RemoveAt (dayIndex);
//				}

				int dayIndex = Random.Range (0,canUseChildList.Count);
				dayObject = canUseChildList [dayIndex].gameObject;
				canUseChildList.RemoveAt (dayIndex);

				int nightIndex = Random.Range (0,canUseChildList.Count);
				//Debug.Log ("@@@@@@@@@@ " + nightIndex + "  " + canUseChildList.Count + "    " +  gameObject.name);
				nightObject = canUseChildList [nightIndex].gameObject;
			}

			IsNoChildrenObject = false;
		} else {

			dayObject = transform.gameObject;
			nightObject = transform.gameObject;

			IsNoChildrenObject = true;
		}

		if (dayObject != null) {
			dayCopyMaterial = dayObject.GetComponent<MeshRenderer> ().material;
		}

		if (nightObject != null) {
			nightCopyMaterial = nightObject.GetComponent<MeshRenderer> ().material;
		}


		SwitchDayNight (true);
	}

	public override bool TriggerPlayerPickAction()
	{
		if (SwitchType == SwitchType.Null || SwitchType == SwitchType.FrzzedAfterInitItem) {
			//pick wrong item

			return false;
		} else {
			IsFoundByPlayer = true;
			IgnorePick = true;
			ShaderToFounded (dayCopyMaterial);
			ShaderToFounded (nightCopyMaterial);
			return true;
		}
	}

	public void PrepareSwitch(bool isSwitchToDay)
	{
		switch (SwitchType) {
		case SwitchType.RandomSwitchItem:
			{
				if (isSwitchToDay) {
					if (IsFoundByPlayer) {
						ShaderToNormal (nightCopyMaterial);
					}
				} else {
					if (IsFoundByPlayer) {
						ShaderToNormal (dayCopyMaterial);
					}
				}
			}
			break;

		default:
			break;
		}
	}

	public void SwitchDayNight(bool isDay)
	{
		switch (SwitchType) {
		case SwitchType.RandomSwitchItem:
			{
				if (isDay) {
					if(dayObject != null)
						dayObject.SetActive (true);

					if (nightObject == null) {
						Debug.LogError (name);
					}

					nightObject.SetActive (false);
				} else {
					if(dayObject != null)
						dayObject.SetActive (false);
					
					nightObject.SetActive (true);
				}
			}
			break;

		default:
			break;
		}
	}

	public void EndSwitch(bool isSwitchToDay)
	{
		switch (SwitchType) {
		case SwitchType.RandomSwitchItem:
			{
				if (!isSwitchToDay) {
					if (IsFoundByPlayer) {
						ShaderToFounded (nightCopyMaterial);
					}
				} else {
					if (IsFoundByPlayer) {
						ShaderToFounded (dayCopyMaterial);
					}
				}
			}
			break;

		default:
			break;
		}
	}
	void SetMeshHighlight(Material copyMaterial,bool hightLight)
	{
		if (hightLight) {
			ShaderToSelected (copyMaterial);
		} else {
			ShaderToNormal (copyMaterial);
		}
	}

	public override void OnRayEnter()
	{
		if (DeviceContainer.Instance.DayNightSwitcher_Current == null)
			return;
		
//		if (DeviceContainer.Instance.DayNightSwitcher_Current.IsDay)
//			return;
		
		if (IsFoundByPlayer)
			return;

		if (DeviceContainer.Instance.DayNightSwitcher_Current.IsDay) {
			SetMeshHighlight (dayCopyMaterial,true);
		} else {
			SetMeshHighlight (nightCopyMaterial,true);
		}

	}

	public override void OnRayExit()
	{
		if (IsFoundByPlayer)
			return;

		if (DeviceContainer.Instance.DayNightSwitcher_Current != null) {
			if (DeviceContainer.Instance.DayNightSwitcher_Current.IsDay) {
				SetMeshHighlight (dayCopyMaterial,false);
			} else {
				SetMeshHighlight (nightCopyMaterial,false);
			}
		}
	}

	void OnDestroy()
	{
		Object.Destroy (dayCopyMaterial);
		Object.Destroy (nightCopyMaterial);
	}

	public void ShaderToNormal(Material copyMaterial)
	{
		if (copyMaterial == null)
			return;
		copyMaterial.shader = DeviceContainer.Instance.normalShader;
	}

	public void ShaderToSelected(Material copyMaterial)
	{
		if (copyMaterial == null)
			return;

		copyMaterial.shader = DeviceContainer.Instance.selectedShader;
		copyMaterial.SetColor ("_Fresnel_Color", new Color (1, 153 / 255f, 0));
		//copyMaterial.SetFloat ("_Outline_Width", 0.05f);
		copyMaterial.SetFloat ("_Pulse_Speed", 6);
		copyMaterial.SetFloat ("_Fresnel_Width", 1.5f);
		//copyMaterial.SetFloat ("_FresMultOut", 0.5f);
		copyMaterial.SetFloat ("_Corrective_Glow", 1);
		copyMaterial.SetFloat ("_Emission_Intensity", 1);
	}

	public void ShaderToFounded(Material copyMaterial)
	{
		copyMaterial.shader = DeviceContainer.Instance.selectedShader;
		copyMaterial.SetColor ("_Fresnel_Color", new Color (0, 1, 0));
		//copyMaterial.SetFloat ("_Outline_Width", 0.05f);
		copyMaterial.SetFloat ("_Pulse_Speed", 6);
		copyMaterial.SetFloat ("_Fresnel_Width", 1.5f);
		//copyMaterial.SetFloat ("_FresMultOut", 0.5f);
		copyMaterial.SetFloat ("_Corrective_Glow", 1);
		copyMaterial.SetFloat ("_Emission_Intensity", 1);
	}
}
