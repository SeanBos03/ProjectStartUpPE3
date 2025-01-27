using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int theHealth;
    public List<string> elementsResistant = new List<string>();
    public List<int> elementsResistantValues = new List<int>();

    void Start()
    {
        theHealth = maxHealth;

        if (elementsResistant.Count != elementsResistant.Count)
        {
            Debug.Log("Character resistance values not valid");
        }
    }

    public int DetermineResistance(List<string> theElements)
    {
        int sumResistanceSum = 0;
        foreach (string element in theElements)
        {
            for (int i = 0; i < elementsResistant.Count; i++)
            {
                if (element == elementsResistant[i])
                {
                    sumResistanceSum += elementsResistantValues[i];
                    break;
                }
            }
        }
        return sumResistanceSum;
    }
}
