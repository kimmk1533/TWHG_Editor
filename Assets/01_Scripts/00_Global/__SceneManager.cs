using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class __SceneManager : Singleton<__SceneManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string Scenename)
    {
        SceneManager.LoadScene(Scenename, LoadSceneMode.Single);
    }

    public void LoadGameScene()
    {
        LoadScene("GameScene");
    }
    public void LoadMainMenuScene()
    {
        LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
