using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject playerHead;
    //public GameObject firstPersonCamera;

    void Update()
    {
        this.transform.position = playerHead.transform.position;
    }
}