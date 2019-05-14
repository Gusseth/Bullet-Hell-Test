using UnityEngine;
using System.Collections;

/// <summary>
/// This script is run just before gameplay starts. Use EnvironmentPostInit for anything that has to load at the absolute start of the executale.
/// </summary>
public class GameInit : MonoBehaviour
{
    /// <summary>
    /// The sole function in this script. Runs through the initialize protocols.
    /// </summary>
    public static void Initialize()
    {
        // Private variables first
        GameObject cullingBorder = GameObject.Find("Cull Boundary");
        PlayerHandler playerHandler = Environment.playerHandler;

        // Defines game variables that are to be assigned at this period
        Environment.cullBoundary = cullingBorder;

        // Reset player statistics
        playerHandler.score = 0;
        playerHandler.points = 0;
        playerHandler.power = 0;
        playerHandler.graze = 0;

        // Defines the size of the culling border
        cullingBorder.GetComponent<BoxCollider2D>().size = cullingBorder.GetComponent<BoxCollider2D>().size * Environment.camera.GetComponent<Camera>().orthographicSize;

        // Makes gameplay runtime true once initialization is complete
        Environment.gameplayRunning = true;
    }
}
