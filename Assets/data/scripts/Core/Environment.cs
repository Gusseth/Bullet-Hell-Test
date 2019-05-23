using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The game's environment shell. Game-wide system variables and methods such as time in tick, audio events, and displaying dialogue is handled here. 
/// </summary>
public class Environment : MonoBehaviour {



// Variable Declarations

    // Technical variables //////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> The name of the executable, and the game. </summary>
    public readonly static string gameName = "Touhou 18 - Mountain of Debugging";

    /// <summary> The current version of this build. </summary>
    public readonly static string version = "v0.02b-dev";

    /// <summary> Anti-Wriggle mode. </summary>
    public readonly static bool debugMode = true;

    /// <summary> Item gravity scale value. Default value is 0.5</summary>
    public static float itemGravityScale = 0.5F;

    /// <summary> Is gameplay running? </summary>
    public static bool gameplayRunning = true;

    /// <summary> List that contains all sound effects in the game. </summary>
    public static List<AudioClip> sfx = new List<AudioClip>();

    /// <summary> List that contains all items in the game. </summary>
    public static List<GameObject> itemList = new List<GameObject>();

    /// <summary> Music volume. </summary>
    public static float bgmVolume = 1.0F;

    /// <summary> SFX volume. </summary>
    public static float sfxVolume = .5F;

    // Shortcuts for commonly used Components ///////////////////////////////////////////////////////////////////////////////////

    /// <summary> The backend Core GameObject. </summary>
    public static GameObject core;

    /// <summary> The GameManager GameObject. </summary>
    public static GameObject gameManager;

    /// <summary> The PlayerHandler Monobehaviour attatched to the player. </summary>
    public static PlayerHandler playerHandler;

    /// <summary> The player GameObject. </summary>
    public static GameObject player;

    /// <summary> The culling boundary. The DestroyOnBorder script references this to find the boundary. </summary>
    public static GameObject cullBoundary;

    /// <summary> The DestroyOnBorder script. Referenced when a GameObject that has to be culled is missing the script for redundancy. </summary>
    public static MonoBehaviour cullScript;

    /// <summary> The main camera. </summary>
    public new static GameObject camera;

    /// <summary> The Audio Listener attatched to the camera. </summary>
    public static AudioListener audioListener;

    /// <summary> The BGM Audio Source attatched to the camera. Call this audio source for playing stage/boss music. </summary>
    public static AudioSource bgmAudioSource;

    /// <summary> The SFX Audio Source attatched to the camera. Call this audio source for playing sound effects. </summary>
    public static AudioSource sfxAudioSource;

    // Game Variables ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> Ticks passed since the beginning of the game, 1 tick = 1 frame. </summary>
    public static ulong time;

    /// <summary> Locks displacement input, bombing, etc. </summary>
    public static bool lockInput = false;

    /// <summary> Locks ALL input. </summary>
    public static bool lockAllInput = false;

    /// <summary> Enumeration of all possible difficulties. </summary>
    public enum Difficulty
    {
        EasyModo, Normal, Hard, Lunatic, Extra//, Phantasm, Phantasm2
    }

// Public Static Methods and Functions

    /// <summary>
    /// Returns a vector that is clamped to the visibility of the main camera.
    /// </summary>
    /// <param name="position"> The vector to be clamped. </param>
    public static Vector2 ClampPositionToCamera(Vector2 position)
    {
        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector2 topRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        Vector2 resultVector = new Vector2(Mathf.Clamp(position.x, bottomLeft.x, topRight.x), Mathf.Clamp(position.y, bottomLeft.y, topRight.y));
        return resultVector;
    }

    /// <summary>
    /// Reloads all variable values.
    /// </summary>
    public static void ReloadAllEnvironmentVars()
    {
        camera = GameObject.Find("Main Camera");
        player = GameObject.FindGameObjectWithTag("Player");
        playerHandler = player.GetComponent<PlayerHandler>();
        audioListener = camera.GetComponent<AudioListener>();
        bgmAudioSource = camera.GetComponents<AudioSource>()[0];
        sfxAudioSource = camera.GetComponents<AudioSource>()[1];
    }

    /// <summary>
    /// Plays a desired sound effect.
    /// </summary>
    /// <param name="sound"> Sound effect to be played. </param>
    public static void PlaySound(Audio.sfx sound)
    {
        sfxAudioSource.PlayOneShot(Audio.Parse(sound),.5F);
    }

    /// <summary>
    /// Plays a desired sound effect with a modifiable volume betweem 0 and 1.
    /// </summary>
    /// <param name="sound"> Sound effect to be played. </param>
    /// <param name="volume"> Custom volume value between 0 and 1. </param>
    public static void PlaySound(Audio.sfx sound, float volume)
    {
        sfxAudioSource.PlayOneShot(Audio.Parse(sound), Mathf.Clamp01(volume));
    }

