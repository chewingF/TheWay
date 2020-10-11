using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILagBehind : MonoBehaviour 
{
	public float smooth = 1f;
	public float increaseFactor = 2f;

	public Controller controller;

	private Vector3 defaultRot;
	private Vector3 newRot;
	private float lastX;
	private float lastY;
	

	void Start () 
	{
		defaultRot = new Vector3(0,0,0);
		newRot = defaultRot;

		//newX = defaultRot.x;
		//newY = defaultRot.y;
	}
	
	void Update () 
	{
		if (Mathf.Abs(controller.camxAxis * increaseFactor) > Mathf.Abs(lastX))
		{
			newRot.x = controller.camxAxis * increaseFactor;
		}
		if (Mathf.Abs(controller.camyAxis * increaseFactor) > Mathf.Abs(lastY))
		{
			newRot.y = controller.camyAxis * increaseFactor;
		}

		// if(controller.camxAxis * increaseFactor > newRot.x)
		// {
		// 	newRot.x = controller.camxAxis * increaseFactor;
		// }
		// if(controller.camyAxis * increaseFactor > newRot.y)
		// {
		// 	newRot.y = controller.camyAxis * increaseFactor;
		// }

		//newRot.x = newX;
		//newRot.y = newY;

		

		GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.Lerp(transform.localRotation.eulerAngles, newRot, smooth));

		//Debug.Log(newRot);

		newRot = Vector3.Lerp(newRot, defaultRot, smooth);
		

		lastX = controller.camxAxis * increaseFactor;
		lastY = controller.camyAxis * increaseFactor;

		//Mathf.Lerp(DOF.focusDistance, newFocus, Time.deltaTime * smooth);
	}
}
