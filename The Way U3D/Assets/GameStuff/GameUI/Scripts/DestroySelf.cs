﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
	public float destroyInSec = 3;
    void Start()
    {
		Destroy(gameObject, destroyInSec);
    }
}
