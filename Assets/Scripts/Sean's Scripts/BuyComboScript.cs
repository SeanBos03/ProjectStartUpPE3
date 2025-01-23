using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyComboScript : MonoBehaviour
{
    public void BuyCombo(string ingredient1, string ingredient2)
    {
        GameData.Combo combo = new GameData.Combo();
        combo.ingredient1 = ingredient1;
        combo.ingredient2 = ingredient2;

        bool repeatCombo = false;

        foreach (GameData.Combo theCombo in GameData.learnedCombos)
        {
            if (theCombo.CheckComboMatched(combo))
            {
                repeatCombo = true;
                break;
            }
        }

        if (!repeatCombo)
        {
            GameData.learnedCombos.Add(combo);
            Debug.Log("Combo learned");
        }
        
    }

    public void SwitchScene(string theScene)
    {
        GameData.SwitchScene(theScene);
    }
}
