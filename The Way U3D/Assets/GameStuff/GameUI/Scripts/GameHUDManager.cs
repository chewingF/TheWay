using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUDManager : MonoBehaviour
{
	[HideInInspector] public GameObject player;
	public GameObject healthBar;
	private float maxHealth;
	private float newHealth;
	public GameObject energyBar;
	private float maxEnergy;
	private float newEnergy;

    void Start()
    {
		// Sets the Player
		player = GameObject.Find("Player");

		// Sets the max values
		maxHealth = player.GetComponent<PlayerBehaviour>().life;
		maxEnergy = player.GetComponent<PlayerBehaviour>().energy;

		// Sets the start values
		healthBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxHealth) * maxHealth;
		energyBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxEnergy) * maxEnergy;
    }

    void Update()
    {
		newHealth = player.GetComponent<PlayerBehaviour>().life;
		newEnergy = player.GetComponent<PlayerBehaviour>().energy;

		// Updates Bars
		healthBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxHealth) * newHealth;
		energyBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxEnergy) * newEnergy;

		// Updates Numbers
		healthBar.transform.FindDeepChild("HealthText").GetComponent<Text>().text = Math.Round(newHealth, 0).ToString();
		energyBar.transform.FindDeepChild("EnergyText").GetComponent<Text>().text = Math.Round(newEnergy, 0).ToString();
    }
}
