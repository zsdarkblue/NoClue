using UnityEngine;
using System.Collections;

public class ExplosionForce : MonoBehaviour {

	float radius = 10f;
	float power = 600f;
	// Use this for initialization
	void Start () {
		Collider[] colliders = Physics.OverlapSphere (transform.position, 10f);
		foreach (var hits in colliders) {
			var body = hits.GetComponent<Rigidbody> ();
			if (body != null) {
				body.AddExplosionForce (power,transform.position,radius);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
