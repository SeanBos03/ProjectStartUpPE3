using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameData;

public class ComboCreateScript : MonoBehaviour
{
    public List<string> starterComboIngredient1 = new List<string>();
    public List<string> starterComboIngredient2 = new List<string>();
    public List<string> shopComboIngredient1 = new List<string>();
    public List<string> shopComboIngredient2 = new List<string>();

    void Start()
    {
        if (starterComboIngredient1.Count != starterComboIngredient2.Count)
        {
            Debug.Log("Invalid data - starterCombo");
            return;
        }

        if (shopComboIngredient1.Count != shopComboIngredient2.Count)
        {
            Debug.Log("Invalud data - shopCombo");
            return;
        }

        GameData.learnedCombos.Clear();
        GameData.shopCombos.Clear();
        for (int i = 0; i < starterComboIngredient1.Count; i++)
        {
            Combo comboToBeAdded = new Combo();
            comboToBeAdded.ingredient1 = starterComboIngredient1[i];
            comboToBeAdded.ingredient2 = starterComboIngredient2[i];
            GameData.learnedCombos.Add(comboToBeAdded);
        }
        for (int i = 0; i < shopComboIngredient1.Count; i++)
        {
            Combo comboToBeAdded = new Combo();
            comboToBeAdded.ingredient1 = shopComboIngredient1[i];
            comboToBeAdded.ingredient2 = shopComboIngredient2[i];
            GameData.shopCombos.Add(comboToBeAdded);
        }

    }
}
