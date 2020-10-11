using UnityEngine;

public class FollowTarget : MonoBehaviour 
{
	//[HideInInspector]
	public GameObject target;
	private Vector3 offset;
	
	void Start()
	{
		offset = gameObject.transform.position;
	}

	void Update () 
	{
		if (target != null)
		{
			transform.position = target.transform.position + offset;
		}
	}

	public void SetTarget(GameObject newTarget)
	{
		target = newTarget;
	}
}
