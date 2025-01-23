using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySceneButtonHandle : MonoBehaviour
{
    public BuyComboScript theComboScript;
    public Button button1;
    public Button button2;

    public string button1Ingredient1;
    public string button1Ingredient2;
    public string button2Ingredient1;
    public string button2Ingredient2;

    void Start()
    {
        button1.onClick.AddListener(ButtonClickButton1);
        button2.onClick.AddListener(ButtonClickButton2);
    }

    void ButtonClickButton1()
    {
        theComboScript.BuyCombo(button1Ingredient1, button1Ingredient2);
    }
    void ButtonClickButton2()
    {
        theComboScript.BuyCombo(button2Ingredient1, button2Ingredient2);
    }
}