    /// <summary>
    /// Spawns an item to a desired location.
    /// </summary>
    /// <param name="item"> ItemType of desired item. </param>
    /// <param name="position"> Central spawn position. </param>
    /// <param name="spread"> Default true, set to false if the item needs to spawn at the exact position. </param>
    /// <param name="startVelocity"> The magnitude of the y-axis velocity for the item spawn. </param>
    public static void SpawnItem(Item.ItemType item, Vector2 position, bool spread = true, float startVelocity = 0)
    {
        GameObject spawn = Instantiate(itemList.Find(itemName => itemName.name == item.ToString()));

        spawn.transform.position = position;

        if (spread)
        {
            spawn.GetComponent<Rigidbody2D>().AddForce(Vector2.left * Random.Range(-50F, 50F));
        }

        if (startVelocity == 0)
        {
            spawn.GetComponent<Rigidbody2D>().velocity = new Vector2(0, Random.Range(1F, 8F));
        }
        else
        {
            spawn.GetComponent<Rigidbody2D>().velocity = new Vector2(0, startVelocity);
        }

    }

    /// <summary>
    /// Spawns an item to a desired location, but is centred around a GameObject.
    /// </summary>
    /// <param name="item"> ItemType of desired item. </param>
    /// <param name="target"> THe GameObject where the item shall spawn. </param>
    /// <param name="spread"> Default true, set to false if the item needs to spawn at the exact position. </param>
    public static void SpawnItem(Item.ItemType item, GameObject target, bool spread = true, float startVelocity = 0)
    {
        SpawnItem(item, target.transform.position, spread, startVelocity);
    }

    /// <summary>
    /// Clears all bullets in the screen.
    /// </summary>
    /// <param name="includingPlayerShots">Set to true if all shots should be cleared indiscriminately.</param>
    public static void ClearAllShots(bool includingPlayerShots = false)
    {
        if (!includingPlayerShots)
        {
            foreach (GameObject shot in GameObject.FindGameObjectsWithTag("Shot"))
            {
                if (shot.layer == LayerMask.NameToLayer("EnemyShot"))
                {
                    shot.GetComponent<ShotHandler>().OnShotNullified();
                }
            }
        }
        else
        {
            foreach (GameObject shot in GameObject.FindGameObjectsWithTag("Shot"))
            {
                shot.GetComponent<ShotHandler>().OnShotNullified();
            }
        }
    }

    /// <summary>
    /// Collects all visible items in the screen.
    /// </summary>
    public static void CollectAllItems()
    {
        foreach (GameObject items in GameObject.FindGameObjectsWithTag("Item"))
        {
            items.GetComponent<ItemHandler>().PlayerCollect();
        }
    }

    /// <summary>
    /// Returns the appropriate displacement of a shot based on the speed given.
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static Vector3 CalculateShotDisplacement(float speed)
    {
        return Vector3.up * speed * Time.deltaTime;
    }

    /// <summary>
    /// Mapping function similar to the one found in my previous game, Cold Front, and Processing.
    /// </summary>
    /// <param name="value">The value to be mapped.</param>
    /// <param name="min1">The minimum value of the starting reference.</param>
    /// <param name="max1">The maximum value of the starting reference.</param>
    /// <param name="min2">The minimum value of the new reference.</param>
    /// <param name="max2">The maximum value of the new reference.</param>
    /// <returns></returns>
    public static float Map(float value, float min1, float max1, float min2, float max2)
    {
        return (value - min1) * (max2 - min2) / (max1 - min1) + min2;
    }

// Public Static IEnumerators

    /// <summary>
    /// Adds a small delay to run the code below this line for x seconds. Use 'IEnumerator delay = Enviroment.AddDelay(x, delegate {code})' to do so.
    /// </summary>
    /// <param name="seconds">Delay added in seconds as a float.</param>
    /// <param name="method">Insert pieces of code to run after the delay has elapsed. Use 'delegate {code}'.</param>
    /// <returns></returns>
    public static IEnumerator AddDelay(float seconds, System.Action method)
    {
        yield return new WaitForSeconds(seconds);
        method.Invoke();
    }

    /// <summary>
    /// Adds a small delay to run the code below this line for x seconds. Use 'IEnumerator delay = Enviroment.AddDelay(x, delegate {code})' to do so.
    /// </summary>
    /// <param name="seconds">Delay added in seconds as an integer.</param>
    /// <param name="method">Insert lines of code to run after the delay has elapsed. Use 'delegate {code}'.</param>
    /// <returns></returns>
    public static IEnumerator AddDelay(int seconds, System.Action method)
    {
        yield return new WaitForSeconds(seconds);
        method.Invoke();
    }



// Private Backend Methods and Functions like Initialization

    /// <summary>
    /// Priority intialization, put VERY important things that need to be loaded first here.
    /// </summary>
    private static void PriorityInitialize()
    {
        // Loads important data and other crap...
    }

    // Other functions such as adding tick to the time

    private void Awake()
    {
        PriorityInitialize();
        camera = GameObject.Find("Main Camera");
        core = gameObject;
    }

    private void Start()
    {
        ReloadAllEnvironmentVars();
        GameInit.Initialize();
    }

    private void FixedUpdate()
    {
        // Updates time
        time++;
    }
}
