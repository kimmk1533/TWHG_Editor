using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class __SceneManager : MonoBehaviour
{
    public static void LoadScene(string Scenename)
    {
        SceneManager.LoadScene(Scenename, LoadSceneMode.Single);
    }

    public static void LoadGameScene()
    {
        LoadScene("GameScene");
    }
    public static void LoadMainMenuScene()
    {
        LoadScene("MainMenuScene");
    }
    public static void QuitGame()
    {
        Application.Quit();
    }
}
