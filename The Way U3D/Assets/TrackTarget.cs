using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTarget : MonoBehaviour
{
	public Transform target;
	public Transform moveTarget;
	public float trackTargetSpeed = 5;
	public float moveToTargetSpeed = 5;


    void Update()
    {
		if (target != null)
		{
			Vector3 relativePos = target.position - transform.position;
        	Quaternion rotation = Quaternion.LookRotation(relativePos);

			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.unscaledDeltaTime * trackTargetSpeed);
		}
		if (moveTarget != null)
		{
			transform.position = Vector3.Lerp(transform.position, moveTarget.position, Time.unscaledDeltaTime * moveToTargetSpeed);
		}
		
    }
}
