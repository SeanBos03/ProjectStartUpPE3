using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int theHealth;
    public List<string> elementsResistant = new List<string>();
    public List<int> elementsResistantValues = new List<int>();
    [HideInInspector] public int stunBar;
    public int stunThreshold; //threshold the stunBar can reach before character turns stunned
    [HideInInspector] public bool isStuned;
    public int turnsStunned; //amount of turns enemy is stunned
    int turnsStunnedCurrentValue; 

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

    public void DealStun(int theValue)
    {
        if (isStuned)
        {
            return;
        }

        stunBar += theValue;

        if (stunBar >= stunThreshold)
        {
            stunBar = stunThreshold;
            isStuned = true;
            turnsStunnedCurrentValue = turnsStunned;
        }

        if (stunBar < 0)
        {
            stunBar = 0;
        }
    }

    //for x amount of turns (as long as turnsStunnedCurrentValue is greater than 0), enemy is stunned
    //this will decrease the amount of turns of the current value by 1 and see if it's over
    public void TryCeaseStun()
    {
        Debug.Log(turnsStunnedCurrentValue);
   //     Debug.Log("isStuned: " + isStuned);
        if (turnsStunnedCurrentValue <= 0) //this should never be ture, but just in case
        {
            isStuned = false;
      //      Debug.Log("Enemy got stunned 1");
            return;
        }

        if (isStuned)
        {
            stunBar = 0;
            turnsStunnedCurrentValue--;

            if (turnsStunnedCurrentValue <= 0)
            {
       //         Debug.Log("Enemy got stunned 2");
                isStuned = false;
            }
        }
        
    }
}
