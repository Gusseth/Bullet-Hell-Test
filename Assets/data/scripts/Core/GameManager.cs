using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

// Variable Declarations

    /// <summary> Point multiplier. Use this to anything that adds points. </summary>
    public static float pointMultiplier = 1.0F;

    /// <summary> The highest score achieved in this difficulty. </summary>
    public static ulong hiScore = 0;

    /// <summary> Is the boss defeated? </summary>
    public static bool bossDefeated = false;

    /// <summary> Returns true if the player ran out of lives. </summary>
    public static bool gameOver = false;

    /// <summary> The difficulty of the game. </summary>
    public Environment.Difficulty difficulty;

    /// <summary> The difficulty of the game. </summary>
    public Environment.Stage stage;

    // Public Functions and Methods

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause()
    {
        if (Environment.isPaused)
        {
            // Unpausing game...
            Time.timeScale = 1;
            Environment.isPaused = false;
            Environment.PlaySound(Audio.sfx.cancel, Audio.sfxTopPriority * Environment.sfxMasterVolume);
            Environment.PauseBGM();
            return;
        }
        else
        {
            // Pausing
            Time.timeScale = 0;
            Environment.isPaused = true;
            Environment.PauseBGM();
            Environment.PlaySound(Audio.sfx.pause, Audio.sfxTopPriority * Environment.sfxMasterVolume);
            return;
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            Environment.PlaySound(Audio.sfx.pause, Audio.sfxTopPriority * Environment.sfxMasterVolume);
        }
        else
        {
            Time.timeScale = 1;
        }
        Environment.isPaused = pause;
    }

    public void LoadStageDialogue()
    {

    }

    /// <summary>
    /// Ends the stage.
    /// </summary>
    public void EndStageMono()
    {
        Environment.PlaySound(Audio.sfx.cardClear, Audio.sfxTopPriority * Environment.sfxMasterVolume);
        GameUIHandler.PlayStageTransition();
        Environment.lockInput = true;
        StartCoroutine(Audio.FadeOut(Environment.bgmAudioSource, 3));

        // Audio related things during the end scene
        int i = Random.Range(0, 5);
        switch (i)
        {
            case 0:
                StartCoroutine(Environment.AddDelay(3, delegate { Environment.PlayBGM("The Lost Emotion"); }));
                break;
            case 1:
                StartCoroutine(Environment.AddDelay(3, delegate { Environment.PlayBGM("Shinkirou Orchestra"); }));
                break;
            case 2:
                StartCoroutine(Environment.AddDelay(3, delegate { Environment.PlayBGM("Tomorrow will be Special; Yesterday Was Not"); }));
                break;
            case 3:
                StartCoroutine(Environment.AddDelay(3, delegate { Environment.PlayBGM("The Space Shrine Maiden Appears"); }));
                break;
            case 4:
                StartCoroutine(Environment.AddDelay(3, delegate { Environment.PlayBGM("Old Adam Bar"); }));
                break;
            default:
                StartCoroutine(Environment.AddDelay(3, delegate { Environment.PlayBGM("The Lost Emotion"); }));
                break;
        }
    }

    /// <summary>
    /// Triggers the Game Over response.
    /// </summary>
    public void GameOverMono()
    {
        Environment.lockInput = true;
        gameOver = true;
        Environment.PlayBGM(Audio.bgm.score);
        GameUIHandler.PlayGameOver();
        Time.timeScale = 0;
    }

    // Public Static Methods and Functions

    /// <summary>
    /// Returns the current difficulty of the game.
    /// </summary>
    /// <returns>Environment.Difficulty</returns>
    public static Environment.Difficulty GetDifficulty()
    {
        return Environment.gameManager.difficulty;
    }

    /// <summary>
    /// Returns the current stage of the game.
    /// </summary>
    /// <returns>Environment.Stage</returns>
    public static Environment.Stage GetStage()
    {
        return Environment.gameManager.stage;
    }

    /// <summary>
    /// Makes the boss attack.
    /// </summary>
    public static void BossAttack()
    {
        GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHandler>().TriggerAttack();
    }

    /// <summary>
    /// Ends the stage.
    /// </summary>
    public static void EndStage()
    {
        Environment.gameManager.EndStageMono();
    }

    public static void GameOver()
    {
        Environment.gameManager.GameOverMono();
    }

 // Private Functions and Methods

    /// <summary>
    /// Pauses the game when the window is unfocused.
    /// </summary>
    /// <param name="pause"></param>
    void OnApplicationPause(bool pause)
    {
        if (!gameOver)
        {
            Pause(pause);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Temp Code below /////////////////////////////////////////////////////////////////////////////////////////////////////
        StartCoroutine(Environment.AddDelay(3, delegate
        {
            DialogueHandler.StartDialogue();
        }));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            Pause();
            return;
        }
    }

    void FixedUpdate()
    {
        if (!Environment.isPaused)
        {
            Environment.gameplayTime++;
        }

        if (PlayerHandler.isRespawning || PlayerHandler.isBombing)
        {
            // Prevents the enemy from spamming bullets when the player is not alive.
            Environment.ClearAllShots();
        }

        if (PlayerHandler.isBombing)
        {
            Environment.CollectAllItems();
        }
    }

}
