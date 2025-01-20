using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardObject : MonoBehaviour
{
    public int manaCost = 2;
    [HideInInspector] public bool isMarked; //if true, at enemy's turn, card will possibly be refreshed
    public int amountGenerate;
    public int theValue;
    public string theType;
    public int multiplierNumber; //an ability card applies mulitplier to element card increasing the value
    void Start()
    {
    }

    public void ChangeCard(CardObject other)
    {
        theType = other.theType;
        manaCost = other.manaCost;
        if (theType.Contains("Multiplier"))
        {
            this.transform.Find("Canvas").gameObject.SetActive(true);
            this.transform.Find("ElementDisplay").gameObject.SetActive(false);
            TMP_Text theText = this.transform.Find("Canvas").Find("theText").gameObject.GetComponent<TMP_Text>();
            theText.text = other.transform.Find("Canvas").Find("theText").gameObject.GetComponent<TMP_Text>().text;
            multiplierNumber = other.multiplierNumber;
        }

        if (theType.Contains("Element"))
        {
            this.transform.Find("Canvas").gameObject.SetActive(false);
            this.transform.Find("ElementDisplay").gameObject.SetActive(true);
            this.transform.Find("ElementDisplay").gameObject.GetComponent<Renderer>().material =
            other.transform.Find("ElementDisplay").gameObject.GetComponent<Renderer>().material;
            theValue = other.theValue;
        }
    }
}
