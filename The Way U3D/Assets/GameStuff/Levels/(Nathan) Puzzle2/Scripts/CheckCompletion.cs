using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCompletion : MonoBehaviour
{
	public GameObject alarmTrigger;
	public GameObject finishTrigger;

    void Update()
    {
		if (alarmTrigger != null)
		{
			int tempCount = 0;
			Component[] consoleTimer = GetComponentsInChildren(typeof(ConsoleTimer), true);
			foreach (ConsoleTimer item in consoleTimer)
			{
				if (item.isGreen)
					tempCount++;
			}
			if (tempCount == consoleTimer.Length)
			{
				finishTrigger.SetActive(true);
				foreach (ConsoleTimer item in consoleTimer)
				{
					item.StayGreen();
				}
			}	
		}
		else
		{
			Debug.Log("'SetOffAlarm' Trigger not found! Puzzle can't be completed!");
		}
    }
}
