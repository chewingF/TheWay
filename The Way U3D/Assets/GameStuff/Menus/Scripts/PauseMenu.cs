using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class PauseMenu : MonoBehaviour
{
    [HideInInspector]public bool pauseMenuActive = false;
    [HideInInspector]public GameObject player;
    public GameObject gameMenus;
    private TriggerManager triggerManager;

    public Material strongFrostedGlass;
    private float strongFrostedGlassNew;
    public Material weakFrostedGlass;
    private float weakFrostedGlassNew;
    public Material levelSelectMenuBlur;
    public float smooth;
    private bool deadActive = false;
	private bool isPaused = false;

    private bool animCloseStart = false;

    void Start()
    {
        player = GameObject.Find("Player");
		Time.timeScale = 1;
		// Sets Frosted Glass Blur to 0
        strongFrostedGlass.SetFloat("_Radius", 0f);
        weakFrostedGlass.SetFloat("_Radius", 0f);
        levelSelectMenuBlur.SetFloat("_Radius", 0f);
        triggerManager = this.gameObject.GetComponent<TriggerManager>();
    }

    void Update()
    {
        if (hardInput.GetKeyDown("PauseGame"))
        {
            pauseMenuActive = !pauseMenuActive;

			// Opens Menu
            if (pauseMenuActive)
            {
                this.transform.Find("Menu").Find("PauseMenu").gameObject.SetActive(true);
				// If player is dead keep screen blur the same
				if (!player.GetComponent<PlayerBehaviour>().dead)
				{
					gameMenus.transform.Find("ScreenBlur").GetComponent<Animate>().DoAnimation();
					weakFrostedGlass.SetFloat("_Radius", 0f);
					weakFrostedGlassNew = 4f;
					ActivateOrDeactivatePlayer(false);
				}
                gameMenus.transform.Find("PauseMenu").GetComponent<Animate>().DoAnimation();
                strongFrostedGlass.SetFloat("_Radius", 0f);
                strongFrostedGlassNew = 24f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
				isPaused = true;
            }
			// Closes Menu
            else
            {
                animCloseStart = true;
				// If player is dead keep screen blur the same
				if (!player.GetComponent<PlayerBehaviour>().dead)
				{
					gameMenus.transform.Find("ScreenBlur").GetComponent<Animate>().DoAnimation();
					weakFrostedGlass.SetFloat("_Radius", 4f);
					weakFrostedGlassNew = 0f;
					ActivateOrDeactivatePlayer(true);
				}
                gameMenus.transform.Find("PauseMenu").GetComponent<Animate>().DoAnimation();
                strongFrostedGlass.SetFloat("_Radius", 24f);
                strongFrostedGlassNew = 0f;
                UnPauseGame();
                if (!triggerManager.usingButtonPress)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
				isPaused = false;
            }
        }
		// GameOver Screen
        if (player.GetComponent<PlayerBehaviour>().dead && !deadActive)
        {
			EMMotionManager.Open("GameOver");
			ActivateOrDeactivatePlayer(false);
            gameMenus.transform.Find("ScreenBlur").GetComponent<Animate>().DoAnimation();
			gameMenus.transform.Find("ScreenDarken").GetComponent<Animate>().DoAnimation();
            weakFrostedGlass.SetFloat("_Radius", 0f);
            weakFrostedGlassNew = 4f;
			deadActive = true;
        }

		// Pauses Time!
		if (isPaused && !gameMenus.transform.Find("PauseMenu").GetComponent<Animation>().IsPlaying("PauseMenu"))
		{
            PauseGame();
		}
		
		// Updates Frosted Glass
        strongFrostedGlass.SetFloat("_Radius", Mathf.Lerp(strongFrostedGlass.GetFloat("_Radius"), strongFrostedGlassNew, Time.deltaTime * smooth));
        weakFrostedGlass.SetFloat("_Radius", Mathf.Lerp(weakFrostedGlass.GetFloat("_Radius"), weakFrostedGlassNew, Time.deltaTime * smooth));

        if (animCloseStart)
        {
            if (!this.transform.Find("Menu").Find("PauseMenu").GetComponent<Animation>().isPlaying)
            {
                animCloseStart = false;
                this.transform.Find("Menu").Find("PauseMenu").gameObject.SetActive(false);
            }
        }
    }

    // Completley Quits the Game
    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        Clock globalClock = Timekeeper.instance.Clock("Root");
        Time.timeScale = 0.0000001f;
        globalClock.localTimeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnPauseGame()
    {
        Clock globalClock = Timekeeper.instance.Clock("Root");
		Time.timeScale = 1;
        globalClock.localTimeScale = 1;
        if (!triggerManager.usingButtonPress)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ActivateOrDeactivatePlayer(bool answer)
    {
        player.GetComponent<PlayerBehaviour>().enabled = answer;
        player.GetComponent<CameraBehaviour>().enabled = answer;
    }
}
