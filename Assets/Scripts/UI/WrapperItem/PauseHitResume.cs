using UnityEngine;
using System.Collections;

public class PauseHitResume : MonoBehaviour {

	public PauseUI ui;
	// Use this for initialization
	void OnTriggerEnter(Collider other) {
		ui.ResumeMusic ();
	}

	void Update()
	{
		var colliders = Physics.OverlapBox(transform.position,new Vector3(0.1f,0.1f,0.01f));
		if (colliders != null) {
			foreach (var collider in colliders) {
				if (collider.name.Contains ("Capsule")) {
					ui.ResumeMusic ();
				}
			}
		}
	}
}
