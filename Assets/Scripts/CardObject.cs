using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardObject : MonoBehaviour
{
    public int theValue;
    public string theType;
    TMP_Text theText; //used for ability card only
    public bool isMultiplier;
    public int multiplierNumber; //an ability card applies mulitplier to element card increasing the value
    void Start()
    {
        theText = this.transform.Find("Canvas").Find("theText").gameObject.GetComponent<TMP_Text>();
    }

    public void ChangeCard(CardObject other)
    {
        if (theText.text == null)
        {
            Debug.Log(" itself null");
        }

        if (other.theText.text == null)
        {
            Debug.Log(" other null");
        }

        theText.text = other.theText.text;
        isMultiplier = other.isMultiplier;
        multiplierNumber = other.multiplierNumber;
        theValue = other.theValue;
        theType = other.theType;
        this.transform.Find("ElementDisplay").gameObject.GetComponent<Renderer>().material =
        other.transform.Find("ElementDisplay").gameObject.GetComponent<Renderer>().material;

        if (isMultiplier)
        {
            this.transform.Find("Canvas").gameObject.SetActive(true);
            this.transform.Find("ElementDisplay").gameObject.SetActive(false);
        }

        else
        {
            this.transform.Find("Canvas").gameObject.SetActive(false);
            this.transform.Find("ElementDisplay").gameObject.SetActive(true);
        }
    }
}
