using UnityEngine;
using System.Collections;

/// <summary>
/// Moves Flock leader.
/// </summary>
public class FishLeaderSwimScript : MonoBehaviour {

	internal float timeDelta = 0.1f;
	public float speed = 15.0f;

	Vector3[] swimPath;
	Vector3 currentDir = Vector3.zero;
	Vector3 currentTarget;
	int currentPathIndex;
	bool moveToEnd = false;
	/// <summary>
	/// Start main routine.
	/// </summary>
	void Start () {
	}

	public void StartSwimAtPath(Vector3[] path)
	{
		swimPath = path;
		//StartCoroutine(MainLoop());
		currentPathIndex = 1;
		currentTarget = swimPath[currentPathIndex];
		transform.position = swimPath [0];
		currentDir = (currentTarget - transform.position).normalized;
		transform.LookAt (currentTarget);
	}

	void Update()
	{
		if (currentDir == Vector3.zero)
			return;
		
		if (moveToEnd)
			return;
		
		if (Vector3.Dot (currentTarget - transform.position, currentDir) < 0) {
			currentPathIndex++;
			if (currentPathIndex >= swimPath.Length) {
				moveToEnd = true;
				if (transform.parent != null) {
					GameObject.Destroy (transform.parent.gameObject);				
				}
				else {
					GameObject.Destroy (gameObject);	
				}
				return;
			}
			else {
				currentTarget = swimPath[currentPathIndex];
				currentDir = (currentTarget - transform.position).normalized;
				iTween.LookTo (transform.gameObject, currentTarget, 2);
				//transform.LookAt (currentTarget);
			}
		}
		else
		{
			this.transform.position += currentDir * speed * Time.deltaTime;
		}
	}
}
