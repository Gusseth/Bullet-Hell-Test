using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    /// <summary> Point multiplier. Use this to anything that adds points. </summary>
    public static float pointMultiplier = 1.0F;

    /// <summary> The highest score achieved in this difficulty. </summary>
    public static ulong hiScore;

    /// <summary> The difficulty of the game. </summary>
    public static Environment.Difficulty difficulty = Environment.Difficulty.Normal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
