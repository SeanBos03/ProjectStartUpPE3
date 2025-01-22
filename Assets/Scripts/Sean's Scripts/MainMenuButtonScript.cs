using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene("TheScene");
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
