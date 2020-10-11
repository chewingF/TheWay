using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonBListener : MonoBehaviour {
	private GameObject buttonB;
	// Use this for initialization
	private int count;

	void Start () {
		//buttonB.activeSelf = true;
		buttonB = GameObject.Find("buttonB");
		count = 0;

	}
	
	// Update is called once per frame
	void Update () {
		if (buttonB != null) {
			if (hardInput.GetKeyDown ("WallWalkerAbility")) {
				count = 5;
				buttonB.SetActive (false);
			}
			if (count > 0) {
				Debug.Log ("track count " + count);
				count -= 1;
			} else {
				buttonB.SetActive (true);
			}
		}
	}
}
