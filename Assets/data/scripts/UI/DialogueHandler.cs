using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using LitJson;
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
    public DialogueEvent.Speaker speaker;

    /// <summary> Is the player currently visible on the screen? </summary>
    public static bool playerDrawn = false;

    /// <summary> Is the enemy currently visible on the screen? </summary>
    public static bool enemyDrawn = false;

    /// <summary> Is dialogue currently being exchanged? </summary>
    public static bool isDialogueActive = false;

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

    /// <summary> List that contains all dialogue and dialogue events for this encounter. </summary>
    public List<Dialogue> dialogueTable;

    /// <summary> List that contains all dialogue and dialogue events for this entire stage. </summary>
    public StageDialogue stageDialogueTable;

    /// <summary> List containing raw JSON dialogue data for the entire stage. </summary>
    public List<string> stageJsonData = new List<string>();

    // Other Technical Variables
    /// <summary> The coroutine delay to cycle to the next dialogue if there are any </summary>
    Coroutine dialogueDelay;

    // Public Methods and Functions

    public void CycleDialogue()
    {
        try
        {
            Dialogue dialogue = dialogueTable[0];
            float delay = dialogue.delayInSeconds;

            if (dialogue.delayInSeconds >= 0F)
            {
                // If no delay was set, set to the default 10 second delay.
                delay = 10F;
            }

            RunDialogue(dialogue);
            dialogueTable.Remove(dialogueTable[0]);

            if (!dialogue.endDialogue && !dialogue.endStage)
            {
                // Continue running the table if the message is not supposed to end the encounter or stage.
                dialogueDelay = StartCoroutine(Environment.AddDelay(delay, delegate
                {
                    CycleDialogue();
                }));
            }
            else
            {
                // Code below is only completely run when the player runs down the delay timer
                // If the player presses Z or Left Control, the catch statements below are run.

                dialogueDelay = StartCoroutine(Environment.AddDelay(delay, delegate
                {
                    // No more dialogue remains, so end the conversation and shoot!
                    EndDialogue();

                    if (!dialogue.endStage)
                    {
                        // If the dialogue is only supposed to end the encounter, load the next encounter
                        dialogueTable = stageDialogueTable.Table[0].Table;
                        stageDialogueTable.Table.Remove(stageDialogueTable.Table[0]);
                    }
                    else
                    {
                        // If the dialogue DOES end the stage, trigger the end stage event.
                        GameManager.EndStage();
                    }
                }));
            }

            //Invoke("CycleDialogue", delay);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            try
            {
                // Safeguard measure in case I forget to turn a dialogue event to end the conversation to 'true'.
                // This section of code is only run when the dialogueTable list is empty because it errors out, triggering this.

                // No more dialogue remains, so end the conversation and shoot!
                EndDialogue();

                // Replaces the empty dialogue table with the next encounter.
                dialogueTable = stageDialogueTable.Table[0].Table;
                stageDialogueTable.Table.Remove(stageDialogueTable.Table[0]);

                //Debug.LogWarning("Dialogue parsed was the last message in the encounter, but it doesn't have endDialogue set to true!");
                return;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                // Safeguard measure in case I forget to turn a dialogue event to end the conversation to 'true'.
                // This section of code is only run when the stageDialogueTable list is empty because it errors out, triggering this.
                
                // No more dialogue remains, so end the conversation and shoot!
                EndDialogue();

                // Tells the GameManager component to end the stage.
                GameManager.EndStage();

                //Debug.LogWarning("Dialogue parsed was the last message in the stage, but it doesn't have endStage set to true!");
                return;
            }
        }
    }

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
        if (dialogue.speaker == DialogueEvent.Speaker.player)
        {
            // Player as speaker

            subjectFaceImage = playerFaceImage;
            subjectFaceImage.overrideSprite = Dialogue.ParseFace(dialogue.expression, dialogue.speaker);
            subjectBody = playerBody;
            subjectText = playerText;


            // Animations played below if the speaker before is not the speaker now with the two sprites already loaded. This one slides the player in, enemy out.
            if (speaker != dialogue.speaker && speaker != DialogueEvent.Speaker.none)
            {
                speaker = dialogue.speaker;
                enemyBody.GetComponent<Animator>().Play("Slide Out");
                enemyBalloon.GetComponent<Animator>().Play("Slide Out");

                // Sliding the player in
                playerBody.GetComponent<Animator>().Play("Slide In");
                playerBalloon.GetComponent<Animator>().Play("Slide In");
            }

            // Otherwise, if there was no one speaking or the player is currently not drawn on the screen.
            if (speaker == DialogueEvent.Speaker.none || !playerDrawn)
            {
                playerDrawn = true;
                speaker = dialogue.speaker;
                playerBody.GetComponent<Animator>().SetBool("disappear", false);
                playerBody.GetComponent<Animator>().Play("Appear");
                playerBalloon.GetComponent<Animator>().Play("Slide In");
            }

        }
        else if (dialogue.speaker == DialogueEvent.Speaker.enemy)
        {
            // Enemy as speaker

            subjectFaceImage = enemyFaceImage;
            subjectFaceImage.overrideSprite = Dialogue.ParseFace(dialogue.expression, dialogue.speaker);
            subjectBody = enemyBody;
            subjectText = enemyText;


            // Animations played below if the speaker before is not the speaker now with the two sprites already loaded. This one slides the enemt in, player out.
            if (speaker != dialogue.speaker && speaker != DialogueEvent.Speaker.none)
            {
                speaker = dialogue.speaker;
                enemyBody.GetComponent<Animator>().Play("Slide In");
                enemyBalloon.GetComponent<Animator>().Play("Slide In");

                // Sliding the player out
                playerBody.GetComponent<Animator>().Play("Slide Out");
                playerBalloon.GetComponent<Animator>().Play("Slide Out");
            }

            // Otherwise, if there was no one speaking or the enemy is currently not drawn on the screen.
            if (speaker == DialogueEvent.Speaker.none || !enemyDrawn)
            {
                enemyDrawn = true;
                speaker = dialogue.speaker;
                enemyBody.GetComponent<Animator>().SetBool("disappear", false);
                enemyBody.GetComponent<Animator>().Play("Appear");
                enemyBalloon.GetComponent<Animator>().Play("Slide In");
            }
        }

        if (dialogue.playBGM)
        {
            // TEMP CODE ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Environment.PlayBGM(Audio.bgm.stg01b);
        }

        subjectText.text = dialogue.message;

        if (dialogue.action != null)
        {
            // If there is any code attatched to the dialogue, run it.
            dialogue.action.Invoke();
        }
    }

    /// <summary>
    /// Loads the first DialogueEvent in the dialogue table.
    /// </summary>
    public static void StartDialogue()
    {
        Environment.dialogueHandler.CycleDialogue();
        isDialogueActive = true;
        PlayerHandler.canShoot = false;
        PlayerHandler.isInvincible = true;

        if (GameManager.bossDefeated)
        {
            int stage = (int)GameManager.GetStage();
            Environment.dialogueHandler.enemyBody.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("th17demo/face/" + "enemy" + stage + "/" + "face" + (stage).ToString("00") + "bsl");
        }
    }

    /// <summary>
    /// Ends ALL dialogue, both speakers disappear.
    /// </summary>
    public void EndDialogue()
    {
        speaker = DialogueEvent.Speaker.none;
        playerDrawn = false;
        enemyDrawn = false;
        isDialogueActive = false;
        enemyBody.GetComponent<Animator>().SetBool("disappear", true);
        playerBody.GetComponent<Animator>().SetBool("disappear", true);
        playerBalloon.GetComponent<Animator>().Play("Slide Out");
        enemyBalloon.GetComponent<Animator>().Play("Slide Out");

        if (!GameManager.bossDefeated)
        {
            Environment.PlaySound(Audio.sfx.chargeUp0, Audio.sfxNormalPriority * Environment.sfxMasterVolume);
            StartCoroutine(Environment.AddDelay(2, delegate
            {
                GameManager.BossAttack();
                Environment.PlaySound(Audio.sfx.enemyDeath, Audio.sfxTopPriority * Environment.sfxMasterVolume);
            }));
        }
        /*else
        {
            Environment.PlaySound(Audio.sfx.cardClear, Audio.sfxTopPriority * Environment.sfxMasterVolume);
        }*/

        PlayerHandler.canShoot = true;
        PlayerHandler.isInvincible = false;
    }

    /// <summary>
    /// Test method for experimenting with JSON.
    /// </summary>
    public void LoadDialogueFromJson()
    {
        List<DialogueEvent> diagTable = new List<DialogueEvent>();

        string jsonFile = "stg" + ((int)GameManager.GetStage()).ToString("00") + ".json";
        string jsonPath = Path.Combine(Environment.dialogueDataPath, jsonFile);
        /*JsonData jsonStageData = JsonMapper.ToObject(File.ReadAllText(jsonPath));

        foreach (JsonData jsonStageData in JsonMapper.ToObject<JsonData>(File.ReadAllText(jsonPath)))
        {
            Debug.Log(jsonStageData);
        }*/
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

        // Not yet fully implemented, supposed to read .json data and place them into the stageDialogueTable list of stage-wide dialogue data
        LoadDialogueFromJson();

        // Moving the first dialogue data into the runtime dialogue table.
        dialogueTable = stageDialogueTable.Table[0].Table;
        stageDialogueTable.Table.Remove(stageDialogueTable.Table[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Z) && isDialogueActive) || (Input.GetKey(KeyCode.LeftControl) && isDialogueActive))
        {
            if (dialogueDelay != null)
            {
                StopCoroutine(dialogueDelay);
            }
            CycleDialogue();
        }
    }
}

