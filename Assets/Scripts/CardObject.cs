using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardObject : MonoBehaviour
{
    TMP_Text theText;
    public int theNumber;
    void Start()
    {
        theText = this.transform.Find("Canvas").Find("theText").GetComponent<TMP_Text>();
        RandomText();
    }

    void RandomText()
    {
        if (theText != null)
        {
            theNumber = Random.Range(1, 11);
            theText.text = theNumber.ToString();
        }
    }
}
