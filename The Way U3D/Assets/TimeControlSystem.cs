using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class TimeControlSystem : MonoBehaviour
{
	public float newTimeScale = 1;
	public float smooth = 1;

    void Start()
    {

    }

    void Update()
    {
		Clock globalClock = Timekeeper.instance.Clock("Root");

		if (hardInput.GetKeyDown("IncreaseTimeScale"))
			newTimeScale += 1;

		if (hardInput.GetKeyDown("DecreaseTimeScale"))
			newTimeScale -= 1;

		if (hardInput.GetKeyDown("ResetTimeScale"))
			newTimeScale = 1;

		globalClock.localTimeScale = Mathf.Lerp(globalClock.localTimeScale, newTimeScale, Time.unscaledDeltaTime * smooth);
		
		if (globalClock.localTimeScale >= 0)
		{
			Time.timeScale = globalClock.localTimeScale;
		}
    }
}