/// <summary>
/// Dialogue object for all dialogue-related things.
/// </summary>
public class DialogueEvent
{
// Variable declarations

    /// <summary> The speaker of this message. </summary>
    public Speaker speaker;

    /// <summary> The type of event of this object. Defaults to event unless it is a dialogue. </summary>
    public DialogueEventType eventType = DialogueEventType.dialogueEvent;

    /// <summary> Event ends the dialogue after execution. </summary>
    public bool endDialogue;

    /// <summary> Event ends the stage after execution. </summary>
    public bool endStage;

    /// <summary> Event plays the music for the boss. </summary>
    public bool playBGM;

    /// <summary> Suspends the dialogue routine by x seconds. </summary>
    public float delayInSeconds;

    /// <summary> The code that is executed by this event. </summary>
    public System.Action action;

    /// <summary> Enumeration of all valid speakers. </summary>
    public enum Speaker
    {
        none, player, enemy
    }

    /// <summary> Enumeration of all valid dialogue types. </summary>
    public enum DialogueEventType
    {
        dialogueEvent, dialogue
    }

 // Structs

    // Class struct #1 - Injecting code + Assigning speaker
    /// <summary>
    /// Constructs a dialogue event with a speaker and an action.
    /// </summary>
    /// <param name="speaker">The speaker susceptible to this action</param>
    /// <param name="action">The code to be run</param>
    /// <param name="endDialogue">Set to true if the dialogue should end afterwards</param>
    /// <param name="delayInSeconds">Set a delay after this event.</param>
    public DialogueEvent(Speaker speaker, System.Action action, bool endDialogue = false, float delayInSeconds = 0)
    {
        this.speaker = speaker;
        this.action = action;
        this.endDialogue = endDialogue;
        this.delayInSeconds = delayInSeconds;
    }

