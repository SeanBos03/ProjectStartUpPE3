using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MixtureUnlocked : MonoBehaviour
{
    public GameData.Mixture theMixture;
    public TMP_Text theText;

    void Start()
    {
        GameObject theTextObject = GameObject.Find("Mixture/theText");
      //  TMP_Text textt = theTextObject.GetComponent<TMP_Text>();
        
        bool mixtureMatched = false;

        foreach (GameData.Mixture mixture in GameData.mixturesKnown)
        {
            if (mixture.CheckMixtureMatched(theMixture))
            {
                theText.text = "= " + mixture.value;
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
