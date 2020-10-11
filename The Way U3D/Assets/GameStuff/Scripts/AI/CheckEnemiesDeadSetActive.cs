using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemiesDeadSetActive : MonoBehaviour {
    public GameObject activeObject;
    private bool run= true;

    // Use this for initialization
    void Update()
    {
        if (run)
        {
            run = false;
            bool setActive = true;

            Transform[] ts = GetComponentsInChildren<Transform>();
            foreach (Transform t in ts)
            {
                Transform[] child = t.GetComponentsInChildren<Transform>();
                foreach (Transform c in child)
                {
                    if (c.gameObject.name == "Animator")
                    {
                        setActive = false;
                        break;
                    }
                }
            }

            if (setActive)
            {
                activeObject.SetActive(true);
            }
        }
        StartCoroutine(SetRunTrue(1));
    }

    IEnumerator SetRunTrue(float time)
    {
        yield return new WaitForSeconds(time);
        run = true;
    }
}
