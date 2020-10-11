using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
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
            maxHealth = character.GetComponent<AIBehaviour>().life;
        }

        // Sets the start values
        healthBar.transform.FindDeepChild("Fill").GetComponent<Image>().fillAmount = (1 / maxHealth) * maxHealth;
    }

    void Update()
    {
        if (character != null)
        {
            if (character.GetComponent<AIBehaviour>().life > 0)
            {
                newHealth = character.GetComponent<AIBehaviour>().life;

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
