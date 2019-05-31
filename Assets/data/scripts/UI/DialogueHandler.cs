using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Monobehaviour responsible for creating and managing anything related to game dialogue.
/// </summary>
public class DialogueHandler : MonoBehaviour
{
    // Variable Declaration

    /// <summary> The 'Speech Container' GameObject. </summary>
    public GameObject speechContainer;

    /// <summary> The current speaker. </summary>
    public static Dialogue.Speaker speaker;

    // Title Card Variables

    /// <summary> The 'Title Card' GameObject. </summary>
    public GameObject bossTitleCard;

    /// <summary> The image behind the boss' name in the title card. </summary>
    GameObject titleCardSymbol;

    /// <summary> The text that includes the boss' name in the title card. </summary>
    GameObject titleCardBossName;


    // Player Dialogue Variables

    /// <summary> The 'Player Body' GameObject. </summary>
    public GameObject playerBody;

    /// <summary> The 'Player Face' GameObject. </summary>
    GameObject playerFace;

    /// <summary> The Image component of the 'Player Face' GameObject. </summary>
    Image playerFaceImage;

    /// <summary> The 'Player Balloon' GameObject. </summary>
    public GameObject playerBalloon;

    /// <summary> The TextMeshPro component for the player. </summary>
    TextMeshProUGUI playerText;


    // Enemy Dialogue Variables

    /// <summary> The 'Enemy Body' GameObject. </summary>
    public GameObject enemyBody;

    /// <summary> The 'Enemy Face' GameObject. </summary>
    GameObject enemyFace;

    /// <summary> The Image component of the 'Enemy Face' GameObject. </summary>
    Image enemyFaceImage;

    /// <summary> The 'Enemy Balloon' GameObject. </summary>
    public GameObject enemyBalloon;

    /// <summary> The TextMeshPro component for the enemy. </summary>
    TextMeshProUGUI enemyText;


    // Dialogue Table

    /// <summary> List of all dialogue lines in this encounter. </summary>
    public List<Dialogue> DialogueTable;

    // Public Methods and Functions

