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
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static UnityEditor.Rendering.FilterWindow;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental.GraphView;

public class CardGameUI : MonoBehaviour
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
    [SerializeField] TMP_Text enemyDamageDisplay;

    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject theEnemy;

    public Button endTurnButton;
    public Button confirmButton;

    [SerializeField] List<GameObject> theDeck = new List<GameObject>();

    [SerializeField] Slider enemyHealthSlider;
    [SerializeField] Slider playerHealthSlider;
    [SerializeField] Slider playerManaSlider;

    bool playerTurn = true;
    bool gameOver;
    int resultMana;

    public float damageDisplayTimeStart;
    public float damageDisplayTime;

    Animator enemyAnimator;
    Coroutine enemyDisplayTimer;
    void Start()
    {
        enemyAnimator = theEnemy.GetComponent<Animator>();
        enemyAnimator.SetInteger("animState", 1);
        enemyDisplayTimer = StartCoroutine(DisplayEnemyDamage());
        enemyHealthSlider.maxValue = theEnemy.GetComponent<CharacterObject>().maxHealth;
        playerHealthSlider.maxValue = thePlayer.GetComponent<CharacterObject>().maxHealth;
        playerManaSlider.maxValue = manaMaxValue;
        resultMana = mana;
        UpdateManaPlayer();
        resultManaDisplay.text = "Result mana: " + resultMana;
        StartCoroutine(RandomizeDeckTimer());
        UpdateHealthPlayer();

        UpdateHealthEnemy();

        confirmButton.onClick.AddListener(ConfirmButtonOnClick);
        endTurnButton.onClick.AddListener(EndTurnButtonClick);

        //generating the list
        foreach (GameObject theCardSetter in theCardSetterList)
        {
            CardObjectImage theCardObject = theCardSetter.GetComponent<CardObjectImage>();

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
            theCard.GetComponent<CardObjectImage>().ChangeCard(randomCard.GetComponent<CardObjectImage>());
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
                foreach (GameObject theDeckCard in theDeck)
                {
                    RectTransform rectTransform = theDeckCard.GetComponent<RectTransform>();
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
                    {
                        if (rectTransform.rect.Contains(localPoint))
                        {
                            if (theDeckCard.CompareTag("Card"))
                            {
                                //click marked card to unselected
                                if (theDeckCard.GetComponent<CardObjectImage>().isMarked == true)
                                {
                                    foreach (GameObject theCard in cardSelectList)
                                    {
                                        if (theDeckCard.gameObject == theCard)
                                        {
                                            resultMana += theCard.gameObject.GetComponent<CardObjectImage>().manaCost;
                                            CheckAndDisplayResultMana();
                                            theCard.GetComponent<CardObjectImage>().isMarked = false;
                                            theCard.transform.Find("cardMark").gameObject.SetActive(false);
                                            cardSelectList.Remove(theCard);
                                            break;
                                        }
                                    }
                                }

                                //click toBeDeleredCard card to unselect
                                else if (theDeckCard.GetComponent<CardObjectImage>().isToBeDeleted == true)
                                {
                                    foreach (GameObject theCard in cardDelteList)
                                    {
                                        if (theDeckCard.gameObject == theCard)
                                        {
                                            cardSelectList.Remove(theCard);
                                            cardDelteList.Remove(theCard);
                                            resultMana -= theCard.gameObject.GetComponent<CardObjectImage>().manaDiscardValue;
                                            CheckAndDisplayResultMana();
                                            theCard.GetComponent<CardObjectImage>().isToBeDeleted = false;
                                            theCard.transform.Find("cardMarkDelete").gameObject.SetActive(false);
                                            break;
                                        }
                                    }
                                }

                                //select unmarked card
                                else
                                {
                                    HandleCardClick(theDeckCard);
                                }

                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1)) //right click
            {
                foreach (GameObject theDeckCard in theDeck)
                {
                    RectTransform rectTransform = theDeckCard.GetComponent<RectTransform>();
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
                    {
                        if (rectTransform.rect.Contains(localPoint))
                        {
                            if (theDeckCard.CompareTag("Card"))
                            {
                                HandleCardDelete(theDeckCard);
                            }
                        }
                    }
                }
            }
        }

        //enemy's turn
        else
        {
            enemyAnimator.SetInteger("animState", 3);
            int amountCardMissing = 0;
            foreach (GameObject theCard in theDeck)
            {
                if (theCard.GetComponent<CardObjectImage>().isMarked ||
                    theCard.GetComponent<CardObjectImage>().isToBeDeleted)
                {
                    UnselectCard(theCard);
                }

                if (theCard.gameObject.GetComponent<CardObjectImage>().isDeleted)
                {
                    theCard.gameObject.GetComponent<TweenStuff>().StartMovingBack();
                    amountCardMissing++;
                }
            }


            int theNumber;
            theNumber = UnityEngine.Random.Range(2, 21);
            thePlayer.GetComponent<CharacterObject>().theHealth -= theNumber;
            UpdateHealthPlayer();
            playerTurn = true;

            enemyDamageDisplay.text = theNumber.ToString();
            enemyDamageDisplay.gameObject.SetActive(true);
            if (enemyDisplayTimer == null)
            {

                enemyDisplayTimer = StartCoroutine(DisplayEnemyDamage());
            }

            else
            {
                StopCoroutine(enemyDisplayTimer);
                enemyDisplayTimer = StartCoroutine(DisplayEnemyDamage());
            }

            Debug.Log("Enemy deals: " + theNumber);

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
        if (cardObject.gameObject.GetComponent<CardObjectImage>().isToBeDeleted)
        {
            overAllMana -= cardObject.gameObject.GetComponent<CardObjectImage>().manaCost;
        }

        if (cardRepeat == false && overAllMana >= cardObject.gameObject.GetComponent<CardObjectImage>().manaCost)
        {
            UpdateCardStatus(cardObject, true, true, true);
            cardObject.transform.Find("cardMark").gameObject.SetActive(true);
            cardObject.gameObject.GetComponent<CardObjectImage>().isMarked = true;
            cardSelectList.Add(cardObject);
            resultMana -= cardObject.gameObject.GetComponent<CardObjectImage>().manaCost;
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
                cardDelteList.Remove(theCard);
                if (theCard.gameObject.GetComponent<CardObjectImage>().isToBeDeleted)
                {
                    resultMana -= theCard.gameObject.GetComponent<CardObjectImage>().manaDiscardValue;
                }
                
                CheckAndDisplayResultMana();
                break;
            }
        }

        UpdateCardStatus(cardObject, false, true, true);
        cardObject.transform.Find("cardMarkDelete").gameObject.SetActive(true);
        cardObject.gameObject.GetComponent<CardObjectImage>().isToBeDeleted = true;
        cardDelteList.Add(cardObject);
        resultMana += cardObject.gameObject.GetComponent<CardObjectImage>().manaDiscardValue;
        CheckAndDisplayResultMana();

        if (cardRepeat == false)
        {

        }
    }

    void RandomizeDeck()
    {
        foreach (GameObject theCard in theDeck)
        {
            if (theCard.GetComponent<CardObjectImage>().isToBeRefreshed)
            {
                theCard.GetComponent<CardObjectImage>().isToBeRefreshed = true;
                // theCard.SetActive(true);
                int randomIndex = UnityEngine.Random.Range(0, theCardList.Count);
                GameObject randomCard = theCardList[randomIndex];
                theCardList.RemoveAt(randomIndex);
                theCard.GetComponent<CardObjectImage>().ChangeCard(randomCard.GetComponent<CardObjectImage>());
            }
        }

        cardAmountDisplay.text = "Card amount: " + theCardList.Count.ToString();
    }

    void UnselectCard(GameObject theCard)
    {
        if (theCard.GetComponent<CardObjectImage>().isMarked)
        {
            UpdateCardStatus(theCard, false, true, true);
            cardSelectList.Remove(theCard);
        }

        if (theCard.GetComponent<CardObjectImage>().isToBeDeleted)
        {
            UpdateCardStatus(theCard, true, true, true);
            cardDelteList.Remove(theCard);
        }

        if (cardDelteList.Contains(theCard))
        {
            cardDelteList.Remove(theCard);
        }

        if (cardSelectList.Contains(theCard))
        {
            cardSelectList.Remove(theCard);
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
            theCard.gameObject.GetComponent<CardObjectImage>().isDeleted = true;
            theCard.gameObject.GetComponent<TweenStuff>().StartMoving();
        }
    }

    void UpdateCardStatus(GameObject theCard, bool istoBeMarked, bool extractMana, bool removeFromList, bool useCardDiscardCost = false)
    {
        if (istoBeMarked)
        {
            if (theCard.gameObject.GetComponent<CardObjectImage>().isToBeDeleted)
            {
                if (extractMana)
                {
                    if (useCardDiscardCost)
                    {
                        resultMana -= theCard.gameObject.GetComponent<CardObjectImage>().manaDiscardValue;
                    }

                    else
                    {
                        resultMana -= theCard.gameObject.GetComponent<CardObjectImage>().manaCost;
                    }

                    CheckAndDisplayResultMana();
                }

                theCard.transform.Find("cardMarkDelete").gameObject.SetActive(false);
                theCard.gameObject.GetComponent<CardObjectImage>().isToBeDeleted = false;

                if (removeFromList)
                {
                    cardDelteList.Remove(theCard);
                }

            }
        }

        else
        {
            if (theCard.gameObject.GetComponent<CardObjectImage>().isMarked)
            {
                if (extractMana)
                {
                    if (useCardDiscardCost)
                    {
                        resultMana += theCard.gameObject.GetComponent<CardObjectImage>().manaDiscardValue;
                    }

                    else
                    {
                        resultMana += theCard.gameObject.GetComponent<CardObjectImage>().manaCost;
                    }

                    CheckAndDisplayResultMana();
                }

                theCard.transform.transform.Find("cardMark").gameObject.SetActive(false);
                theCard.gameObject.GetComponent<CardObjectImage>().isMarked = false;
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


    private IEnumerator DisplayStartEnemyDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageDisplayTimeStart);
        }
    }

    private IEnumerator DisplayEnemyDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageDisplayTime);
            enemyDamageDisplay.gameObject.SetActive(false);
        }
    }


    void ConfirmButtonOnClick()
    {
        if (playerTurn && !gameOver)
        {
            if (enemyAnimator.GetInteger("animState") != 1)
            {
                return;
            }

            foreach (GameObject theCard in cardDelteList)
            {
                theCard.GetComponent<CardObjectImage>().isToBeRefreshed = true;
                theCard.transform.Find("cardMarkDelete").gameObject.SetActive(false);
                RemoveCard(theCard);
            }

            int elementAmountAdd = 0;
            List<String> elementListCompare = new List<String>();
            List<GameObject> uniqueElementCardList = new List<GameObject>();

            //copy the values of elementList to elementListCompare. So modifying elementListCompare wont modify elementList
            foreach (String element in GameData.elementList)
            {
                elementListCompare.Add((string)element.Clone());
            }

            foreach (GameObject theCard in cardSelectList)
            {
                if (theCard.GetComponent<CardObjectImage>().theType.Contains("Element_"))
                {
                    elementAmountAdd++;
                }

                foreach (String element in elementListCompare)
                {
                    if (theCard.GetComponent<CardObjectImage>().theType == element)
                    {
                        elementListCompare.Remove(element);
                        uniqueElementCardList.Add(theCard);
                        break;
                    }
                }

                if (uniqueElementCardList.Count > 2)
                {
                    return;
                }
            }

            if (uniqueElementCardList.Count == 0)
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
                if (elementAmountAdd < 2)
                {
                    return;
                }

                int valueSum = 0;
                int multiplierSum = 0;
                List<string> theSelectedElements = new List<string>();

                for (int i = 0; i < cardSelectList.Count; i++)
                {
                    //adds the multipler values
                    if (cardSelectList[i].GetComponent<CardObjectImage>().theType.Contains("Multiplier"))
                    {
                        multiplierSum += cardSelectList[i].GetComponent<CardObjectImage>().multiplierNumber;
                    }

                    else
                    {
                        //does value sum
                        if (cardSelectList[i].GetComponent<CardObjectImage>().doesHealing)
                        {
                            valueSum -= cardSelectList[i].GetComponent<CardObjectImage>().theValue;
                        }

                        valueSum += cardSelectList[i].GetComponent<CardObjectImage>().theValue;
                        theSelectedElements.Add(cardSelectList[i].GetComponent<CardObjectImage>().theType);
                    }
                }

                //if there is no multipllier value simply, do no multiplier bonus aka * 1
                if (multiplierSum == 0)
                {
                    multiplierSum = 1;
                }

                int theNumber = 0;
                theNumber = valueSum * multiplierSum; //base damage
                theNumber -= theEnemy.GetComponent<CharacterObject>().DetermineResistance(theSelectedElements); //dealing with enemy resistance

                //there will only be two unique element at most (uniqueElementCardList.count is either 2 or 1)
                //so, checking if the two unique element combination matches one of the natural combos
                //(GameData.combosKnown) is the natural combos
                if (uniqueElementCardList.Count > 1)
                {
                    GameData.Combo combo = new GameData.Combo();
                    combo.ingredient1 = uniqueElementCardList[0].GetComponent<CardObjectImage>().theType;
                    combo.ingredient2 = uniqueElementCardList[1].GetComponent<CardObjectImage>().theType;
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
                        theNumber += (theSelectedElements.Count * comboBonusValue); //natural combo calculation.  dealing bonus damage depends on the amount of element cards and the cobmo bonus multiplier set
                    }
                }

                //learned combos calculation (combos that are known at start or from shop, has their own bonus value)
                foreach (GameData.Mixture theMixure in GameData.mixturesKnown)
                {
                    GameData.Mixture checkMixture = new GameData.Mixture();
                    checkMixture.theElements = new List<string>();
                    foreach (GameObject theCard in cardSelectList)
                    {
                        checkMixture.theElements.Add(theCard.GetComponent<CardObjectImage>().theType);
                    }

                    //mixture matched, does the value the mixture has
                    if (checkMixture.CheckMixtureMatched(theMixure))
                    {
                        if (theMixure.doesHealing)
                        {
                            theNumber -= theMixure.value;
                        }

                        theNumber += theMixure.value; //learned combo calculation
                        break;
                    }
                }

                theEnemy.GetComponent<CharacterObject>().theHealth -= theNumber;

                if (theNumber > 0)
                {
                    enemyAnimator.SetInteger("animState", 2);
                }


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
                    theCard.GetComponent<CardObjectImage>().isToBeRefreshed = true;
                    RemoveCard(theCard);
                }
                cardSelectList.Clear();
            }

            //if (resultMana > manaMaxValue)
            //{
            //    resultMana = manaMaxValue;
            //}
            cardDelteList.Clear();
            CheckAndDisplayResultMana();
            mana = resultMana;
            CheckAndDisplayMana(false);
        }
    }

    void EndTurnButtonClick()
    {
        if (playerTurn && !gameOver)
        {
            if (enemyAnimator.GetInteger("animState") == 1)
            {
                playerTurn = false;
            }
        }
    }
}

