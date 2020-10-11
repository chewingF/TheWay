using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
	private GameObject fpsCounter;
	private GameObject HUD;

	private bool camerasActive = false;
	private int cameraNumber = 0;

	private Transform previousCamPos;

	private Transform camTargetRot;
	private bool camTargetRotateStart = false;

	
	[Header("Cameras")]
	public List<GameObject> cameras = new List<GameObject>();

    void Start()
    {
		fpsCounter = GameObject.Find("FramerateCounter");
		HUD = GameObject.Find("HUD");
		camTargetRot = GameObject.Find("CameraRotateRoot").transform;

		foreach (GameObject item in cameras)
		{
			item.SetActive(camerasActive);
		}
    }

    void Update()
    {
		if (hardInput.GetKeyDown("ToggleSpyCameras"))
		{
			camerasActive = !camerasActive;

			foreach (GameObject item in cameras)
			{
				item.SetActive(camerasActive);
			}
			fpsCounter.SetActive(!camerasActive);
			HUD.SetActive(!camerasActive);
		}

		if (hardInput.GetKeyDown("CycleSpyCameras"))
		{
			previousCamPos = cameras[cameraNumber].transform;

			if (cameraNumber < (cameras.Count - 1))
				cameraNumber++;
			else
				cameraNumber = 0;

			
			camTargetRotateStart = false;
			camTargetRot.localEulerAngles = new Vector3(0,0,0);

			// if (cameras[cameraNumber].name == "CameraRotate")
			// {
			// 	cameras[cameraNumber].transform.position = previousCamPos.position;
			// 	camTargetRotateStart = true;
			// }
			
			foreach (GameObject item in cameras)
			{
				
				if (item.name != "Camera_Rotate")
				{
					if(item.name == "Camera_Zoom")
					{
						item.GetComponent<Camera>().fieldOfView = 60;
						item.GetComponent<ChangeCameraEffects>().ChangeFOV(10, 3);
					}
					item.SetActive(false);
				}
				else
				{
					item.transform.position = previousCamPos.position;
					camTargetRotateStart = true;
				}
			}

			cameras[cameraNumber].SetActive(true);
		}
		if (camTargetRotateStart)
		{
			camTargetRot.localEulerAngles -= new Vector3(0, 50f * Time.unscaledDeltaTime, 0);
		}

    }
}
