using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySceneButtonHandle : MonoBehaviour
{
    public BuyMixtureScript theMixtureScript;
    public Button button1;
    public Button button2;
    public GameData.Mixture button1Mixture;
    public GameData.Mixture button2Mixture;

    void Start()
    {
        button1.onClick.AddListener(ButtonClickButton1);
        button2.onClick.AddListener(ButtonClickButton2);
    }

    void ButtonClickButton1()
    {
        theMixtureScript.BuyMixture(button1Mixture);
    }
    void ButtonClickButton2()
    {
        theMixtureScript.BuyMixture(button2Mixture);
    }
}
