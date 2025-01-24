    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Security.Cryptography;
public class CardGame : MonoBehaviour
{
    [SerializeField] int comboBonusValue = 2;
    [SerializeField] int mana = 10;
    [SerializeField] int manaMaxValue = 10;
    [SerializeField] int manaRecovery = 1;
    List<GameObject> cardSelectList = new List<GameObject>(); //list of marked/selected cards
    List<GameObject> cardDelteList = new List<GameObject>(); //list of to be deleted cards
    
    [SerializeField] List<GameObject> theCardSetterList = new List<GameObject>();
    List<GameObject> theCardList = new List<GameObject>(); //list of cards the deck can draw from

    [SerializeField] TMP_Text playerHealthDisplay;
    [SerializeField] TMP_Text enemyHealthDisplay;
    [SerializeField] TMP_Text manaDisplay;
    [SerializeField] TMP_Text resultManaDisplay;
    [SerializeField] TMP_Text cardAmountDisplay;

    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject theEnemy;

    [SerializeField] List<GameObject> theDeck = new List<GameObject>();

    [SerializeField] Slider enemyHealthSlider;
    [SerializeField] Slider playerHealthSlider;
    [SerializeField] Slider playerManaSlider;

    bool playerTurn = true;
    bool gameOver;
    int resultMana;

    bool isMaking;

    public LayerMask ignoreLayer;  // Reference to the layer that ray ignore
    void Start()
    {
        enemyHealthSlider.maxValue = theEnemy.GetComponent<CharacterObject>().maxHealth;
        playerHealthSlider.maxValue = thePlayer.GetComponent<CharacterObject>().maxHealth;
        playerManaSlider.maxValue = manaMaxValue;
        resultMana = mana;
        UpdateManaPlayer();
        resultManaDisplay.text = "Result mana: " + resultMana;
        StartCoroutine(RandomizeDeckTimer());
        UpdateHealthPlayer();
        
        UpdateHealthEnemy();

        //generating the list
        foreach (GameObject theCardSetter in theCardSetterList)
        {
            CardObject theCardObject = theCardSetter.GetComponent<CardObject>();

            for (int i = 1; i <= theCardObject.amountGenerate; i++)
            {
                theCardList.Add(theCardSetter);
            }

            //generate list of unique elements
            if (theCardObject.theType.Contains("Element_"))
            {
                bool elementRepeat = false;
                foreach (String element in GameData.elementList)
                {
                    if (theCardObject.theType == element)
                    {
                        elementRepeat = true;
                    }
                }

                if (!elementRepeat)
                {
                    GameData.elementList.Add(theCardObject.theType);
                }
            }
        }
    }