    // Class struct #2 - Just code
    /// <summary>
    /// Constructs a dialogue event with only an action.
    /// </summary>
    /// <param name="action">The code to be run</param>
    public DialogueEvent(System.Action action)
    {
        this.action = action;
    }

    // Class struct #3 - Ending dialogue
    /// <summary>
    /// Constructs a dialogue event that only ends all dialogue.
    /// </summary>
    /// <param name="endDialogue">Set to true if the dialogue should end afterwards</param>
    public DialogueEvent(bool endDialogue)
    {
        this.endDialogue = true;
    }

    // Class struct #4 - Delay
    /// <summary>
    /// Constructs a dialogue event that only adds delay.
    /// </summary>
    /// <param name="delayInSeconds">Set a delay after this event.</param>
    public DialogueEvent(float delayInSeconds)
    {
        this.delayInSeconds = delayInSeconds;
    }

    // Class struct #5 - Empty struct
    /// <summary>
    /// COnstructs an empty event that is mostly used as a dummy for dialogue.
    /// </summary>
    public DialogueEvent()
    {
    }
}



/// <summary>
/// Dialogue text object. This is the graphical text displayed.
/// </summary>
[System.Serializable]
public class Dialogue : DialogueEvent
{

 // Variable Declarations

    /// <summary> The speaker's facial expression. </summary>
    public Face expression;

    /// <summary> The message conveyed. </summary>
    public string message;

    /// <summary> Set to true if the speaker's sprite should disappear from the screen after this message. </summary>
    public bool speakerDisappears;

    /// <summary> Enumeration of all valid facial expressions. </summary>
    public enum Face
    {
        normal, angry, challenge, confused, happy, nervous, smug, surprised, lose
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
        eventType = DialogueEventType.dialogue;
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
            case Face.lose:
                fileSuffix = "lo";
                break;
            default:
                fileSuffix = "no";
                Debug.LogWarning("Invalid Face '" + face.ToString() + "'parsed!");
                break;
        }

        Sprite faceSprite = null; // Sprite to be returned later, default empty just in case of errors

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

[System.Serializable]
public class EncounterDialogue : List<Dialogue>
{
    public List<Dialogue> Table;
}

[System.Serializable]
public class StageDialogue : List<EncounterDialogue>
{
    public List<EncounterDialogue> Table;
}