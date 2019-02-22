using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HelpUI : MonoBehaviour {

	public void Back()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
