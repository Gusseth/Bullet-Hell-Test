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
    public Dialogue.Speaker speaker;

    /// <summary> Is the player currently visible on the screen? </summary>
    public static bool playerDrawn = false;

    /// <summary> Is the enemy currently visible on the screen? </summary>
    public static bool enemyDrawn = false;

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

        // Determining which references should be used based on speaker, "deselects" them too
        if (dialogue.speaker == Dialogue.Speaker.player)
        {
            // Player as speaker

            subjectFaceImage = playerFaceImage;
            subjectFaceImage.overrideSprite = Dialogue.ParseFace(dialogue.expression, dialogue.speaker);
            subjectBody = playerBody;
            subjectText = playerText;


            // Animations played below if the speaker before is not the speaker now with the two sprites already loaded. This one slides the player in, enemy out.
            if (speaker != dialogue.speaker && speaker != Dialogue.Speaker.none)
            {
                speaker = dialogue.speaker;
                enemyBody.GetComponent<Animator>().Play("Slide Out");
                enemyBalloon.GetComponent<Animator>().Play("Slide Out");

                // Sliding the player in
                playerBody.GetComponent<Animator>().Play("Slide In");
                playerBalloon.GetComponent<Animator>().Play("Slide In");
            }

            // Otherwise, if there was no one speaking or the player is currently not drawn on the screen.
            if (speaker == Dialogue.Speaker.none || !playerDrawn)
            {
                playerDrawn = true;
                speaker = dialogue.speaker;
                playerBody.GetComponent<Animator>().SetBool("disappear", false);
                playerBody.GetComponent<Animator>().Play("Appear");
                playerBalloon.GetComponent<Animator>().Play("Slide In");
            }

        }
        else if (dialogue.speaker == Dialogue.Speaker.enemy)
        {
            // Enemy as speaker

            subjectFaceImage = enemyFaceImage;
            subjectFaceImage.overrideSprite = Dialogue.ParseFace(dialogue.expression, dialogue.speaker);
            subjectBody = enemyBody;
            subjectText = enemyText;


            // Animations played below if the speaker before is not the speaker now with the two sprites already loaded. This one slides the enemt in, player out.
            if (speaker != dialogue.speaker && speaker != Dialogue.Speaker.none)
            {
                speaker = dialogue.speaker;
                enemyBody.GetComponent<Animator>().Play("Slide In");
                enemyBalloon.GetComponent<Animator>().Play("Slide In");

                // Sliding the player out
                playerBody.GetComponent<Animator>().Play("Slide Out");
                playerBalloon.GetComponent<Animator>().Play("Slide Out");
            }

            // Otherwise, if there was no one speaking or the enemy is currently not drawn on the screen.
            if (speaker == Dialogue.Speaker.none || !enemyDrawn)
            {
                enemyDrawn = true;
                speaker = dialogue.speaker;
                enemyBody.GetComponent<Animator>().SetBool("disappear", false);
                enemyBody.GetComponent<Animator>().Play("Appear");
                enemyBalloon.GetComponent<Animator>().Play("Slide In");
            }
        }

        subjectText.text = dialogue.message;
    }

    /// <summary>
    /// Ends ALL dialogue, both speakers disappear.
    /// </summary>
    public void EndDialogue()
    {
        speaker = Dialogue.Speaker.none;
        playerDrawn = false;
        enemyDrawn = false;
        enemyBody.GetComponent<Animator>().SetBool("disappear", true);
        playerBody.GetComponent<Animator>().SetBool("disappear", true);
        playerBalloon.GetComponent<Animator>().Play("Slide Out");
        enemyBalloon.GetComponent<Animator>().Play("Slide Out");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Assigning variables for player dialogue
        playerFace = playerBody.transform.Find("Face").gameObject;
        playerFaceImage = playerFace.GetComponent<Image>();
        playerText = playerBalloon.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        
        // For the boss' end
        enemyFace = enemyBody.transform.Find("Face").gameObject;
        enemyFaceImage = enemyFace.GetComponent<Image>();
        enemyText = enemyBalloon.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        // Boss title card
        titleCardSymbol = bossTitleCard.transform.Find("Symbol").gameObject;
        titleCardBossName = bossTitleCard.transform.Find("Boss Name Card").gameObject;
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

    /// <summary> Set to true if the speaker's sprite should disappear from the screen after this message. </summary>
    public bool speakerDisappears;

    /// <summary> Enumeration of all valid facial expressions. </summary>
    public enum Face
    {
        normal, angry, challenge, confused, happy, nervous, smug, surprised
    }

    /// <summary> Enumeration of all valid speakers. </summary>
    public enum Speaker
    {
        none, player, enemy
    }

    // Class Struct

    /// <summary>
    /// Creates a dialogue object to display text on dialogue boxes.
    /// </summary>
    /// <param name="speaker">The speaker of this message</param>
    /// <param name="expression">The speaker's facial expression</param>
    /// <param name="message">The message to be conveyed</param>
    /// <param name="speakerDisappears">Set to true if the speaker's sprite should disappear from the screen after this message.</param>
    public Dialogue(Speaker speaker, Face expression, string message, bool speakerDisappears = false)
    {
        this.speaker = speaker;
        this.expression = expression;
        this.message = message;
        this.speakerDisappears = speakerDisappears;
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
            string playerFileRoot = "pl" + ((int)Environment.playerHandler.player.character).ToString("00");

            faceSprite = Resources.Load<Sprite>("th17demo/face/" + playerFileRoot + "/face_" + playerFileRoot + fileSuffix);
        }
        else if (speaker == Speaker.enemy)
        {
            Environment.Stage stage = GameManager.GetStage();

            // Expression below determines the appropriate 'enemyX' folder
            string enemyFolderRoot = "enemy" + (int)stage;

            // Expression below determines the appropriate 'faceXXYY' file
            string faceFile = "face" + ((int)stage).ToString("00") + fileSuffix;

            faceSprite = Resources.Load<Sprite>("th17demo/face/" + enemyFolderRoot + "/" + faceFile );
        }
        else
        {
            Debug.LogWarning("Cannot parse speaker!");
            return null;
        }

        return faceSprite;
    }
}