using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleTimer : MonoBehaviour
{
	private Coroutine timmer;
	[HideInInspector]public bool isOff = true;
	[HideInInspector]public bool isOn = false;
	[HideInInspector]public bool isGreen = false;
	[HideInInspector]public bool isRed = false;

	public GameObject off;
	public GameObject on;
	public GameObject green;
	public GameObject red;
	public GameObject enemySpawner;
	public GameObject enemy;
	public GameObject arrowUI;

    void Update()
    {
		off.gameObject.SetActive(isOff);
		on.gameObject.SetActive(isOn);
		green.gameObject.SetActive(isGreen);
		red.gameObject.SetActive(isRed);

		if (!arrowUI.GetComponent<Animation>().isPlaying)
			arrowUI.GetComponent<Animation>().Play();
    }

	public void StartTimer(float time)
	{
		if (timmer != null)
			StopCoroutine(timmer);

		timmer = StartCoroutine(Timmer(time));

		isOff = false;
		isOn = false;
		isGreen = true;
		isRed = false;
	}

	private IEnumerator Timmer(float time)
	{
		yield return new WaitForSeconds(time);

		isOff = false;
		isOn = false;
		isGreen = false;
		isRed = true;

		GameObject newEnemy = Instantiate(enemy, enemySpawner.transform.position, enemySpawner.transform.rotation);
		newEnemy.gameObject.SetActive(true);
	}

	public void TurnOn()
	{
		isOff = false;
		isOn = true;
		isGreen = false;
		isRed = false;
	}

	public void StayGreen()
	{
		this.gameObject.transform.Find("Trigger").GetComponent<Collider>().enabled = false;
		this.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
		StopCoroutine(timmer);
		isOff = false;
		isOn = false;
		isGreen = true;
		isRed = false;
	}
}
