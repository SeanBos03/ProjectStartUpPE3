using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static List<Combo> learnedCombos = new List<Combo>();
    public static List<Combo> shopCombos = new List<Combo>();
    public struct Combo
    {
        public string ingredient1;
        public string ingredient2;
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
}