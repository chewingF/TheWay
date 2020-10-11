// Copyright © 2017, Nathan Chapman

using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    // Elements Objects
    public GameObject interactions;
    public GameObject subtitles;

    // Active States
    //private bool interactionsIsActive = false;
    //private bool subtitlesIsActive = false;

    //private bool stopPreviousSubs = false;

    // Coroutines
    private Coroutine InteractFadeOut;
    private Coroutine SubDelay;
    private Coroutine SubFadeOut;

    // The FadeIn Animation Clips
    private Animation interactFadeIn;
    private Animation subFadeIn;

    // For Fading Blur
    public Material interactBlur;
    private float interactBlurNew;
    public Material subBlur;
    private float subBlurNew;
    public float smooth;

    // Other From Trigger Event
    private bool btnPressed = false;
    private bool triggerAfterSubtitles;
    private GameObject triggerObj;
    [HideInInspector]public bool usingButtonPress = false; 


    void Start()
    {
        subFadeIn = subtitles.GetComponent<Animation>();
        interactFadeIn = interactions.GetComponent<Animation>();
        interactBlur.SetFloat("_Radius", 0f);
        subBlur.SetFloat("_Radius", 0f);
        interactions.gameObject.SetActive(false);
        subtitles.gameObject.SetActive(false);
    }

    void Update()
    {
        // Updates Frosted Glass
        interactBlur.SetFloat("_Radius", Mathf.Lerp(interactBlur.GetFloat("_Radius"), interactBlurNew, Time.unscaledDeltaTime * smooth));
        subBlur.SetFloat("_Radius", Mathf.Lerp(subBlur.GetFloat("_Radius"), subBlurNew, Time.unscaledDeltaTime * smooth));

        if (!btnPressed)
        {
            if (hardInput.GetKeyDown("Enter"))
            {
                btnPressed = true;
            }
        }
    }

    // Shows/Hides Interactable Text
    public void SetInteractable(bool activeState, string interactionText)
    {
        if (InteractFadeOut != null)
            StopCoroutine(InteractFadeOut);

        if (activeState)
        {
            interactions.gameObject.SetActive(true);
            interactions.GetComponent<Animate>().DoAnimation();
            interactBlurNew = 3f;

            if (interactionText != null)
                interactions.transform.Find("Text").GetComponent<Text>().text = interactionText;
        }
        else if (!activeState && ((interactions.activeSelf && !interactions.GetComponent<Animation>().isPlaying) || (interactions.activeSelf && interactions.GetComponent<Animation>().isPlaying && interactFadeIn[interactFadeIn.clip.name].speed > 0)))
        {
            interactBlurNew = 0f;
            interactions.GetComponent<Animate>().DoAnimation();
            InteractFadeOut = StartCoroutine(InteractableFadeOut());
        }
    }

    // Starts the Subtitles
    public void SetSubtitleText(List<string> newText, bool activeState, bool useDynamic, float textDelay, int WPS, bool btnProgression, bool pauseGame, bool triggerAfterSubs, GameObject trigger)
    {
        if (SubDelay != null)
            StopCoroutine(SubDelay);

        if ((subtitles.GetComponent<Animation>().isPlaying && subFadeIn[subFadeIn.clip.name].speed < 0))
        {
            StopCoroutine(SubFadeOut);
            subtitles.gameObject.SetActive(true);
            subtitles.GetComponent<Animate>().DoAnimation();
            Debug.Log("Stoping Animation Playing Backwards!"); 
        }
        else if (!subtitles.activeSelf)
        {
            subtitles.gameObject.SetActive(true);
            subtitles.GetComponent<Animate>().DoAnimation();
        }
        subBlurNew = 3f;
        triggerAfterSubtitles = triggerAfterSubs;
        triggerObj = trigger;
        usingButtonPress = btnProgression;
        SubDelay = StartCoroutine(SubtitleDelay(newText, useDynamic, textDelay, WPS, btnProgression, pauseGame));
    }

    // Cysles through each subtitle page
    private IEnumerator SubtitleDelay(List<string> subs, bool isDynamic, float amount, int wordsPerSec, bool btnProgression, bool pauseGame)
    {
        // Sets the delay timer
        float count = amount;

        // Loops through each page
        for (int i = 0; i < subs.Count; i++)
        {
            btnPressed = false;
            // If set to dynamic, will change spped based on number of words/words per second
            if (isDynamic)
            {
                count = subs[i].Split(' ').Length;
                count = count / wordsPerSec;
            }

            // Sets the display text
            subtitles.transform.Find("Text").GetComponent<Text>().text = subs[i];

            if (!btnProgression)
            {
                if (subtitles.GetComponent<Animation>().isPlaying)
                {
                    if (pauseGame)
                    {
                        yield return new WaitForSecondsRealtime(1);
                        this.GetComponent<PauseMenu>().PauseGame();
                        this.GetComponent<PauseMenu>().ActivateOrDeactivatePlayer(false);
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    // FadeIn Delay
                    yield return new WaitForSecondsRealtime(count);
                }
                else
                {
                    // Waits to show next page
                    yield return new WaitForSecondsRealtime(count);
                }
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GameObject.Find("Player").GetComponent<PlayerBehaviour>().subtitlesActive = true;
                //if (i == subs.Count - 1)
                //    subtitles.transform.FindDeepChild("Button").GetChild(0).GetComponent<Text>().text = "enter          to          close";
                //else
                //    subtitles.transform.FindDeepChild("Button").GetChild(0).GetComponent<Text>().text = "enter          to          proceed";

                if (pauseGame && subtitles.GetComponent<Animation>().isPlaying)
                {
                    yield return new WaitForSecondsRealtime(1);
                    this.GetComponent<PauseMenu>().PauseGame();
                    this.GetComponent<PauseMenu>().ActivateOrDeactivatePlayer(false);
                    
                }
                yield return new WaitUntil(() => btnPressed);
            }
        }

        // Plays closeing animation
        if ((subtitles.GetComponent<Animation>().isPlaying && subFadeIn[subFadeIn.clip.name].speed > 0) || (subtitles.activeSelf && !subtitles.GetComponent<Animation>().isPlaying))
            subtitles.GetComponent<Animate>().DoAnimation();

        // Sets target blur value
        subBlurNew = 0f;

        // Starts fade out
        SubFadeOut = StartCoroutine(SubtitleFadeOut());
    }

    // Fades out Subtitles Text, then disables object
    private IEnumerator SubtitleFadeOut()
    {
        this.GetComponent<PauseMenu>().UnPauseGame();
        this.GetComponent<PauseMenu>().ActivateOrDeactivatePlayer(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        usingButtonPress = false;
        GameObject.Find("Player").GetComponent<PlayerBehaviour>().subtitlesActive = false;

        if (triggerAfterSubtitles)
        {
            // Interact Stuff Here
            Component[] buttons = triggerObj.GetComponentsInChildren(typeof(Button), true);
            foreach (Button item in buttons)
            {
                if (item.gameObject.name == "2")
                    item.onClick.Invoke();
            }
        }

        yield return new WaitUntil(() => !subtitles.GetComponent<Animation>().isPlaying);
        subtitles.gameObject.SetActive(false);
    }

    // Fades out Interactable Text, the disables object
    private IEnumerator InteractableFadeOut()
    {
        yield return new WaitUntil(() => !interactions.GetComponent<Animation>().isPlaying);
        interactions.gameObject.SetActive(false);
    }

    public void SetButtonPressed()
    {
        btnPressed = true;
    }

    // Sound put here because audio manager on buttons get erased 
    public void PlaySound(string sound)
    {
        AudioManager.instance.Play(sound);
    }
    public void StopSound(string sound)
    {
        AudioManager.instance.Stop(sound);
    }
}
