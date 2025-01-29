using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardObjectImage : MonoBehaviour
{
    public int manaDiscardValue = 2;
    public int manaCost = 2;
    [HideInInspector] public bool isMarked;
    [HideInInspector] public bool isToBeDeleted;
    [HideInInspector] public bool isToBeRefreshed;
    [HideInInspector] public bool isDeleted;
    public int amountGenerate;
    public int theValue;
    public string theType;
    public int multiplierNumber; //an ability card applies mulitplier to element card increasing the value
    public bool doesHealing;
    public bool isMoving;

    List<GameObject> cardSelectList = new List<GameObject>();
    void Start()
    {
    }

    public void ChangeCard(CardObjectImage other)
    {
        theType = other.theType;
        manaCost = other.manaCost;
        manaDiscardValue = other.manaDiscardValue;
        isToBeRefreshed = other.isToBeRefreshed;
        isDeleted = other.isDeleted;
        doesHealing = other.doesHealing;

        Image theImage = transform.Find("CardBase").GetComponent<Image>();
        theImage.sprite = other.transform.Find("CardBase").GetComponent<Image>().sprite;

        if (theType.Contains("Multiplier"))
        {
            multiplierNumber = other.multiplierNumber;
        }

        if (theType.Contains("Element"))
        {
            theValue = other.theValue;
        }
    }
}
