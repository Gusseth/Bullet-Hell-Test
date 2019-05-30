using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Variable Declarations

    /// <summary> Point multiplier. Use this to anything that adds points. </summary>
    public static float pointMultiplier = 1.0F;

    /// <summary> The highest score achieved in this difficulty. </summary>
    public static ulong hiScore;

    /// <summary> The difficulty of the game. </summary>
    public static Environment.Difficulty difficulty = Environment.Difficulty.Normal;

    // Public Functions and Methods

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause()
    {
        if (Environment.isPaused)
        {
            Time.timeScale = 1;
            Environment.isPaused = false;
            return;
        }
        else
        {
            Time.timeScale = 0;
            Environment.isPaused = true;
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
        }
        else
        {
            Time.timeScale = 1;
        }
        Environment.isPaused = pause;
    }


    /// <summary>
    /// Pauses the game when the window is unfocused.
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        Pause(pause);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

}
