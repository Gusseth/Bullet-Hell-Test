using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script is run just before gameplay starts. Use EnvironmentPostInit for anything that has to load at the absolute start of the executale.
/// </summary>
public class GameInit : MonoBehaviour
{
    /// <summary>
    /// The sole function in this script. Runs through the initialize protocols.
    /// </summary>

    // Temporary Public Variables

    public static void Initialize()
    {
        // Private variables first
        DialogueHandler dialogueHandler = GameObject.Find("Dialogue").GetComponent<DialogueHandler>();
        GameObject cullingBorder = GameObject.Find("Cull Boundary");
        PlayerHandler playerHandler = Environment.playerHandler;

        // Defines game variables that are to be assigned at this period
        Environment.cullBoundary = cullingBorder;
        Environment.gameManager = Environment.core.GetComponent<GameManager>();
        Environment.background = GameObject.Find("Stage Background");
        Environment.camera = GameObject.Find("Main Camera");
        Environment.viewportCanvas = GameObject.Find("Window Canvas");
        Environment.gameUIHandler = Environment.viewportCanvas.GetComponent<GameUIHandler>();

        // Dialogue
        Environment.dialogueHandler = dialogueHandler;
        dialogueHandler.speechContainer = GameObject.Find("Speech Container");
        dialogueHandler.playerBody = GameObject.Find("Player Body");
        dialogueHandler.playerBalloon = GameObject.Find("Player Balloon");

        dialogueHandler.enemyBody = GameObject.Find("Enemy Body");
        dialogueHandler.enemyBalloon = GameObject.Find("Enemy Balloon");

        dialogueHandler.bossTitleCard = GameObject.Find("Title Card");

        // Temporary Hardcode lmao ecks dee ////////////////////////////////////////////////////////////////////////////////
        Environment.dialogueHandler.stageDialogueTable.Table[0].Table[27].action = delegate { GameUIHandler.PlayItemGetLine(); };

        // Music
        Environment.bgmAudioSource.volume = Environment.bgmMasterVolume;

        // Reset player statistics
        PlayerHandler.isRespawning = false;
        playerHandler.score = 0;
        playerHandler.points = 0;
        playerHandler.power = 0;
        playerHandler.graze = 0;

        Environment.lockAllInput = false;
        Environment.lockInput = false;
        Environment.isPaused = false;
        GameManager.gameOver = false;
        GameManager.bossDefeated = false;

        // Defines the size of the culling border
        cullingBorder.GetComponent<BoxCollider2D>().size = cullingBorder.GetComponent<BoxCollider2D>().size * Environment.camera.GetComponent<Camera>().orthographicSize;

        // Makes gameplay runtime true once initialization is complete
        Environment.gameplayTime = 0;
        Environment.gameplayRunning = true;
    }
}
