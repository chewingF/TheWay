using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBossUI : MonoBehaviour
{
    public GameObject character;
    private GameObject healthBar;
    private float maxHealth;
    private float newHealth;

    void Start()
    {
        healthBar = this.gameObject;

        if (character != null)
        {
            // Sets the max values
            if (character.tag == "Boss")
            {
                maxHealth = character.GetComponent<BossBehaviour>().life;
            }
            if (character.tag == "Giant Laser")
            {
                maxHealth = character.GetComponent<GiantLaserBehaviour>().life;
            }
        }

        // Sets the start values
        healthBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxHealth) * maxHealth;
    }

    void Update()
    {
        if (character != null)
        {
            if (character.tag == "Boss")
            {
                if (character.GetComponent<BossBehaviour>().life > 0)
                {
                    newHealth = character.GetComponent<BossBehaviour>().life;

                    // Updates Bars
                    healthBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxHealth) * newHealth;

                    // Updates Numbers
                    healthBar.transform.FindDeepChild("Text").GetComponent<Text>().text = (Math.Round(newHealth, 0) + "/" + maxHealth);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
            }
            if (character.tag == "Giant Laser")
            {
                if (character.GetComponent<GiantLaserBehaviour>().life > 0)
                {
                    newHealth = character.GetComponent<GiantLaserBehaviour>().life;

                    // Updates Bars
                    healthBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxHealth) * newHealth;

                    // Updates Numbers
                    healthBar.transform.FindDeepChild("Text").GetComponent<Text>().text = (Math.Round(newHealth, 0) + "/" + maxHealth);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
            }

        }

    }
}
