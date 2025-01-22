using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene("Chapter 1 Map");
    }
    public void PrintMessage()
    {
        Debug.Log("button clicked");

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
