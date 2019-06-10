using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Monobehaviour that manages UI anchored to the viewport camera.
/// </summary>
public class GameUIHandler : MonoBehaviour
{
 // Variable Declarations

    /// <summary> The GameObject that contains the Item Get Line UI. </summary>
    public GameObject itemGetLine;

    /// <summary> The GameObject that contains the Extend UI. </summary>
    public GameObject extendUI;

    /// <summary> The GameObject that contains the Stage Clear UI. </summary>
    public GameObject stageClearUI;

    /// <summary> The GameObject that contains the fade to black mask. </summary>
    public GameObject fade;

    /// <summary> The GameObject that contains the components for the spellcard banner. </summary>
    public GameObject spellcardBanner;

// Public static functions and methods

    /// <summary>
    /// Plays and shows the Item Get Line's animation.
    /// </summary>
    public static void PlayItemGetLine()
    {
        Environment.gameUIHandler.itemGetLine.GetComponent<Animator>().Play("Execute");
    }

    /// <summary>
    /// Plays and shows the Extend UI animation.
    /// </summary>
    public static void PlayExtendUI()
    {
        Environment.gameUIHandler.extendUI.GetComponent<Animator>().Play("Execute");
    }

    /// <summary>
    /// Plays the animations for stage transitions.
    /// </summary>
    public static void PlayStageTransition()
    {
        Environment.gameUIHandler.fade.GetComponent<Animator>().Play("Stage Clear");
    }

    /// <summary>
    /// Plays the animations for the game over screen.
    /// </summary>
    public static void PlayGameOver()
    {
        Environment.gameUIHandler.fade.GetComponent<Animator>().Play("Game Over");
    }

    /// <summary>
    /// Plays the appropriate animations of a spell card attack.
    /// </summary>
    /// <param name="init">Set to true if the starting animation should be played</param>
    /// <param name="spellcardName">Name of the spellcard</param>
    public static void PlayBossSpellcard(bool init, string spellcardName = null)
    {
        if (init)
        {
            Environment.gameUIHandler.spellcardBanner.transform.Find("Banner Graphic").Find("Text").GetComponent<TextMeshProUGUI>().text = spellcardName;
            Environment.gameUIHandler.spellcardBanner.GetComponent<Animator>().Play("Appear");
            Environment.background.GetComponent<Animator>().Play("spellcardAppear");
        }
        else
        {
            Environment.gameUIHandler.spellcardBanner.GetComponent<Animator>().Play("Disappear");
            Environment.background.GetComponent<Animator>().Play("spellcardDisappear");
        }
    }
}
