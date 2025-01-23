using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static List<Combo> combosKnown = new List<Combo>();
    public static List<Mixture> mixturesKnown = new List<Mixture>();
    public static bool startedMixtureSetted = false;
    public static List<String> elementList = new List<String>(); //list of elements

    [System.Serializable]
    public struct Mixture
    {

        public List<string> theElements;
        public int value;
        public bool CheckMixtureMatched(Mixture other)
        {
            var thisList = theElements.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var otherList = other.theElements.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            return thisList.Count == otherList.Count && !thisList.Except(otherList).Any();
        }
        public bool CheckMixtureValid()
        {
            int amountElement = 0;
            List<String> elementListCompare = new List<String>();

            foreach (String element in elementList)
            {
                elementListCompare.Add((string)element.Clone());
            }

            foreach (string theCardElement in theElements)
            {
                foreach (String element in elementListCompare)
                {
                    if (theCardElement == element)
                    {
                        amountElement++;
                        elementListCompare.Remove(element);
                        break;
                    }
                }

                if (amountElement > 2)
                {
                    return false;
                }
            }

            foreach (String theCardElement in theElements)
            {
                if (!elementList.Contains(theCardElement))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public struct Combo
    {
        public string ingredient1;
        public string ingredient2;
        public bool isStrong;
        public bool CheckComboMatched(Combo other)
        {
            if ((ingredient1 == other.ingredient1 && ingredient2 == other.ingredient2)
                ||
                (ingredient1 == other.ingredient2 && ingredient2 == other.ingredient1))
            {
                return true;
            }
            return false;
        }
    }

    public static void SwitchScene(string theScene)
    {
        SceneManager.LoadScene(theScene);
    }
}