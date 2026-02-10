using JetBrains.Annotations;
using NUnit.Framework;
using TMPro;
using Unity.Burst.Intrinsics;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.Rendering;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;

public class BlackJack : MonoBehaviour
{
    [Header("Game Settings")]
    public bool isTalking = false;
    public bool canMove = true;
    public bool playAgain = true;
    public bool gameActive = false;

    public bool playerNear = false;

    [Header("Black Jack Cards")]
    public Sprite[] cardSprites; //array of card sprites
    public int[] Cards = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 10, 10 , 10,  2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 10, 10, 10,  2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 10, 10, 10, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 10, 10, 10 }; //cards in order
    //11 = Ace, after is J, K, Q (not value in game, just noting the value in their order in the sprites index)
    public int[] remainingCards; //duplicate array that will have cards removed as they are used

    [Header("Black Jack UI")]
    public GameObject blackJackScreen;
    //dialogue
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    //spaces for player's cards
    public GameObject playerCardSpace1; 
    public GameObject playerCardSpace2;
    public GameObject playerCardSpace3;
    public GameObject playerCardSpace4;
    public GameObject playerCardSpace5;
    public GameObject playerCardSpace6;
    public GameObject playerCardSpace7;
    public GameObject playerCardSpace8;

    //spaces for dealer's cards
    public GameObject dealerHideCard;
    public GameObject dealerCardSpace1;
    public GameObject dealerCardSpace2;
    public GameObject dealerCardSpace3;
    public GameObject dealerCardSpace4;
    public GameObject dealerCardSpace5;
    public GameObject dealerCardSpace6;
    public GameObject dealerCardSpace7;
    public GameObject dealerCardSpace8;

    //buttons to start game / play again
    public GameObject button1; //button that says yes 
    public GameObject button2; //button that says no
    public GameObject button3; //button for Hit
    public GameObject button4; //button for stand

    [Header("Black Jack Data")]
    public string[] dialogueLines = new string[] {"", "Do you want to play?", "You Win!", "You Lose!", "Do you want to play again?", "Tie!"}; //dialogue the dealer can say
    public int dialogueIndex = 0; //number to call what dialogue is said
    public int cardsDealt = 0; //numvber to track how many cards have been dealt

    public int randomCard; //number randomly generated to pick index
    public int randomCardValue; // randomly generated card's value
    public List<int> dealerHand; //array to track what cards are in the dealer's hand
    public int dealerHandValue; //sum of dealer hand
    public List<int> playerHand; //array to track what cards are in the player's hand
    public int playerHandValue; //sum of player hand

    public bool canHit = true; //as long as the player hasn't bust they can hit and get another card
    public bool bust = false; //met if player hand has sum that is over 21
    public bool onWinLose = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // initialize
        remainingCards = Cards;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerNear && Input.GetButtonDown("Interact"))
        {
            isTalking = true;
        }

        //if talking to the Black Jack NPC
        if(isTalking && !gameActive)
        {
            playAgain = true;
            canMove = false; // you don't want player to be able to move around while Gambling so need to freeze movement
            ShowUI(blackJackScreen); //shows all ui related to blackjack
            ShowUI(button1);
            ShowUI(button2);
            dialogueIndex = 1;
            StartDialogue();
            ShowUI(dialogueUI);
            
            if(!playAgain) { HideUI(blackJackScreen); } //hides all ui related to blackjack
        }
        if (!isTalking) { canMove = true; } // return movement if not talking to minigame npc

        if(gameActive) //space for active gameplay and things during it that need to be frequently checked on update -- past start
        {
            //buttons for hit me/ stay
            if(!bust)
            {
                //each turn check status
                playerHandValue = playerHand.Sum(); //calculate sum of player hand
                dealerHandValue = dealerHand.Sum(); //calculate sum of dealer hand
                if (playerHandValue > 21) { bust = true; } //if above 21 then bust
                if(playerHandValue == 21 && cardsDealt == 2) //if win on dealt
                {
                    WinGame();
                }
                if(playerHandValue == 21 && cardsDealt > 2) //if dealt to 21
                {
                    Stand();
                }
                if(playerHandValue < 21)
                {
                    ShowUI(button3);
                    ShowUI(button4);
                    bust = false;
                }
            }

            if(bust)
            {
                //lose the game
                LoseGame();
            }

            if(onWinLose) //if on win lose screen, you need input to move forward
            {
                if (Input.GetButton("Shoot") || Input.GetButton("Interact"))
                {
                    //move forward
                    HideAllCards();
                    dialogueIndex = 4;
                    ShowUI(button1);
                    ShowUI(button2);
                    HideUI(button3);
                    HideUI(button4);
                    gameActive = false;
                }
            }
        }
    }

    public void ShowUI(GameObject UI)
    {
        UI.SetActive(true);
    }

    public void HideUI(GameObject UI)
    {
        UI.SetActive(false);
    }

    public void StartGame()
    {
        //this is where gameplay is set up
        gameActive = true;
        onWinLose = false;
        HideUI(button1); //hide button that sais yes
        HideUI(button2); //hide button that says no
        ResetGame(); //reset all game data to ensure clean start
        HideUI(dialogueUI); //hide dialogue box
        remainingCards = Cards; //reset card deck

        Deal(0); //initial deal player card 1
        playerCardSpace1.GetComponent<Image>().sprite = cardSprites[randomCard]; //update card sprite
        Deal(0); //initial deal card 2
        playerCardSpace2.GetComponent<Image>().sprite = cardSprites[randomCard]; //update card sprite
        //initial deal show player cards
        ShowUI(playerCardSpace1);
        ShowUI(playerCardSpace2);

        Deal(1); //initial deal dealer card 1
        dealerCardSpace1.GetComponent<Image>().sprite = cardSprites[randomCard]; //update card sprite
        Deal(1); //initial deal dealer card 2
        dealerCardSpace2.GetComponent<Image>().sprite = cardSprites[randomCard]; //update card sprite
        //initial deal show dealer cards
        ShowUI(dealerHideCard);
        ShowUI(dealerCardSpace1);
        ShowUI(dealerCardSpace2);

        cardsDealt = 2;
    }

    public void Deal(int i)
    {
        randomDraw();
        if (remainingCards[randomCard] != 0)
        {
            randomCardValue = remainingCards[randomCard]; //save value of card
            remainingCards[randomCard] = 0; //null that card space so it can't be drawn again
            if(i == 0) { playerHand.Add(randomCardValue); } //if player turn
            if (i == 1) { dealerHand.Add(randomCardValue); } //if dealer turn
        }
        else Deal(i);
    }

    public void randomDraw()
    {
        randomCard = Random.Range(0, remainingCards.Length);
    }

    public void Hit()
    {
        if(cardsDealt == 2)
        {
            Deal(0);
            playerCardSpace3.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(playerCardSpace3);
            cardsDealt++;
            return;
        }
        if(cardsDealt == 3)
        {
            Deal(0);
            playerCardSpace4.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(playerCardSpace4);
            cardsDealt++;
            return;
        }
        if (cardsDealt == 4)
        {
            Deal(0);
            playerCardSpace5.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(playerCardSpace5);
            cardsDealt++;
            return;
        }
        if (cardsDealt == 5)
        {
            Deal(0);
            playerCardSpace6.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(playerCardSpace6);
            cardsDealt++;
            return;
        }
        if (cardsDealt == 6)
        {
            Deal(0);
            playerCardSpace7.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(playerCardSpace7);
            cardsDealt++;
            return;
        }
        if (cardsDealt == 7)
        {
            Deal(0);
            playerCardSpace8.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(playerCardSpace8);
            cardsDealt++;
            return;
        }
    }

    public void ShowDealerCards()
    {
        if(dealerHand.Count == 2)
        {
            Deal(1);
            dealerCardSpace3.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(dealerCardSpace3);
            return;
        }
        if (dealerHand.Count == 3)
        {
            Deal(1);
            dealerCardSpace4.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(dealerCardSpace4);
            return;
        }
        if (dealerHand.Count == 4)
        {
            Deal(1);
            dealerCardSpace5.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(dealerCardSpace5);
            return;
        }
        if (dealerHand.Count == 5)
        {
            Deal(1);
            dealerCardSpace6.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(dealerCardSpace6);
            return;
        }
        if (dealerHand.Count == 6)
        {
            Deal(1);
            dealerCardSpace7.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(dealerCardSpace7);
            return;
        }
        if (dealerHand.Count == 7)
        {
            Deal(1);
            dealerCardSpace8.GetComponent<Image>().sprite = cardSprites[randomCard];
            ShowUI(dealerCardSpace8);
            return;
        }
    }
    public void DealerTurn()
    {
        HideUI(dealerHideCard);
        dealerHandValue = dealerHand.Sum(); //calculate sum of dealer hand
        while (dealerHandValue < 17)
        {
            ShowDealerCards();
            dealerHandValue = dealerHand.Sum(); //calculate sum of dealer hand
        }

    }

    public void Stand()
    {
        //end playing
        HideUI(button3);
        HideUI(button4);

        DealerTurn();
        if ((playerHandValue < 21 && dealerHandValue < playerHandValue) || (playerHandValue == 21 && dealerHandValue != 21) || (dealerHandValue > 21)) //if player is less than 21, dealer is less than 21, but dealer has less than player win ; if player gets to 21 and dealer did not win
        {
            //win the game
            WinGame();
        }
        if (((playerHandValue < dealerHandValue) && dealerHandValue < 21) || (dealerHandValue == 21 && playerHandValue != 21)) //if player has less than dealer, and dealer has less than 21 fail ; if dealer got to 21 and player did not fail
        {
            //lose the game
            LoseGame();
        }
        if ((dealerHandValue == playerHandValue)) //if both dealer and player end with same amount, draw
        {
            //tie
            DrawGame();
        }

    }
    public void HideAllCards()
    {
        //hide player cards
        HideUI(playerCardSpace1);
        HideUI(playerCardSpace2);
        HideUI(playerCardSpace3);
        HideUI(playerCardSpace4);
        HideUI(playerCardSpace5);
        HideUI(playerCardSpace6);
        HideUI(playerCardSpace7);
        HideUI(playerCardSpace8);

        //hide dealer cards
        HideUI(dealerCardSpace1);
        HideUI(dealerCardSpace2);
        HideUI(dealerCardSpace3);
        HideUI(dealerCardSpace4);
        HideUI(dealerCardSpace5);
        HideUI(dealerCardSpace6);
        HideUI(dealerCardSpace7);
        HideUI(dealerCardSpace8);

        HideUI(dealerHideCard);
    }
    public void StartDialogue()
    {
        dialogueText.text = dialogueLines[dialogueIndex];
    }

    public void WinGame()
    {
        //win the game
        dialogueIndex = 2;
        StartDialogue();
        ShowUI(dialogueUI);
        onWinLose = true;
    }

    public void LoseGame()
    {
        dialogueIndex = 3;
        StartDialogue();
        ShowUI(dialogueUI);
        onWinLose = true;
    }

    public void DrawGame()
    {
        dialogueIndex = 5;
        StartDialogue();
        ShowUI(dialogueUI);
        onWinLose = true;
    }

    public void ResetGame()
    {
        cardsDealt = 0;
        dialogueIndex = 0;
        remainingCards = Cards; //reset deck
        playerHand.Clear(); //reset player hand
        dealerHand.Clear(); //reset dealer hand
        playerHandValue = 0;
        dealerHandValue = 0;
        bust = false;
    }

    public void EndGame()
    {
        HideUI(blackJackScreen);
        HideUI(dialogueUI);
        isTalking = false;
    }

    public bool returnMovement()
    {
        return canMove;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            playerNear = true;
        Debug.Log("OnCollisionEnter2D");
        Debug.Log(other.gameObject.name + " : " + gameObject.name + " : " + Time.time);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
        Debug.Log("OnCollisionExit2D");
        Debug.Log(other.gameObject.name + " : " + gameObject.name + " : " + Time.time);
    }
}
