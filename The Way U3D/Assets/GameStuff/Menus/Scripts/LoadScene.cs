using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void ReloadScene()
    {
        AudioManager.instance.StopAll();
        
        if (SceneManager.GetActiveScene().name == "TestScene")
            TestScene();
        else if (SceneManager.GetActiveScene().name == "MainMenu")
            MenuScene();
        else if (SceneManager.GetActiveScene().name == "CombatPuzzle")
            CombatPuzzleScene();
        else if (SceneManager.GetActiveScene().name == "Level_h")
            MazeScene();
        else if (SceneManager.GetActiveScene().name == "tutorial room")
            TutorialScene();
        else if (SceneManager.GetActiveScene().name == "BossBattle")
            BossBattleScene();
        else if (SceneManager.GetActiveScene().name == "GiantLaser")
            GiantLaserScene();
        else if (SceneManager.GetActiveScene().name == "TimeJumpTest")
            TimeJumpScene();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void TestScene()
    {
        AudioManager.instance.StopAll();
        SceneManager.LoadScene("TestScene");
    }
    public void MenuScene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("MenuBGMusic");
        SceneManager.LoadScene("MainMenu");
    }
    public void CombatPuzzleScene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("BGMusic_CreatureFromTheCrackLagoon");
        SceneManager.LoadScene("CombatPuzzle");
    }
    public void MazeScene()
    {
        AudioManager.instance.StopAll();
        //AudioManager.instance.Play("BGMusic_TheCreepingBlob");
        SceneManager.LoadScene("Level_h");
    }
    public void TutorialScene()
    {
        AudioManager.instance.StopAll();
        //AudioManager.instance.Play("BGMusic_TheCreepingBlob");
        SceneManager.LoadScene("tutorial room");
    }
    public void BossBattleScene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("BGMusic_TroubleOnMercury");
        SceneManager.LoadScene("BossBattle");
    }
    public void GiantLaserScene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("BGMusic_FactoryOnMercury");
        SceneManager.LoadScene("GiantLaser");
    }
    public void TimeJumpScene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("BGMusic_TheCreepingBlob");
        SceneManager.LoadScene("TimeJumpTest");
    }
    public void PuzzleX4Scene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("BGMusic_TheCreepingBlob");
        SceneManager.LoadScene("PuzzleX4");
    }
    public void PuzzleX6Scene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("BGMusic_TheCreepingBlob");
        SceneManager.LoadScene("PuzzleX6");
    }
    public void PuzzleX0Scene()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.Play("BGMusic_TheCreepingBlob");
        SceneManager.LoadScene("PuzzleX0");
    }
}