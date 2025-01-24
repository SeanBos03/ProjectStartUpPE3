using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MixtureUnlocked : MonoBehaviour
{
    public GameData.Mixture theMixture;

    void Start()
    {
        GameObject theTextObject = GameObject.Find("Mixture/theText");
        TextMeshProUGUI textt = theTextObject.GetComponent<TextMeshProUGUI>();
        textt.text = "= " + theMixture.value;

        bool mixtureMatched = false;
        foreach (GameData.Mixture mixture in GameData.mixturesKnown)
        {
            if (mixture.CheckMixtureMatched(theMixture))
            {
                mixtureMatched = true;
                return;
            }
        }

        if (!mixtureMatched)
        {
            Transform objectMixture = transform.Find("Mixture");
            objectMixture.gameObject.SetActive(false);
            Transform objectText = transform.Find("NotUnlockedText");
            objectText.gameObject.SetActive(true);
        }
    }
}
