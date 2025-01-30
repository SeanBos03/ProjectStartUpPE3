using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int theHealth;
    public List<string> elementsResistant = new List<string>();
    public List<int> elementsResistantValues = new List<int>();
    public List<string> elementsWeakness = new List<string>();
    public List<int> elementsWeaknessStuntValue = new List<int>();
    [HideInInspector] public int stunBar;
    public int stunThreshold; //threshold the stunBar can reach before character turns stunned
    [HideInInspector] public bool isStuned;
    public int turnsStunned; //amount of turns enemy is stunned
    int turnsStunnedCurrentValue;

    [HideInInspector] List<string> rotationResistance = new List<string>();
    public int lastAttackResistancePercentage;

    void Start()
    {
        theHealth = maxHealth;

        if (elementsResistant.Count != elementsResistant.Count)
        {
            Debug.Log("Character resistance values not valid");
        }

        if (elementsWeakness.Count != elementsWeaknessStuntValue.Count)
        {
            Debug.Log("Character weakness values not valid");
        }

        if (lastAttackResistancePercentage < 0)
        {
            lastAttackResistancePercentage = 0;
        }
    }

    public void RotateResistance(List<GameObject> theCards)
    {
        rotationResistance.Clear();
        rotationResistance.Add(theCards[0].GetComponent<CardObjectImage>().theType);
  //      Debug.Log("get element percentage: " + theCards[0].GetComponent<CardObjectImage>().theType);
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

    public int DetermineWeaknessStunt(List<string> theElements)
    {
        int sumStuntSum = 0;
        foreach (string element in theElements)
        {
            for (int i = 0; i < elementsWeakness.Count; i++)
            {
                if (element == elementsWeakness[i])
                {
                    sumStuntSum += elementsWeaknessStuntValue[i];
                    break;
                }
            }
        }
        return sumStuntSum;
    }

    public bool RotationResistanceCanApply(List<string> theElements)
    {
        //foreach (string element in rotationResistance)
        //{
        //    Debug.Log("Resistanet element per element: " + element);
        //}

        foreach (string element in theElements)
        {
            for (int i = 0; i < rotationResistance.Count; i++)
            {
                if (element == rotationResistance[i])
                {
                    Debug.Log("Deal percentage resistance: " + rotationResistance[i]);
                    return true;
                }
            }
        }
        return false;
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

    public void CheckStunAtAll()
    {
        if (turnsStunnedCurrentValue <= 0) //this should never be ture, but just in case
        {
            isStuned = false;
            //      Debug.Log("Enemy got stunned 1");
            return;
        }
    }

    //for x amount of turns (as long as turnsStunnedCurrentValue is greater than 0), enemy is stunned
    //this will decrease the amount of turns of the current value by 1 and see if it's over
    public void TryCeaseStun()
    {
      //  Debug.Log(turnsStunnedCurrentValue);
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
