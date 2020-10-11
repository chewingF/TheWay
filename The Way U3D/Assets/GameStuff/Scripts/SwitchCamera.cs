using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Camera Camera1;
    public Camera Camera2;

    // Use this for initialization
    void Start ()
    {
        Camera1.enabled = true;
        Camera2.enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.C))
        {
            if (Camera1.enabled == true)
            {
                Camera1.enabled = false;
                Camera2.enabled = true;
            }
            else if (Camera2.enabled == true)
            {
                Camera1.enabled = true;
                Camera2.enabled = false;
            }
        }
	}
}
