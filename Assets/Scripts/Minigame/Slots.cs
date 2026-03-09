using JetBrains.Annotations;
using NUnit.Framework;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;

public class Slots : MonoBehaviour
{
    [Header("Game Settings")]
    public bool isTalking = false;
    public bool canMove = true;
    public bool playAgain = true;
    public bool gameActive = false;

    public bool playerNear = false;

    [Header("Slots Symbols")]
    public Sprite[] slotSymbols; //array of all the symbols on the slot machine
    public int[] slotValues; //array for the value of each symbol on the slot machine

    public Sprite[] creditSprites; //sprites for credits

    public int[] feed1; //the feed for column 1
    public int[] feed2; //the feed for column 2
    public int[] feed3; //the feed for column 3
    public int[] feed4; //the feed for column 4
    public int[] feed5; //the feed for column 5

    [Header("Slot UI - Base")]
    public GameObject slotScreen; //background screen
    //dialogue
    public GameObject dialogueUI; //background panel for dialogue
    public TMP_Text dialogueText; //plays jackpot text and any other flavor text
    public GameObject lever; //lever (make button)

    [Header("Slot UI - Credits")]
    public GameObject creditsPanel;
    public GameObject creditsNumber1;
    public GameObject creditsNumber2;
    public GameObject creditsNumber3;
    public GameObject creditsNumber4;

    //*SLOTS ITSELF* in rows and columns, 3X5 = 15 spaces//
    [Header("Slot UI - Row 1")]
    public GameObject r1c1; 
    public GameObject r1c2;
    public GameObject r1c3;
    public GameObject r1c4;
    public GameObject r1c5;

    [Header("Slot UI -  Row 2")]
    public GameObject r2c1;
    public GameObject r2c2;
    public GameObject r2c3;
    public GameObject r2c4;
    public GameObject r2c5;

    [Header("Slot UI - Row 3")]
    public GameObject r3c1;
    public GameObject r3c2;
    public GameObject r3c3;
    public GameObject r3c4;
    public GameObject r3c5;

    [Header("Slot UI - Combo Lines")]
    public GameObject straightAcrossTop;
    public GameObject straightAcrossMiddle;
    public GameObject straightAcrossBottom;

    public GameObject straightDown1;
    public GameObject straightDown2;
    public GameObject straightDown3;
    public GameObject straightDown4;
    public GameObject straightDown5;

    public GameObject acrossLeftUp;
    public GameObject acrossLeftDown;
    public GameObject acrossRightUp;
    public GameObject acrossRightDown;


    [Header("Slots Minigame Data")]
    public string[] dialogueLines = new string[] { "", "JACKPOT!"}; //dialogue the minigame can say
    public int dialogueIndex = 0; //number to call what dialogue is said
    public int credits; //credits whole number
    public string creditsString;
    public int ones;
    public int tens;
    public int hundreds;
    public int thousands;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetInt("credits", credits);
        LoadCreditsUI();

        if (playerNear && Input.GetButtonDown("Interact"))
        {
            isTalking = true;
        }

        //if talking to the Slot NPC
        if (isTalking && !gameActive)
        {
            playAgain = true;
            canMove = false; // you don't want player to be able to move around while Gambling so need to freeze movement
            ShowUI(slotScreen); //shows all ui related to slots
            dialogueIndex = 0;

            if (!playAgain) { HideUI(slotScreen); } //hides all ui related to blackjack
        }
        if (!isTalking) { canMove = true; } // return movement if not talking to minigame npc

        if(gameActive)
        {
            //this is where stuff that happens inside the game goes
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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

    public void LoadCreditsUI()
    {
        UpdateCreditsSprites(); //update the sprites for UI before showing and revealing them

        if (credits >= 0) { ShowUI(creditsNumber1); }//if credits is in ones space

        if (credits > 9) { ShowUI(creditsNumber2); } //if credits is in tens space show second digit
        if (credits < 10) { HideUI(creditsNumber2); } //if it is below that then hide it

        if (credits > 99) { ShowUI(creditsNumber3); }  //if credits is in hundreds space show third digit
        if (credits < 100) { HideUI(creditsNumber3); } //if it is below that then hide it

        if (credits > 999) { ShowUI(creditsNumber4); } //if credits is in thousands space
        if (credits < 1000) { HideUI(creditsNumber4); } //if it is below that then hide it
    }

    public void UpdateCreditsSprites()
    {
        credits = PlayerPrefs.GetInt("credits");
        UpdateCreditsDigits();

        if (credits < 9)
        {
            creditsNumber1.GetComponent<Image>().sprite = creditSprites[ones]; //update credit number sprite
        }

        if (credits > 9 && credits < 100)
        {
            creditsNumber2.GetComponent<Image>().sprite = creditSprites[ones]; //update credit number sprite
            creditsNumber1.GetComponent<Image>().sprite = creditSprites[tens]; //update credit number sprite
        }

        if (credits > 99 && credits < 1000)
        {
            creditsNumber3.GetComponent<Image>().sprite = creditSprites[ones]; //update credit number sprite
            creditsNumber2.GetComponent<Image>().sprite = creditSprites[tens]; //update credit number sprite
            creditsNumber1.GetComponent<Image>().sprite = creditSprites[hundreds]; //update credit number sprite
        }

        if (credits > 999)
        {
            creditsNumber4.GetComponent<Image>().sprite = creditSprites[ones]; //update credit number sprite
            creditsNumber3.GetComponent<Image>().sprite = creditSprites[tens]; //update credit number sprite
            creditsNumber2.GetComponent<Image>().sprite = creditSprites[hundreds]; //update credit number sprite
            creditsNumber1.GetComponent<Image>().sprite = creditSprites[thousands]; //update credit number sprite
        }

    }

    public void UpdateCreditsDigits()
    {
        creditsString = credits.ToString(); //convert to string

        if(creditsString.Length == 4) //thousands
        {
            ones = creditsString[0];
            ones = CharToInt(ones);

            tens = creditsString[1];
            tens = CharToInt(tens);

            hundreds = creditsString[2];
            hundreds = CharToInt(hundreds);

            thousands = creditsString[3];
            thousands = CharToInt(thousands);
        }

        if (creditsString.Length == 3) //hundreds
        {
            ones = creditsString[0];
            ones = CharToInt(ones);

            tens = creditsString[1];
            tens = CharToInt(tens);

            hundreds = creditsString[2];
            hundreds = CharToInt(hundreds);

            thousands = 0;
        }

        if (creditsString.Length == 2) //tens
        {
            ones = creditsString[0];
            ones = CharToInt(ones);

            tens = creditsString[1];
            tens = CharToInt(tens);

            hundreds = 0;
            thousands = 0;
        }

        if (creditsString.Length == 1) //ones
        {
            ones = creditsString[0];
            ones = CharToInt(ones);

            tens = 0;
            hundreds = 0;
            thousands = 0;
        }

    }

    public int CharToInt(int character)
    {
        if (character == 48) { return 0; }
        if (character == 49) { return 1; }
        if (character == 50) { return 2; }
        if (character == 51) { return 3; }
        if (character == 52) { return 4; }
        if (character == 53) { return 5; }
        if (character == 54) { return 6; }
        if (character == 55) { return 7; }
        if (character == 56) { return 8; }
        if (character == 57) { return 9; }
        else { return 0; }
    }

    public void PullLever()
    {
        gameActive = true;
    }
    
    public void LoadFeed(int[] feed)
    {
        //this is for code for filling the slots itself
    }
}
