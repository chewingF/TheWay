using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FaceObject : MonoBehaviour 
{

	private Transform target;

	private Text numText;

	void Start () 
	{
		if (GameObject.Find("Player"))
		{
			target = GameObject.Find("Player").transform;
		}
	}
	
	void Update () 
	{
		// transform.LookAt(target);
		// transform.rotation*=Quaternion.Euler(0, 30, 0);

		Vector3 targetPostition = new Vector3(target.position.x, this.transform.position.y, target.position.z);

		
 		this.transform.LookAt(2 * transform.position - targetPostition);

		 //transform.LookAt(2 * transform.position - target.position);

		 //transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
	}

	public void SetRecordingText(int recordingNumber)
	{
		numText = gameObject.GetComponent<Text>();
		numText.text = "GHOST - " + recordingNumber;
	}
}