    /// <summary>
    /// Displays the inputted dialogue object. 
    /// </summary>
    /// <param name="dialogue">The dialogue to be displayed</param>
    public void RunDialogue(Dialogue dialogue)
    {
        GameObject subjectBody = null;
        Image subjectFaceImage = null;
        TextMeshProUGUI subjectText = null;
        speaker = dialogue.speaker; // Sets the current speaker

        // Determining which references should be used based on speaker, "deselects" them too
        if (dialogue.speaker == Dialogue.Speaker.player)
        {
            // Player as speaker

            subjectBody = playerBody;
            subjectFaceImage = playerFaceImage;
            subjectText = playerText;

            // Greying out the non-speaker
            enemyBody.GetComponent<Image>().color = Environment.ColorInt(100, 100, 100);
            enemyFaceImage.color = Environment.ColorInt(100, 100, 100);

            // Not greying out the speaker
            playerBody.GetComponent<Image>().color = Environment.ColorInt(255, 255, 255);
            playerFaceImage.color = Environment.ColorInt(255, 255, 255);
        }
        else if (dialogue.speaker == Dialogue.Speaker.enemy)
        {
            // Enemy as speaker

            subjectBody = enemyBody;
            subjectFaceImage = enemyFaceImage;
            subjectText = enemyText;

            // Greying out the non-speaker
            playerBody.GetComponent<Image>().color = Environment.ColorInt(100, 100, 100);
            playerFaceImage.color = Environment.ColorInt(100, 100, 100);

            // Not greying out the speaker
            enemyBody.GetComponent<Image>().color = Environment.ColorInt(255, 255, 255);
            enemyFaceImage.color = Environment.ColorInt(255, 255, 255);
        }


        subjectFaceImage.sprite = Dialogue.ParseFace(dialogue.expression, dialogue.speaker);
        subjectText.text = dialogue.message;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Assigning variables for player dialogue
        enemyFace = Environment.FindChild("Face", playerBody);
        playerFaceImage = playerFace.GetComponent<Image>();
        playerText = Environment.FindChild("Text", playerBalloon).GetComponent<TextMeshProUGUI>();
        // For the boss' end
        enemyFace = Environment.FindChild("Face", enemyBody);
        enemyFaceImage = enemyFace.GetComponent<Image>();
        enemyText = Environment.FindChild("Text", enemyBalloon).GetComponent<TextMeshProUGUI>();

        // Boss title card
        titleCardSymbol = Environment.FindChild("Symbol", bossTitleCard);
        titleCardBossName = Environment.FindChild("Boss Name Card", bossTitleCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/// <summary>
/// Dialogue object. Use this to create dialogue!
/// </summary>
[System.Serializable]
public class Dialogue
{

 // Variable Declarations

    /// <summary> The speaker of this message. </summary>
    public Speaker speaker;

    /// <summary> The speaker's facial expression. </summary>
    public Face expression;
    
    /// <summary> The message conveyed. </summary>
    public string message;

    /// <summary> Enumeration of all valid facial expressions. </summary>
    public enum Face
    {
        normal, angry, challenge, confused, happy, nervous, smug, surprised
    }

    /// <summary> Enumeration of all valid speakers. </summary>
    public enum Speaker
    {
        player, enemy
    }

// Class Struct
    /// <summary>
    /// Creates a dialogue object to display text on dialogue boxes.
    /// </summary>
    /// <param name="speaker">The speaker of this message</param>
    /// <param name="expression">The speaker's facial expression</param>
    /// <param name="message">The message to be conveyed</param>
    public Dialogue(Speaker speaker, Face expression, string message)
    {
        this.speaker = speaker;
        this.expression = expression;
        this.message = message;
    }

    // Public Methods and Functions

    /// <summary>
    /// Returns a Sprite with the inputted facial expression.
    /// </summary>
    /// <param name="face">The facial expression to be returned</param>
    /// <param name="speaker">The speaker (possibly) subject to an unnatural smugness</param>
    /// <returns>Sprite Face</returns>
    public static Sprite ParseFace(Face face, Speaker speaker)
    {
        string fileSuffix;
        switch (face)
        {
            case Face.normal:
                fileSuffix = "no";
                break;
            case Face.angry:
                fileSuffix = "an";
                break;
            case Face.challenge:
                fileSuffix = "n2";
                break;
            case Face.confused:
                fileSuffix = "pr";
                break;
            case Face.happy:
                fileSuffix = "hp";
                break;
            case Face.nervous:
                fileSuffix = "sw";
                break;
            case Face.smug:
                fileSuffix = "dp";
                break;
            case Face.surprised:
                fileSuffix = "sp";
                break;
            default:
                fileSuffix = "no";
                Debug.LogWarning("Invalid Face '" + face.ToString() + "'parsed!");
                break;
        }

        Sprite faceSprite; // Sprite to be returned later

        if (speaker == Speaker.player)
        {
            // Expression below determines the appropriate 'plXX' folder and files 'face_plXX' based on the current player.
            string playerFileRoot = "pl" + Environment.playerHandler.player.character.ToString("00");

            faceSprite = Resources.Load<Sprite>("th17demo/face/" + playerFileRoot + "/face_" + playerFileRoot + fileSuffix + ".png");
        }
        else if (speaker == Speaker.enemy)
        {
            Environment.Stage stage = GameManager.GetStage();

            // Expression below determines the appropriate 'enemyX' folder
            string enemyFolderRoot = "enemy" + stage;

            // Expression below determines the appropriate 'faceXXYY' file
            string faceFile = "face" + stage.ToString("00") + fileSuffix;

            faceSprite = Resources.Load<Sprite>("th17demo/face/" + enemyFolderRoot + "/" + faceFile + ".png");
        }
        else
        {
            Debug.LogWarning("Cannot parse speaker!");
            faceSprite = null;
        }

        return faceSprite;
    }
}