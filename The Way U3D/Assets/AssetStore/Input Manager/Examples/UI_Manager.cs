using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Hard Shell Studios/Examples/UI Manager")]
public class UI_Manager : MonoBehaviour
{
    public GameObject menu;

    public bool isActive;
    int num = 0;

    void Start()
    {
        num = hardInput.GetControllerTypeIndex();
        menu.SetActive(false);
        hardInput.MouseLock(true);
        hardInput.MouseVisible(true);
        isActive = false;
    }


	// Update is called once per frame
	void Update ()
    {
        if (hardInput.GetKeyDown("KeyBindingMenu"))
        {
            if (menu.activeSelf)
            {
                menu.SetActive(false);
                hardInput.MouseLock(true);
                hardInput.MouseVisible(true);
				isActive = false;
				//GameObject.Find("player").GetComponent("FPSWalkerEnhanced") = true;
            }
            else
            {
                menu.SetActive(true);
                hardInput.MouseLock(false);
                hardInput.MouseVisible(true);
				isActive = true;
				//GameObject.Find("player").GetComponent("FPSWalkerEnhanced") = false;
            }
        }

        if (hardInput.GetKeyDown("CycleKeyBindingModes"))
        {
            num++;
            if (num == 4)
                num = 0; 

            if(num == 0)
                hardInput.SetControllerType(hardInput.controllerType.PS3);
            else if (num == 1)
                hardInput.SetControllerType(hardInput.controllerType.PS4);
            else if (num == 2)
                hardInput.SetControllerType(hardInput.controllerType.XBOX1);
            else if (num == 3)
                hardInput.SetControllerType(hardInput.controllerType.XBOX360);

        }


    }
}
