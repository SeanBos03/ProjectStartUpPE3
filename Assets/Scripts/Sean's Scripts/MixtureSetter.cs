using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixtureSetter : MonoBehaviour
{
    [SerializeField] List<GameData.Mixture> mixtureListKnownAtStart = new List<GameData.Mixture>();
    [SerializeField] List<string> theElements = new List<string>();

    void Start()
    {
        if (GameData.startedMixtureSet)
        {
            return;
        }

        foreach (string theElement in theElements)
        {
            GameData.elementList.Add(theElement);
        }

        GameData.startedMixtureSet = true;

        foreach (GameData.Mixture mixture in mixtureListKnownAtStart)
        {
            if (mixture.CheckMixtureValid())
            {
                GameData.mixturesKnown.Add(mixture);
            }

            else
            {
                Debug.Log("Invalid mixture not added");
            }
            
        }
    }
}
