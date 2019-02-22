using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public void StartGame()
    {
        SceneManager.LoadScene("MultiPlayerReadyScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Help()
    {
        SceneManager.LoadScene("HelpScene");
    }

}
