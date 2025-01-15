using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
public class CardGame : MonoBehaviour
{
    [SerializeField] int elementAmount = 10; //10
    [SerializeField] int abilityAmount = 5;  //5

    [SerializeField] List<GameObject> theCardSetterList = new List<GameObject>();
    List<GameObject> theCardList = new List<GameObject>(); //list of cards the deck can draw from

    GameObject card1 = null; //first card player is selecting
    GameObject card2 = null; //second card play is sleecting
    int theNumber;

    [SerializeField] TMP_Text playerHealthDisplay;
    [SerializeField] TMP_Text enemyHealthDisplay;
    [SerializeField] TMP_Text cardAmountDisplay;

    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject theEnemy;

    [SerializeField] List<GameObject> theDeck = new List<GameObject>();

    bool playerTurn = true;
    bool gameOver;

    public LayerMask ignoreLayer;  // Reference to the layer that ray ignore
    void Start()
    {
        StartCoroutine(RandomizeDeckTimer());
        playerHealthDisplay.text = "Player: " + thePlayer.GetComponent<CharacterObject>().theHealth.ToString();
        enemyHealthDisplay.text = "Enemy: " + theEnemy.GetComponent<CharacterObject>().theHealth.ToString();
        //generating the list
        foreach (GameObject theCardSetter in theCardSetterList)
        {
            CardObject theCardObject = theCardSetter.GetComponent<CardObject>();
            if (!theCardObject.isMultiplier)
            {
                for (int i = 1; i <= elementAmount; i++)
                {
                    theCardList.Add(theCardSetter);
                }
            }

            else
            {
                for (int i = 1; i <= abilityAmount; i++)
                {
                    theCardList.Add(theCardSetter);
                }
            }
        }
    }
    private System.Collections.IEnumerator RandomizeDeckTimer()
    {
        yield return new WaitForSeconds(0.001f);
        RandomizeDeck();
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
                        if (card1 != null && card2 != null)
                        {
                            CardObject card1Card = card1.GetComponent<CardObject>();
                            CardObject card2Card = card2.GetComponent<CardObject>();

                            if (!card1Card.isMultiplier && !card2Card.isMultiplier)
                            {
                                theNumber = card1.GetComponent<CardObject>().theValue +
                                card2.GetComponent<CardObject>().theValue;
                            }

                            else if (card1Card.isMultiplier && !card2Card.isMultiplier)
                            {
                                theNumber = card2.GetComponent<CardObject>().theValue * 
                                    card1.GetComponent<CardObject>().multiplierNumber;
                            }

                            else if (!card1Card.isMultiplier && card2Card.isMultiplier)
                            {
                                theNumber = card1.GetComponent<CardObject>().theValue *
                                    card2.GetComponent<CardObject>().multiplierNumber;
                            }

                            else
                            {
                                return;
                            }


                            theEnemy.GetComponent<CharacterObject>().theHealth -= theNumber;
                            enemyHealthDisplay.text = "Enemy: " + theEnemy.GetComponent<CharacterObject>().theHealth.ToString();
                            Debug.Log("Player deals: " + theNumber);
                            playerTurn = false;

                            //game over feature - if enemy got defeated
                            if (theEnemy.GetComponent<CharacterObject>().theHealth <= 0)
                            {
                                gameOver = true;
                                Debug.Log("Player won");
                            }
                        }


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
                        if (hit.collider.gameObject == card1)
                        {
                            card1.transform.Find("cardMark").gameObject.SetActive(false);
                            card1 = null;
                        }

                        if (hit.collider.gameObject == card2)
                        {
                            card2.transform.Find("cardMark").gameObject.SetActive(false);
                            card2 = null;
                        }

                    }
                }
            }
        }

        //enemy's turn
        else
        {
            card1.transform.Find("cardMark").gameObject.SetActive(false);
            card2.transform.Find("cardMark").gameObject.SetActive(false);
            theNumber = UnityEngine.Random.Range(2, 21);
            thePlayer.GetComponent<CharacterObject>().theHealth -= theNumber;
            playerHealthDisplay.text = "Player: " + thePlayer.GetComponent<CharacterObject>().theHealth.ToString();
            playerTurn = true;
            card1 = null;
            card2 = null;

            
            Debug.Log("Enemy deals: " +  theNumber);

            if (thePlayer.GetComponent<CharacterObject>().theHealth <= 0)
            {
                gameOver = true;
                Debug.Log("Enemy won");
                return;
            }

            if (theCardList.Count < theDeck.Count)
            {
                Debug.Log("No card left!");
                gameOver = true;
                return;
            }

            else
            {
                RandomizeDeck();
            }

            
        }
    }

    void HandleCardClick(GameObject cardObject)
    {
        if (gameOver)
        {
            return;
        }

        if (card1 == null)
        {
            card1 = cardObject;
            card1.transform.Find("cardMark").gameObject.SetActive(true);
            return;
        }

        if (card2 == null && cardObject != card1)
        {
            card2 = cardObject;
            card2.transform.Find("cardMark").gameObject.SetActive(true);
            return;
        }
    }

    void RandomizeDeck()
    {
        foreach (GameObject theCard in theDeck)
        {
            int randomIndex = UnityEngine.Random.Range(0, theCardList.Count);
            GameObject randomCard = theCardList[randomIndex];
            theCardList.RemoveAt(randomIndex);
            theCard.GetComponent<CardObject>().ChangeCard(randomCard.GetComponent<CardObject>());
        }

        cardAmountDisplay.text = "Card amount: " + theCardList.Count.ToString();
    }
}
