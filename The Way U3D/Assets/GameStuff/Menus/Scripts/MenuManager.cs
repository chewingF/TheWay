using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    // Game Objects
    public GameObject menuCamera;
    public GameObject menuUI;
    public Material originalBlur;
    private Material mainMenuBlur;
    private Material levelSelectBlur;

    public GameObject mainMenu;
    public GameObject levelSelectMenu;

    private float mainMenuNewBlur = 0;
    private float levelSelectMenuNewBlur = 0;

    public float smooth;

    private bool startMenu = true;
    private bool canStart = false;

    void Start()
    {
        AudioManager.instance.Play("MenuBGMusic");
        Time.timeScale = 1;
        EMMotionManager.Open("MenuStartTitle");
        StartCoroutine(OpenEMAnimationDelayBySeconds(0.5f, "MenuStartAction"));
        menuCamera.GetComponent<Animation>().Play("StartMenu_CameraFlyIn");

        Component[] mainMenuTransform = mainMenu.GetComponentsInChildren(typeof(Transform), true);
        mainMenuBlur = Instantiate(originalBlur);
        foreach (Transform item in mainMenuTransform)
        {
            if (item.gameObject.name == "Blur")
            {
                item.GetComponent<Image>().material = mainMenuBlur;
            } 
        }
        Component[] levelSelectTransform = levelSelectMenu.GetComponentsInChildren(typeof(Transform), true);
        levelSelectBlur = Instantiate(originalBlur);
        foreach (Transform item in levelSelectTransform)
        {
            if (item.gameObject.name == "Blur")
            {
                item.GetComponent<Image>().material = levelSelectBlur;
            }
        }
    }

    void Update()
    {
        if (hardInput.GetKeyDown("Enter") && startMenu && canStart)
        {
            mainMenuNewBlur = 250f;
            levelSelectMenuNewBlur = 0f;

            ChangeFocus(1.93f, 0.5f);
            EMMotionManager.Close("MenuStartAction");
            EMMotionManager.Close("MenuStartTitle");
            menuCamera.GetComponent<Animation>().Play("StartMenu_CameraFlyOut");
            startMenu = false;
            AudioManager.instance.Play("MenuEnterKey");
        }

        mainMenuBlur.SetFloat("_BlurSize", Mathf.Lerp(mainMenuBlur.GetFloat("_BlurSize"), mainMenuNewBlur, Time.deltaTime * smooth));
        levelSelectBlur.SetFloat("_BlurSize", Mathf.Lerp(levelSelectBlur.GetFloat("_BlurSize"), levelSelectMenuNewBlur, Time.deltaTime * smooth));
    }

    // Changes the camera focus
    private void ChangeFocus(float value, float smooth)
    {
        menuCamera.GetComponent<ChangeCameraEffects>().ChangeFocus(value, smooth);
    }

    // Delay for Open Animation (EM)
    private IEnumerator OpenEMAnimationDelayBySeconds(float time, string name)
    {
        yield return new WaitForSeconds(time);
        EMMotionManager.Open(name);
    }
	
    // Delay for Close Animation (EM)
    private IEnumerator CloseEMAnimationDelayBySeconds(float time, string name)
    {
        yield return new WaitForSeconds(time);
        EMMotionManager.Close(name);
    }

    // Closes the game 
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetNewMainMenuBlur(float amount)
    {
        mainMenuNewBlur = amount;
    }
    public void SetNewLevelSelectBlur(float amount)
    {
        levelSelectMenuNewBlur = amount;
    }

    public void SetLevelSelectBlurRaw(float amount)
    {
        levelSelectMenuNewBlur = amount;
        levelSelectBlur.SetFloat("_BlurSize", amount);
    }

    public void SetCanStart()
    {
        canStart = true;
    }
}