    //at start, place cards at the deck
    private System.Collections.IEnumerator RandomizeDeckTimer()
    {
        yield return new WaitForSeconds(0.001f);

        if (theCardList.Count < theDeck.Count)
        {
            Debug.Log("No card left!");
            SceneManager.LoadScene("MainMenu");
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
                    if (hit.collider.gameObject.name == "image_book")
                    {
                        Debug.Log("Hit book");
                    }

                    //card object click
                    if (hit.collider.CompareTag("Card"))
                    {
                        //click marked card to unselected
                        if (hit.collider.gameObject.GetComponent<CardObject>().isMarked == true)
                        {
                            foreach (GameObject theCard in cardSelectList)
                            {
                                if (hit.collider.gameObject == theCard)
                                {
                                    resultMana += theCard.gameObject.GetComponent<CardObject>().manaCost;
                                    CheckAndDisplayResultMana();
                                    theCard.GetComponent<CardObject>().isMarked = false;
                                    theCard.transform.Find("cardMark").gameObject.SetActive(false);
                                    cardSelectList.Remove(theCard);
                                    break;
                                }
                            }
                        }

                        //click toBeDeleredCard card to unselect
                        else if (hit.collider.gameObject.GetComponent<CardObject>().isToBeDeleted == true)
                        {
                            foreach (GameObject theCard in cardDelteList)
                            {
                                if (hit.collider.gameObject == theCard)
                                {
                                    resultMana -= theCard.gameObject.GetComponent<CardObject>().manaDiscardValue;
                                    CheckAndDisplayResultMana();
                                    theCard.GetComponent<CardObject>().isToBeDeleted = false;
                                    theCard.transform.Find("cardMarkDelete").gameObject.SetActive(false);
                                    break;
                                }
                            }
                        }

                        //select unmarked card
                        else
                        {
                            HandleCardClick(hit.collider.gameObject);
                        }
                        
                    }

                    //end turn button click
                    if (hit.collider.gameObject.name == "EndTurnButton")
                    {
                        playerTurn = false;
                    }

                    //confirm button click
                    if (hit.collider.gameObject.name == "ConfirmButton")
                    {
                        int amountElement = 0;

                        List<String> elementListCompare = new List<String>();
                        List<GameObject> uniqueElementCardList = new List<GameObject>();

                        //copy the values of elementList to elementListCompare. So modifying elementListCompare wont modify elementList
                        foreach (String element in GameData.elementList)
                        {
                            elementListCompare.Add((string)element.Clone());
                        }

                        foreach (GameObject theCard in cardSelectList)
                        {
                            foreach (String element in elementListCompare)
                            {
                                if (theCard.GetComponent<CardObject>().theType == element)
                                {
                                    amountElement++;
                                    elementListCompare.Remove(element);
                                    uniqueElementCardList.Add(theCard);
                                    break;
                                }
                            }

                            if (amountElement > 2)
                            {
                                return;
                            }
                        }

                        if (amountElement == 0)
                        {
                            if (cardDelteList.Count == 0)
                            {
                                return;
                            }
                            
                        }

                        if (resultMana < 0)
                        {
                            return;
                        }

                        //if only 1 card is marked
                        if (cardSelectList.Count == 1)
                        {
                            UnselectCard(cardSelectList[0]);
                            cardSelectList.Clear();
                        }

                        //if both cards are selected, start dealing damage and update the values
                        else if (cardSelectList.Count >= 2)
                        {
                            int valueSum = 0;
                            int multiplierSum = 0;
                            int amountOfElements= 0;

                            for (int i = 0; i < cardSelectList.Count; i++)
                            {
                                if (cardSelectList[i].GetComponent<CardObject>().theType.Contains("Multiplier"))
                                {
                                    multiplierSum += cardSelectList[i].GetComponent<CardObject>().multiplierNumber;
                                }

                                else
                                {
                                    valueSum += cardSelectList[i].GetComponent<CardObject>().theValue;
                                    amountOfElements++;
                                }
                            }

                            if (multiplierSum == 0)
                            {
                                multiplierSum = 1;
                            }

                            int theNumber = 0;
                            theNumber = valueSum * multiplierSum;

                            if (uniqueElementCardList.Count > 1)
                            {
                                GameData.Combo combo = new GameData.Combo();
                                combo.ingredient1 = uniqueElementCardList[0].GetComponent<CardObject>().theType;
                                combo.ingredient2 = uniqueElementCardList[1].GetComponent<CardObject>().theType;
                                bool foundCombo = false;
                                foreach (GameData.Combo learnedCombo in GameData.combosKnown)
                                {
                                    if (combo.CheckComboMatched(learnedCombo))
                                    {
                                        foundCombo = true;
                                        break;
                                    }
                                }

                                if (foundCombo)
                                {
                                    theNumber += (amountOfElements * comboBonusValue);
                                }
                            }

                            foreach (GameData.Mixture theMixure in GameData.mixturesKnown)
                            {
                                GameData.Mixture checkMixture = new GameData.Mixture();
                                checkMixture.theElements = new List<string>();
                                foreach (GameObject theCard in cardSelectList)
                                {
                                    checkMixture.theElements.Add(theCard.GetComponent<CardObject>().theType);
                                }

                                //mixture matched
                                if (checkMixture.CheckMixtureMatched(theMixure))
                                {
                                    theNumber += theMixure.value;
                                    break;
                                }
                            }

                            theEnemy.GetComponent<CharacterObject>().theHealth -= theNumber;
                            
                            UpdateHealthEnemy();
                            Debug.Log("Player deals: " + theNumber);

                            //game over feature - if enemy got defeated
                            if (theEnemy.GetComponent<CharacterObject>().theHealth <= 0)
                            {
                                gameOver = true;
                                SceneManager.LoadScene("Chapter 1 Map");
                                Debug.Log("Player won");
                            }

                            foreach (GameObject theCard in cardSelectList)
                            {
                                theCard.GetComponent<CardObject>().isToBeRefreshed = true;
                                RemoveCard(theCard);
                            }
                            cardSelectList.Clear();
                        }

                        //if (resultMana > manaMaxValue)
                        //{
                        //    resultMana = manaMaxValue;
                        //}

                        foreach (GameObject theCard in cardDelteList)
                        {
                            theCard.GetComponent<CardObject>().isToBeRefreshed = true;
                            theCard.transform.Find("cardMarkDelete").gameObject.SetActive(false);
                            RemoveCard(theCard);
                        }
                        cardDelteList.Clear();
                        CheckAndDisplayResultMana();
                        mana = resultMana;
                        CheckAndDisplayMana(false);
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
                    HandleCardDelete(hit.collider.gameObject);
                }
            }
        }

