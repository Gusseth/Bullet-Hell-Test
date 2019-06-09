using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI elements to the right side of the screen and adjusts as variables change.
/// </summary>
public class StatusHandler : MonoBehaviour
{

// Variable Declaration

    // Shortcut Declarations ///////////////////////////////////////////////////////////////////////////////////////////
    /// <summary> The Difficulty image GameObject. </summary>
    public GameObject difficulty;

    /// <summary> The Hi-Score GameObject. </summary>
    public GameObject hiScore;

    /// <summary> The Score GameObject. </summary>
    public GameObject score;

    /// <summary> The Lives GameObject. </summary>
    public GameObject lives;

    /// <summary> The Hearts GameObject inside lives. </summary>
    public GameObject heartsContainer;

    /// <summary> The Bombs GameObject. </summary>
    public GameObject bombs;

    /// <summary> The Stars GameObject inside lives. </summary>
    public GameObject starsContainer;

    /// <summary> The Power GameObject. </summary>
    public GameObject power;

    /// <summary> The Current Power GameObject, the integer of the number. </summary>
    public GameObject currentPower;

    /// <summary> The decimal of the Current Power GameObject. </summary>
    GameObject currentPowerDecimal;

    /// <summary> The Max Power GameObject, the integer of the number. </summary>
    public GameObject maxPower;

    /// <summary> The decimal of the Max Power GameObject. </summary>
    GameObject maxPowerDecimal;

    /// <summary> The Value GameObject. </summary>
    public GameObject value;

    /// <summary> The graze GameObject. </summary>
    public GameObject graze;

    /// <summary> The sprite for an empty heart. </summary>
    public Sprite emptyHeart;

    /// <summary> The sprite for a filled heart. </summary>
    public Sprite filledHeart;

    /// <summary> The sprite for an empty star. </summary>
    public Sprite emptyStar;

    /// <summary> The sprite for a filled star. </summary>
    public Sprite filledStar;

    // Technical Variables ///////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> List of all hearts under the lives GameObject. </summary>
    public List<GameObject> hearts = new List<GameObject>();

    /// <summary> List of all stars under the bombs GameObject. </summary>
    public List<GameObject> stars = new List<GameObject>();

    /// <summary> Local copy of the player's score count so that SetScore() is called when there is a mismatch for optimization. </summary>
    ulong localScore;

    /// <summary> Local copy of the player's lives count so that SetLives() is called when there is a mismatch for optimization. </summary>
    int localLives;

    /// <summary> Local copy of the player's bombs count so that SetBombs() is called when there is a mismatch for optimization. </summary>
    int localBombs;

    /// <summary> Local copy of the player's power so that SetPower() is called when there is a mismatch for optimization. </summary>
    float localPower;

    /// <summary> Local copy of the player's graze count so that SetGraze() is called when there is a mismatch for optimization. </summary>
    int localGraze;

    // Public Methods and Functions 


    // Private Methods and Functions

    /// <summary>
    /// Processes the backend for the Score UI.
    /// </summary>
    void SetScore()
    {
        localScore = Environment.playerHandler.score;
        string stringedScore = String.Format("{0:n0}", localScore); // Adds the separating commas
        score.transform.GetComponentInChildren<Text>().text = stringedScore;

        if (Environment.playerHandler.score >= GameManager.hiScore)
        {
            hiScore.transform.GetComponentInChildren<Text>().text = stringedScore;
        }
    }

    /// <summary>
    /// Processes the backend for the Lives UI.
    /// </summary>
    void SetLives()
    {
        localLives = Environment.playerHandler.lives;
        for (int i = 0; i != 8; i++)
        {
            if (i < localLives)
            {
                hearts[i].GetComponent<Image>().sprite = filledHeart;
            }
            else
            {
                hearts[i].GetComponent<Image>().sprite = emptyHeart;
            }
        }
    }

    /// <summary>
    /// Processes the backend for the Bombs UI.
    /// </summary>
    void SetBombs()
    {
        localBombs = Environment.playerHandler.bombs;
        for (int i = 0; i != 8; i++)
        {
            if (i < localBombs)
            {
                stars[i].GetComponent<Image>().sprite = filledStar;
            }
            else
            {
                stars[i].GetComponent<Image>().sprite = emptyStar;
            }
        }
    }

    /// <summary>
    /// Processes the backend for the Power UI.
    /// </summary>
    void SetPower()
    {
        localPower = Environment.playerHandler.power;
        int wholeNumber = (int)localPower;

        currentPower.GetComponent<Text>().text = wholeNumber.ToString();
        currentPowerDecimal.GetComponent<Text>().text = (localPower - wholeNumber).ToString(".00");

        // Changes colour based on whether or not max power has been achieved.
        if (localPower != Environment.playerHandler.player.maxPower)
        {
            // If the current power is below the maximum power, colour is orange
            Color colour = new Color(1, (130F / 255F), (16F / 255F)); // dab. colour > color
            currentPower.GetComponent<Text>().color = colour;
            currentPowerDecimal.GetComponent<Text>().color = colour;
        }
        else
        {
            // If the current power is equal to the maximum power, colour is gold yellow
            Color colour = new Color(1, (211F / 255F), (32F / 255F)); // dab. colour > color
            currentPower.GetComponent<Text>().color = colour;
            currentPowerDecimal.GetComponent<Text>().color = colour;
        }
    }

    /// <summary>
    /// Sets the maximum power for the Power UI. This function is only run once.
    /// </summary>
    void SetMaxPower()
    {
        float maxPow = Environment.playerHandler.player.maxPower;
        int wholeNumber = (int)maxPow;

        maxPower.GetComponent<Text>().text = wholeNumber.ToString();
        maxPowerDecimal.GetComponent<Text>().text = (maxPow - wholeNumber).ToString(".00");
    }

    /// <summary>
    /// Processes the backend for the Graze UI Counter.
    /// </summary>
    void SetGraze()
    {
        localGraze = Environment.playerHandler.graze;
        graze.transform.GetComponentInChildren<Text>().text = String.Format("{0:n0}", localGraze);
    }


    // Start is called before the first frame update
    void Start()
    {
        localLives = Environment.playerHandler.lives;

        // Assigning the GameObjects for the decimals.
        currentPowerDecimal = currentPower.transform.GetChild(0).gameObject;
        maxPowerDecimal = maxPower.transform.GetChild(0).gameObject;

        foreach (Transform heart in heartsContainer.transform)
        {
            // Adding all heart GameObjects for ease of use.
            hearts.Add(heart.gameObject);
        }

        foreach (Transform star in starsContainer.transform)
        {
            // Same thing for bombs and stars.
            stars.Add(star.gameObject);
        }

        SetLives();
        SetBombs();
        SetPower();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Environment.playerHandler.score != localScore)
        {
            // Fires when there is a mismatch in score.
            SetScore();
        }

        if (Environment.playerHandler.power != localPower)
        {
            // Fires when there is a mismatch in power.
            SetPower();
        }

        if (Environment.playerHandler.lives != localLives)
        {
            // Fires when there is a mismatch in lives count.
            SetLives();
        }

        if (Environment.playerHandler.bombs != localBombs)
        {
            // Fires when there is a mismatch in bomb count.
            SetBombs();
        }

        if (Environment.playerHandler.graze != localGraze)
        {
            // Fires when there is a mismatch in graze count.
            SetGraze();
        }
    }
}
