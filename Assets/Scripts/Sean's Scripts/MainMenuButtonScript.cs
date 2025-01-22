using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonScript : MonoBehaviour
{
    public void PrintMessage()
    {
        Debug.Log("button clicked");

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
