using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;
public class CardGame : MonoBehaviour
{
    [SerializeField] int mana = 10;
    [SerializeField] int manaMaxValue = 10;
    [SerializeField] int manaRecovery = 1;
    List<GameObject> cardSelectList = new List<GameObject>();
    [SerializeField] List<GameObject> theCardSetterList = new List<GameObject>();
    List<GameObject> theCardList = new List<GameObject>(); //list of cards the deck can draw from

    [SerializeField] TMP_Text playerHealthDisplay;
    [SerializeField] TMP_Text enemyHealthDisplay;
    [SerializeField] TMP_Text manaDisplay;
    [SerializeField] TMP_Text cardAmountDisplay;


    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject theEnemy;

    [SerializeField] List<GameObject> theDeck = new List<GameObject>();

    bool playerTurn = true;
    bool gameOver;

    public LayerMask ignoreLayer;  // Reference to the layer that ray ignore
    void Start()
    {
        manaDisplay.text = "Mana: " + mana;
        StartCoroutine(RandomizeDeckTimer());
        playerHealthDisplay.text = "Player: " + thePlayer.GetComponent<CharacterObject>().theHealth.ToString();
        enemyHealthDisplay.text = "Enemy: " + theEnemy.GetComponent<CharacterObject>().theHealth.ToString();

        //generating the list
        foreach (GameObject theCardSetter in theCardSetterList)
        {
            CardObject theCardObject = theCardSetter.GetComponent<CardObject>();

            for (int i = 1; i <= theCardObject.amountGenerate; i++)
            {
                theCardList.Add(theCardSetter);
            }
        }
    }
    private System.Collections.IEnumerator RandomizeDeckTimer()
    {
        yield return new WaitForSeconds(0.001f);

        if (theCardList.Count < theDeck.Count)
        {
            Debug.Log("No card left!");
            gameOver = true;
            yield break;
        }


        foreach (GameObject theCard in theDeck)
        {
            int randomIndex = UnityEngine.Random.Range(0, theCardList.Count);
            GameObject randomCard = theCardList[randomIndex];
            theCardList.RemoveAt(randomIndex);
            theCard.GetComponent<CardObject>().ChangeCard(randomCard.GetComponent<CardObject>());
        }

        cardAmountDisplay.text = "Card amount: " + theCardList.Count.ToString();

    }

    void Update()
    {

        if (gameOver)
        {
            return;
        }

        if (playerTurn)
        {
            if (Input.GetMouseButtonDown(0)) //left mouse button click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.5f);

                //see if player clicks on a card
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~ignoreLayer))
                {
                    //card object click
                    if (hit.collider.CompareTag("Card"))
                    {
                        HandleCardClick(hit.collider.gameObject);
                    }

                    //confirm button click
                    if (hit.collider.gameObject.name == "ConfirmButton")
                    {
                        //if both cards are selected, start dealing damage and update the values
                        if (cardSelectList.Count >= 2)
                        {
                            int valueSum = 0;
                            int multiplierSum = 0;

                            for (int i = 0; i < cardSelectList.Count; i++)
                            {
                                if (cardSelectList[i].GetComponent<CardObject>().isMultiplier)
                                {
                                    multiplierSum += cardSelectList[i].GetComponent<CardObject>().multiplierNumber;
                                }

                                else
                                {
                                    valueSum += cardSelectList[i].GetComponent<CardObject>().theValue;
                                }
                            }

                            if (multiplierSum == 0)
                            {
                                multiplierSum = 1;
                            }

                            int theNumber = valueSum * multiplierSum;

                            theEnemy.GetComponent<CharacterObject>().theHealth -= theNumber;
                            enemyHealthDisplay.text = "Enemy: " + theEnemy.GetComponent<CharacterObject>().theHealth.ToString();
                            Debug.Log("Player deals: " + theNumber);
                            

                            //game over feature - if enemy got defeated
                            if (theEnemy.GetComponent<CharacterObject>().theHealth <= 0)
                            {
                                gameOver = true;
                                Debug.Log("Player won");
                            }
                        }

                        if (cardSelectList.Count == 1)
                        {
                            cardSelectList[0].gameObject.GetComponent<CardObject>().isMarked = false;
                            cardSelectList[0].transform.Find("cardMark").gameObject.SetActive(false);

                        }
                            playerTurn = false;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1)) //right click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.5f);

                //see if player clicks on a card
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Card"))
                    {
                        foreach (GameObject theCard in cardSelectList)
                        {
                            if (hit.collider.gameObject == theCard && theCard.GetComponent<CardObject>().isMarked == true)
                            {
                                mana += theCard.gameObject.GetComponent<CardObject>().manaCost;
                                manaDisplay.text = "Mana: " + mana;
                                theCard.GetComponent<CardObject>().isMarked = false;
                                theCard.transform.Find("cardMark").gameObject.SetActive(false);
                                cardSelectList.Remove(theCard);
                                break;

                            }
                        }
                    }
                }
            }
        }

        //enemy's turn
        else
        {
            int theNumber;
            theNumber = UnityEngine.Random.Range(2, 21);
            thePlayer.GetComponent<CharacterObject>().theHealth -= theNumber;
            playerHealthDisplay.text = "Player: " + thePlayer.GetComponent<CharacterObject>().theHealth.ToString();
            playerTurn = true;

            Debug.Log("Enemy deals: " +  theNumber);

            if (thePlayer.GetComponent<CharacterObject>().theHealth <= 0)
            {
                gameOver = true;
                Debug.Log("Enemy won");
                return;
            }

            if (theCardList.Count < cardSelectList.Count)
            {
                Debug.Log("No card left!");
                gameOver = true;
                return;
            }

            else
            {
                RandomizeDeck();
            }

            foreach (GameObject theCard in cardSelectList)
            {
                theCard.gameObject.GetComponent<CardObject>().isMarked = false;
                theCard.transform.Find("cardMark").gameObject.SetActive(false);
            }
            cardSelectList.Clear();

            mana += manaRecovery;

            if (mana > manaMaxValue)
            {
                mana = manaMaxValue;
            }


            manaDisplay.text = "Mana: " + mana;

        }
    }

    void HandleCardClick(GameObject cardObject)
    {
        if (gameOver)
        {
            return;
        }

        bool cardRepeat = false;
        foreach (GameObject theCard in cardSelectList)
        {
            if (theCard == cardObject)
            {
                cardRepeat = true;
                break;
            }
        }

        if (cardRepeat == false && mana >= cardObject.gameObject.GetComponent<CardObject>().manaCost)
        {
            cardObject.transform.Find("cardMark").gameObject.SetActive(true);
            cardObject.gameObject.GetComponent<CardObject>().isMarked = true;
            cardSelectList.Add(cardObject);
            mana -= cardObject.gameObject.GetComponent<CardObject>().manaCost;
            manaDisplay.text = "Mana: " + mana;
        }
    }

    void RandomizeDeck()
    {
        foreach (GameObject theCard in theDeck)
        {
            if (theCard.GetComponent<CardObject>().isMarked == true)
            {
                int randomIndex = UnityEngine.Random.Range(0, theCardList.Count);
                GameObject randomCard = theCardList[randomIndex];
                theCardList.RemoveAt(randomIndex);
                theCard.GetComponent<CardObject>().ChangeCard(randomCard.GetComponent<CardObject>());
            }
        }

        cardAmountDisplay.text = "Card amount: " + theCardList.Count.ToString();
    }
}
