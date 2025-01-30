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
    [SerializeField] List<GameObject> theDeckPositionRef = new List<GameObject>();

    [SerializeField] Slider enemyHealthSlider;
    [SerializeField] Slider playerHealthSlider;
    [SerializeField] Slider playerManaSlider;

    bool playerTurn = true;
    bool gameOver;
    bool gameStart = false;
    int resultMana;

    public float damageDisplayTimeStart;
    public float damageDisplayTime;

    public float startTime = 0.001f;

    Animator enemyAnimator;
    Coroutine enemyDisplayTimer;

    public GameObject theMagicObject;
    bool magicIsShooting;
    public float magicAttackSpeed = 0.5f;

    public Transform MagicAttackLocation;



    public GameObject theCamera;
    AudioSource theAudioSource;
    public GameObject theCameraAudioSource2;
    AudioSource theAudioSource2;

    public AudioClip audioClipFireSelect; //Played when a fire card is selected to be played 
    public AudioClip audioClipLightningSelect;
    public AudioClip audioClipRockSelect;
    public AudioClip audioClipVineSelect;
    public AudioClip audioClipWaterSelect;
    public AudioClip audioClipCardSuffle; //Played whenever a card is shuffled into the player’s hand, played once for each card 
    public AudioClip audioClipWendigoDead; // Played when the wendigo runs out of HP 
    public AudioClip audioClipWendigoHit; //Played after the spell is cast by the player and hits the wendigo 
    public AudioClip audioClipWendigoSwipe; // Played when the wendigo hits the player 
    void Start()
    {
        theAudioSource = theCamera.GetComponent<AudioSource>();
        theAudioSource2 = theCameraAudioSource2.GetComponent<AudioSource>();
        theMagicObject.GetComponent<MagicAttackVisualizer>().element1Count = 0;
        theMagicObject.GetComponent<MagicAttackVisualizer>().element2Count = 0;

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

        for (int i = 0; i < theDeckPositionRef.Count; i++)
        {
            theDeckPositionRef[i].SetActive(false);
        }

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
        yield return new WaitForSeconds(startTime);

        if (theCardList.Count < theDeck.Count)
        {
            Debug.Log("No card left!");
            DG.Tweening.DOTween.KillAll();
            SceneManager.LoadScene("MainMenu");
            gameOver = true;
            yield break;
        }

        for (int i = 0; i < theDeck.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, theCardList.Count);
            GameObject randomCard = theCardList[randomIndex];
            theCardList.RemoveAt(randomIndex);
            theDeck[i].GetComponent<CardObjectImage>().ChangeCard(randomCard.GetComponent<CardObjectImage>());
            theDeck[i].GetComponent<CardObjectImage>().isMoving = false;
            theDeck[i].GetComponent<TweenStuff>().MoveTo(theDeckPositionRef[i].transform);
        }

        theAudioSource.clip = audioClipCardSuffle;
        theAudioSource.Play();
        gameStart = true;
        cardAmountDisplay.text = "Card amount: " + theCardList.Count.ToString();

    }

    void Update()
    {
        if (magicIsShooting)
        {

            MagicAttackVisualizer magicAttackVisualizer = theMagicObject.GetComponent<MagicAttackVisualizer>();

            if (magicAttackVisualizer.hitPreCooldown)
            {
                magicAttackVisualizer.hitPreCooldown = false;
                enemyAnimator.SetInteger("animState", 2);
                theAudioSource2.clip = audioClipWendigoHit;
                theAudioSource2.Play();
            }

            if (magicAttackVisualizer.element1Count == 0
                && magicAttackVisualizer.element2Count == 0)
            {
                theMagicObject.gameObject.SetActive(false);
                magicIsShooting = false;
            }

            else
            {
                return;
            }
        }


        if (!gameStart)
        {
            return;
        }

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
                                    UpdateMagic();
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
                                    UpdateMagic();
                                }

                                //select unmarked card
                                else
                                {
                                    if (theDeckCard.GetComponent<CardObjectImage>().isMoving)
                                    {
                                        return;
                                    }
                                    HandleCardClick(theDeckCard);
                                    UpdateMagic();
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
                                if (theDeckCard.GetComponent<CardObjectImage>().isMoving)
                                {
                                    return;
                                }
                                HandleCardDelete(theDeckCard);
                                UpdateMagic();
                            }
                        }
                    }
                }
            }
        }

        //enemy's turn
        else
        {
            Debug.Log("StunBar: " + theEnemy.GetComponent<CharacterObject>().stunBar);
            theEnemy.GetComponent<CharacterObject>().CheckStunAtAll();
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
                    theCard.GetComponent<CardObjectImage>().isMoving = false;
                    theCard.gameObject.GetComponent<TweenStuff>().StartMovingBack();
                    amountCardMissing++;
                }
            }

            if (!theEnemy.GetComponent<CharacterObject>().isStuned)
            {
                enemyAnimator.SetInteger("animState", 3);
                theAudioSource2.clip = audioClipWendigoSwipe;
                theAudioSource2.Play();
                int theNumber;
                theNumber = UnityEngine.Random.Range(2, 21);
                thePlayer.GetComponent<CharacterObject>().theHealth -= theNumber;
                UpdateHealthPlayer();

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
            }

            else
            {
                Debug.Log("Enemy stunned");
            }

            theEnemy.GetComponent<CharacterObject>().TryCeaseStun();

            if (thePlayer.GetComponent<CharacterObject>().theHealth <= 0)
            {
                gameOver = true;
                Debug.Log("Enemy won");
                DG.Tweening.DOTween.KillAll();
                SceneManager.LoadScene("MainMenu");
                return;
            }

            if (theCardList.Count < amountCardMissing)
            {
                Debug.Log("No card left!");
                DG.Tweening.DOTween.KillAll();
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
            playerTurn = true;
        }
    }

    void HandleCardClick(GameObject cardObject)
    {
        if (gameOver)
        {
            return;
        }

        PlayCardsound(cardObject);

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

   //     PlayCardsound(cardObject);

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
        bool playSuffleSound = false;
        foreach (GameObject theCard in theDeck)
        {
            if (theCard.GetComponent<CardObjectImage>().isToBeRefreshed)
            {
                playSuffleSound = true;
                theCard.GetComponent<CardObjectImage>().isToBeRefreshed = true;
                // theCard.SetActive(true);
                int randomIndex = UnityEngine.Random.Range(0, theCardList.Count);
                GameObject randomCard = theCardList[randomIndex];
                theCardList.RemoveAt(randomIndex);
                theCard.GetComponent<CardObjectImage>().ChangeCard(randomCard.GetComponent<CardObjectImage>());
            }
        }

        if (playSuffleSound)
        {
            theAudioSource.clip = audioClipCardSuffle;
            theAudioSource.Play();
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
            theCard.GetComponent<CardObjectImage>().isMoving = true;
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
        if (magicIsShooting)
        {
            return;
        }
        if (!gameStart)
        {
            return;
        }    

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
            int stunValue = 0;

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

                ////apply possible rotation resistance
                if (theEnemy.GetComponent<CharacterObject>().RotationResistanceCanApply(theSelectedElements))
                {

                    float theNumberCalc = theNumber;
                    Debug.Log("Damage before: " + theNumber);
                    Debug.Log("Per result: " + (theNumberCalc * ((float)theEnemy.GetComponent<CharacterObject>().lastAttackResistancePercentage / 100)));
                    theNumberCalc = theNumberCalc - (theNumberCalc * ((float)theEnemy.GetComponent<CharacterObject>().lastAttackResistancePercentage / 100));

                    theNumber = (int)theNumberCalc;
                }
                theEnemy.GetComponent<CharacterObject>().RotateResistance(uniqueElementCardList); //rotate rotation ressitance
                theEnemy.GetComponent<CharacterObject>().theHealth -= theNumber; //final damage reduce
                stunValue += theEnemy.GetComponent<CharacterObject>().DetermineWeaknessStunt(theSelectedElements); //add stun
                theEnemy.GetComponent<CharacterObject>().CheckStunAtAll();


                theEnemy.GetComponent<CharacterObject>().DealStun(stunValue); //deal with possible stun

                if (theNumber > 0)
                {
                    UpdateMagic();
                }


                UpdateHealthEnemy();
                Debug.Log("Player deals: " + theNumber);

                //game over feature - if enemy got defeated
                if (theEnemy.GetComponent<CharacterObject>().theHealth <= 0)
                {
                    gameOver = true;
                    DG.Tweening.DOTween.KillAll();
                    SceneManager.LoadScene("Chapter 1 Map");
                    Debug.Log("Player won");
                    theAudioSource.clip = audioClipWendigoDead;
                    theAudioSource.Play();
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
            if (uniqueElementCardList.Count > 0)
            {
                ShootMagic();
            }
            

        }
    }

    void EndTurnButtonClick()
    {
        if (magicIsShooting)
        {
            return;
        }

        if (playerTurn && !gameOver)
        {
            if (enemyAnimator.GetInteger("animState") == 1)
            {
                playerTurn = false;
            }
        }
    }

    void ShootMagic()
    {
        magicIsShooting = true;
        theMagicObject.SetActive(true);
        MagicAttackVisualizer magicAttackVisualizer = theMagicObject.GetComponent<MagicAttackVisualizer>();
        magicAttackVisualizer.moveSpeed = magicAttackSpeed;
        magicAttackVisualizer.Shoot(MagicAttackLocation.position);
    }

    void UpdateMagic()
    {
        MagicAttackVisualizer magicAttackVisualizer = theMagicObject.GetComponent<MagicAttackVisualizer>();
        int elementAmountAdd = 0;
        List<String> elementListCompare = new List<String>();
        List<GameObject> uniqueElementCardList = new List<GameObject>();
        List<String> theSelectedElements = new List<String>();

        //copy the values of elementList to elementListCompare. So modifying elementListCompare wont modify elementList
        foreach (String element in GameData.elementList)
        {
            elementListCompare.Add((string)element.Clone());
        }

        foreach (GameObject theCard in cardSelectList)
        {
            theSelectedElements.Add(theCard.GetComponent<CardObjectImage>().theType);
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
        }

        theMagicObject.gameObject.SetActive(false);
        magicAttackVisualizer.element1Count = 0;
        magicAttackVisualizer.element2Count = 0;

        if (uniqueElementCardList.Count == 0)
        {
            return;
        }

        String element1 = uniqueElementCardList[0].GetComponent<CardObjectImage>().theType;
        String element2 = "";
        int amountElement1 = 0;
        int amountElement2 = 0;


        if (uniqueElementCardList.Count == 2)
        {
            element2 = uniqueElementCardList[1].GetComponent<CardObjectImage>().theType;
        }

        foreach (String theElement in theSelectedElements)
        {
            if (theElement == element1)
            {
                amountElement1++;
                continue;
            }

            if (uniqueElementCardList.Count == 2)
            {
                if (theElement == element2)
                {
                    amountElement2++;
                }
            }
        }

        if (amountElement1 > 0 ||
            amountElement2 > 0)
        {
            theMagicObject.gameObject.SetActive(true);
        }

        magicAttackVisualizer.SetElement(true, element1);
        magicAttackVisualizer.element1Count = amountElement1;

        

        if (uniqueElementCardList.Count == 2)
        {
            magicAttackVisualizer.SetElement(false, element2);
            magicAttackVisualizer.element2Count = amountElement2;
        }

        else
        {
            magicAttackVisualizer.element2Count = 0;
        }
    }

    void PlayCardsound(GameObject cardObject)
    {
        switch (cardObject.gameObject.GetComponent<CardObjectImage>().theType)
        {
            case "Element_Fire":
                theAudioSource.clip = audioClipFireSelect;
                theAudioSource.Play();
                break;
            case "Element_Thunder":
                theAudioSource.clip = audioClipLightningSelect;
                theAudioSource.Play();
                break;
            case "Element_Ground":
                theAudioSource.clip = audioClipRockSelect;
                theAudioSource.Play();
                break;
            case "Element_Green":
                theAudioSource.clip = audioClipVineSelect;
                theAudioSource.Play();
                break;
            case "Element_Water":
                theAudioSource.clip = audioClipWaterSelect;
                theAudioSource.Play();
                break;
        }
    }
}

