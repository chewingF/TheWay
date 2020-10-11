// Copyright © 2017, Nathan Chapman

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
    // Script Object
    private TriggerManager gameManager;
    private PlayerBehaviour recordedClone;

    // Types of Interactions
    [HideInInspector]public bool interactable = true;
    [HideInInspector]public bool subtitles = false;
    [HideInInspector]public bool autoSubtitles = false;
    [HideInInspector]public bool buttonProgression = false;
    [HideInInspector]public bool pauseWhileSubtitles = false;
    [HideInInspector]public bool oneTimeActivate = false;
    [HideInInspector]public bool triggerOnSubs = false;
    [HideInInspector]public bool triggerAfterSubs = false;

    // Internal Tracking Stuff
    private bool canInteract = false;

    // The Subtitle Text
    public List<string> subtitleText = new List<string>();

    // The Buttons that have the interaction stuff
    [HideInInspector]public List<GameObject> interactionObjects = new List<GameObject>();

    // The delay bettween sets of text
    [HideInInspector]public bool dynamicDelay = true;
    [HideInInspector]public int wordsPerSecond = 3;
    [HideInInspector]public float textDelay = 4f;

    [HideInInspector]public string interactText = "Press 'E' to Interact";

    [HideInInspector]public bool cloneActivated = false;

    

    
    void Start()
    {
        // Finding the GameManager - Is required in same scene!
        if (GameObject.Find("GameManager") != null)
            gameManager = GameObject.Find("GameManager").GetComponent<TriggerManager>();
        else
            Debug.Log("GameManager not found!");

        // Makes sure subtitles list is empty before adding
        interactionObjects.Clear();

        // Finding and storing subtitle text
        foreach (Transform child in transform)
        {
            interactionObjects.Add(child.gameObject);
        }
    }

    void Update()
    {
        // If the clone is inside the trigger and activates it
        if (recordedClone != null)
        {
            if (recordedClone.interactPressed)
                cloneActivated = true;
        }

        // If the player is inside the trigger and Interactable is enabled
        if (canInteract && hardInput.GetKeyDown("Interact"))
        {
            gameManager.SetInteractable(false, interactText);
            if (subtitleText != null && subtitles)
            {
                gameManager.SetSubtitleText(subtitleText, true, dynamicDelay, textDelay, wordsPerSecond, buttonProgression, pauseWhileSubtitles, triggerAfterSubs, this.gameObject);
            }
            
			ActivateInteract();

            if(oneTimeActivate)
                this.gameObject.GetComponent<Collider>().enabled = false;

            canInteract = false;
        }

        // If the clone is inside the trigger and activates it
        if (cloneActivated)
        {
			ActivateInteract();
            cloneActivated = false;
        }
    }

    // When player enters the trigger
    private void OnTriggerEnter(Collider col)
    {
        // If Interactable is enabled
        if ((col.tag == "Player") && interactable)
        {
            gameManager.SetInteractable(true, interactText);
            canInteract = true;
        }

        // If Subtitles is enabled
		if ((col.tag == "Player") && subtitles && autoSubtitles && subtitleText != null)
		{
			gameManager.SetSubtitleText(subtitleText, true, dynamicDelay, textDelay, wordsPerSecond, buttonProgression, pauseWhileSubtitles, triggerAfterSubs, this.gameObject);

            // Activates interaction if activate on subs is enabled
            if (triggerOnSubs)
                ActivateInteract();

            // Deactivates its trigger if one time only 
            if(oneTimeActivate)
                this.gameObject.GetComponent<Collider>().enabled = false;
		}

        if (col.tag == "RecordedPlayer")
        {
            recordedClone = col.gameObject.GetComponent<PlayerBehaviour>();
        }
    }

    // When player leaves the trigger
    private void OnTriggerExit(Collider col)
    {
        // If Interactable is enabled
        if ((col.tag == "Player") && interactable)
        {
            gameManager.SetInteractable(false, interactText);
            canInteract = false;
        }

        if (col.tag == "RecordedPlayer")
        {
            recordedClone = null;
        }
    }

    private void ActivateInteract()
    {
        // Interact Stuff Here
        Component[] buttons = GetComponentsInChildren(typeof(Button), true);
        foreach (Button item in buttons)
        {
            if (item.gameObject.name == "1")
                item.onClick.Invoke();
        }
    }
}
