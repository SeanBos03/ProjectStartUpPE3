using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardObject : MonoBehaviour
{
    TMP_Text theText;
    [HideInInspector] public int theNumber = 0;
    void Start()
    {
        theText = this.transform.Find("Canvas").Find("theText").GetComponent<TMP_Text>();
    }

    public void RandomText()
    {
        if (theText != null)
        {
            theNumber = Random.Range(1, 11);
            theText.text = theNumber.ToString();
        }
    }
}
