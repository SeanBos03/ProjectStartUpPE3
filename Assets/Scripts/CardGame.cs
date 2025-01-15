using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CardGame : MonoBehaviour
{
    GameObject card1 = null; //first card player is selecting
    GameObject card2 = null; //second card play is sleecting
    int theNumber;

    [SerializeField] TMP_Text playerHealthDisplay;
    [SerializeField] TMP_Text enemyHealthDisplay;

    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject theEnemy;

    [SerializeField] List<GameObject> theDeck = new List<GameObject>();

    bool playerTurn = true;
    bool gameOver;

    void Start()
    {
        StartCoroutine(RandomizeDeckTimer());
        playerHealthDisplay.text = "Player: " + thePlayer.GetComponent<CharacterObject>().theHealth.ToString();
        enemyHealthDisplay.text = "Enemy: " + theEnemy.GetComponent<CharacterObject>().theHealth.ToString(); 
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
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Card"))
                    {
                        HandleCardClick(hit.collider.gameObject);
                    }

                    if (hit.collider.gameObject.name == "ConfirmButton")
                    {

                        if (card1 != null && card2 != null)
                        {
                            theNumber = card1.GetComponent<CardObject>().theNumber +
                                card2.GetComponent<CardObject>().theNumber;
                            theEnemy.GetComponent<CharacterObject>().theHealth -= theNumber;
                            enemyHealthDisplay.text = "Enemy: " + theEnemy.GetComponent<CharacterObject>().theHealth.ToString();
                            Debug.Log("Player deals: " + theNumber);
                            playerTurn = false;

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

        else
        {
            card1.transform.Find("cardMark").gameObject.SetActive(false);
            card2.transform.Find("cardMark").gameObject.SetActive(false);
            theNumber = Random.Range(2, 21);
            thePlayer.GetComponent<CharacterObject>().theHealth -= theNumber;
            playerHealthDisplay.text = "Player: " + thePlayer.GetComponent<CharacterObject>().theHealth.ToString();
            playerTurn = true;
            card1 = null;
            card2 = null;
            RandomizeDeck();
            Debug.Log("Enemy deals: " +  theNumber);

            if (thePlayer.GetComponent<CharacterObject>().theHealth <= 0)
            {
                gameOver = true;
                Debug.Log("Enemy won");
            }
        }
    }

    void HandleCardClick(GameObject cardObject)
    {
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
            theCard.GetComponent<CardObject>().RandomText();
        }
    }
}
