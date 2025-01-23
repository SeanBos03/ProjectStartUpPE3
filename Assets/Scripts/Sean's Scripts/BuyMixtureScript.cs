using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyMixtureScript : MonoBehaviour
{
    public void BuyMixture(GameData.Mixture theMixture)
    {
        bool repeatMixture = false;
        foreach (GameData.Mixture knownMixture in GameData.mixturesKnown)
        {
            if (knownMixture.CheckMixtureMatched(theMixture))
            {
                repeatMixture = true;
                break;
            }
        }

        if (!repeatMixture)
        {
            if (theMixture.CheckMixtureValid())
            {
                GameData.mixturesKnown.Add(theMixture);
                Debug.Log("Mixture learned");
            }

            else
            {
                Debug.Log("Invalid mixture");
            }
            
        }
        
    }

    public void SwitchScene(string theScene)
    {
        GameData.SwitchScene(theScene);
    }
}