        //enemy's turn
        else
        {
            int amountCardMissing = 0;
            foreach (GameObject theCard in theDeck)
            {
                if (theCard.GetComponent<CardObject>().isMarked ||
                    theCard.GetComponent<CardObject>().isToBeDeleted)
                {
                    UnselectCard(theCard);
                }

                if (theCard.gameObject.GetComponent<CardObject>().isDeleted)
                {
                    theCard.gameObject.GetComponent<SpellManager>().StartMovingBack();
                    amountCardMissing++;
                }
            }


            int theNumber;
            theNumber = UnityEngine.Random.Range(2, 21);
            thePlayer.GetComponent<CharacterObject>().theHealth -= theNumber;
            UpdateHealthPlayer();
            playerTurn = true;

            Debug.Log("Enemy deals: " +  theNumber);

            if (thePlayer.GetComponent<CharacterObject>().theHealth <= 0)
            {
                gameOver = true;
                Debug.Log("Enemy won");
                SceneManager.LoadScene("MainMenu");
                return;
            }

            if (theCardList.Count < amountCardMissing)
            {
                Debug.Log("No card left!");
                SceneManager.LoadScene("MainMenu");
                gameOver = true;
                return;
            }

            else
            {
                RandomizeDeck();
            }

            mana += manaRecovery;
            CheckAndDisplayMana(true);
            resultMana = mana;
            CheckAndDisplayResultMana();

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

        int overAllMana = resultMana;
        if (cardObject.gameObject.GetComponent<CardObject>().isToBeDeleted)
        {
            overAllMana -= cardObject.gameObject.GetComponent<CardObject>().manaCost;
        }

        if (cardRepeat == false && overAllMana >= cardObject.gameObject.GetComponent<CardObject>().manaCost)
        {
            UpdateCardStatus(cardObject, true, true, true);
            cardObject.transform.Find("cardMark").gameObject.SetActive(true);
            cardObject.gameObject.GetComponent<CardObject>().isMarked = true;
            cardSelectList.Add(cardObject);
            resultMana -= cardObject.gameObject.GetComponent<CardObject>().manaCost;
            CheckAndDisplayResultMana();
        }
    }

    void HandleCardDelete(GameObject cardObject)
    {
        if (gameOver)
        {
            return;
        }

        bool cardRepeat = false;
        foreach (GameObject theCard in cardDelteList)
        {
            if (theCard == cardObject)
            {
                cardRepeat = true;
                break;
            }
        }

        if (cardRepeat == false)
        {
            UpdateCardStatus(cardObject, false, true, true);
            cardObject.transform.Find("cardMarkDelete").gameObject.SetActive(true);
            cardObject.gameObject.GetComponent<CardObject>().isToBeDeleted = true;
            cardDelteList.Add(cardObject);
            resultMana += cardObject.gameObject.GetComponent<CardObject>().manaDiscardValue;
            CheckAndDisplayResultMana();
        }
    }

    void RandomizeDeck()
    {
        foreach (GameObject theCard in theDeck)
        {
            if (theCard.GetComponent<CardObject>().isToBeRefreshed)
            {
                theCard.GetComponent<CardObject>().isToBeRefreshed = true;
               // theCard.SetActive(true);
                int randomIndex = UnityEngine.Random.Range(0, theCardList.Count);
                GameObject randomCard = theCardList[randomIndex];
                theCardList.RemoveAt(randomIndex);
                theCard.GetComponent<CardObject>().ChangeCard(randomCard.GetComponent<CardObject>());
            }
        }

        cardAmountDisplay.text = "Card amount: " + theCardList.Count.ToString();
    }

    void UnselectCard(GameObject theCard)
    {
        if (theCard.GetComponent<CardObject>().isMarked)
        {
            UpdateCardStatus(theCard, false, true, true);
            cardSelectList.Remove(theCard);
        }

        if (theCard.GetComponent<CardObject>().isToBeDeleted)
        {
            UpdateCardStatus(theCard, true, true, true);
            cardDelteList.Remove(theCard);
        }        
    }

    void RemoveCard(GameObject theCard)
    {
        //trying to find what card of the card is storing theCard, then, if found, disable that deck object to get the 'removed' effect
        int index = theDeck.FindIndex(a => a == theCard);
        if (index != -1)
        {
            UpdateCardStatus(theCard, true, false, false);
            UpdateCardStatus(theCard, false, false, false);
         //   theDeck[index].gameObject.SetActive(false);
            theCard.gameObject.GetComponent<CardObject>().isDeleted = true;
            theCard.gameObject.GetComponent<SpellManager>().StartMoving();
        }        
    }

    void UpdateCardStatus(GameObject theCard, bool istoBeMarked, bool extractMana, bool removeFromList, bool useCardDiscardCost=false)
    {
        if (istoBeMarked)
        {
            if (theCard.gameObject.GetComponent<CardObject>().isToBeDeleted)
            {
                if (extractMana)
                {
                    if (useCardDiscardCost)
                    {
                        resultMana -= theCard.gameObject.GetComponent<CardObject>().manaDiscardValue;
                    }

                    else
                    {
                        resultMana -= theCard.gameObject.GetComponent<CardObject>().manaCost;
                    }
                    
                    CheckAndDisplayResultMana();
                }

                theCard.transform.Find("cardMarkDelete").gameObject.SetActive(false);
                theCard.gameObject.GetComponent<CardObject>().isToBeDeleted = false;

                if (removeFromList)
                {
                    cardDelteList.Remove(theCard);
                }
                
            }
        }

        else
        {
            if (theCard.gameObject.GetComponent<CardObject>().isMarked)
            {
                if (extractMana)
                {
                    if (useCardDiscardCost)
                    {
                        resultMana += theCard.gameObject.GetComponent<CardObject>().manaDiscardValue;
                    }

                    else
                    {
                        resultMana += theCard.gameObject.GetComponent<CardObject>().manaCost;
                    }
                    
                    CheckAndDisplayResultMana();
                }

                theCard.transform.transform.Find("cardMark").gameObject.SetActive(false);
                theCard.gameObject.GetComponent<CardObject>().isMarked = false;
                if (removeFromList)
                {
                    cardSelectList.Remove(theCard);
                }
                
            }
        }
    }

    void CheckAndDisplayMana(bool checkMax)
    {
        if (checkMax)
        {
            if (mana > manaMaxValue)
            {
                mana = manaMaxValue;
            }
        }

        if (mana < 0)
        {
            mana = 0;
        }

        UpdateManaPlayer();
    }
    void CheckAndDisplayResultMana()
    {
        resultManaDisplay.text = "Result mana: " + resultMana;
    }

    void UpdateHealthEnemy()
    {
        enemyHealthDisplay.text = theEnemy.GetComponent<CharacterObject>().theHealth.ToString() + "/" +
            theEnemy.GetComponent<CharacterObject>().maxHealth.ToString();
        enemyHealthSlider.value = theEnemy.GetComponent<CharacterObject>().theHealth;
    }

    void UpdateHealthPlayer()
    {
        playerHealthDisplay.text = thePlayer.GetComponent<CharacterObject>().theHealth.ToString() + "/" +
            thePlayer.GetComponent<CharacterObject>().maxHealth.ToString();
        playerHealthSlider.value = thePlayer.GetComponent<CharacterObject>().theHealth;
    }

    void UpdateManaPlayer()
    {
        manaDisplay.text = mana.ToString() + "/" +
            manaMaxValue.ToString();
        playerManaSlider.value = mana;
    }
}

