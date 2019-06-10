using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;

/// <summary>
/// The game's environment shell. Game-wide system variables and methods such as time in tick, audio events, and displaying dialogue is handled here. 
/// </summary>
public class Environment : MonoBehaviour {



// Variable Declarations

    // Technical variables //////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> The name of the executable, and the game. </summary>
    public readonly static string gameName = "Touhou 18 - Mountain of Debugging";

    /// <summary> The current version of this build. </summary>
    public readonly static string version = "v0.02d";

    /// <summary> Anti-Wriggle mode. </summary>
    public static bool debugMode = false;

    /// <summary> Item gravity scale value. Default value is 0.5</summary>
    public static float itemGravityScale = 0.5F;

    /// <summary> Is gameplay running? </summary>
    public static bool gameplayRunning = true;

    /// <summary> List that contains all sound effects in the game. </summary>
    public static List<AudioClip> sfx = new List<AudioClip>();

    /// <summary> List that contains all items in the game. </summary>
    public static List<GameObject> itemList = new List<GameObject>();

    /// <summary> Music base volume. </summary>
    public static float bgmMasterVolume = .6F;

    public int bgmOffset;

    public int offset2;

    /// <summary> SFX base volume. </summary>
    public static float sfxMasterVolume = .5F;

    /// <summary> Returns true if the music should be looped. </summary>
    public static bool repeatBGM = true;

    // Shortcuts for commonly used Components ///////////////////////////////////////////////////////////////////////////////////

    /// <summary> The backend Core GameObject. </summary>
    public static GameObject core;

    /// <summary> The background container of the stage GameObject. </summary>
    public static GameObject background;

    /// <summary> The GameManager Monobehaviour. </summary>
    public static GameManager gameManager;

    /// <summary> The PlayerHandler Monobehaviour attatched to the player. </summary>
    public static PlayerHandler playerHandler;

    /// <summary> The DialogueHandler Monobehaviour. </summary>
    public static DialogueHandler dialogueHandler;

    /// <summary> The GameUIHandler Monobehaviour. </summary>
    public static GameUIHandler gameUIHandler;

    /// <summary> The player GameObject. </summary>
    public static GameObject player;

    /// <summary> The culling boundary. The DestroyOnBorder script references this to find the boundary. </summary>
    public static GameObject cullBoundary;

    /// <summary> The DestroyOnBorder script. Referenced when a GameObject that has to be culled is missing the script for redundancy. </summary>
    public static MonoBehaviour cullScript;

    /// <summary> The main camera. </summary>
    public new static GameObject camera;

    /// <summary> The main canvas. </summary>
    public static GameObject mainCanvas;

    /// <summary> The camera viewport canvas. </summary>
    public static GameObject viewportCanvas;

    /// <summary> The Audio Listener attatched to the camera. </summary>
    public static AudioListener audioListener;

    /// <summary> The BGM Audio Source attatched to the camera. Call this audio source for playing stage/boss music. </summary>
    public static AudioSource bgmAudioSource;

    /// <summary> The SFX Audio Source attatched to the camera. Call this audio source for playing sound effects. </summary>
    public static AudioSource sfxAudioSource;

    /// <summary> The path to the Dialogue folder in StreamingAssets. </summary>
    public static readonly string dialogueDataPath = Path.Combine(Application.streamingAssetsPath, "Dialogue");

    // Game Variables ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> Ticks passed since the start of the executable, 1 tick = 1 frame. </summary>
    public static ulong time;

    /// <summary> Ticks passed since the start of the current game session, 1 tick = 1 frame. </summary>
    public static ulong gameplayTime;

    /// <summary> True if the game is paused. </summary>
    public static bool isPaused;

    /// <summary> Locks displacement input, bombing, etc. </summary>
    public static bool lockInput = false;

    /// <summary> Locks ALL input. </summary>
    public static bool lockAllInput = false;

    /// <summary> Enumeration of all possible difficulties. </summary>
    public enum Difficulty
    {
        EasyModo, Normal, Hard, Lunatic, Extra//, Phantasm, Phantasm2
    }

    /// <summary> Enumeration of all possible stages. </summary>
    public enum Stage
    {
        One = 1, Two, Three, Four, Five, Six, Extra
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
    /// Returns a Color value from the inputted 8-bit integer values.
    /// </summary>
    /// <param name="r">Red value between 0-255</param>
    /// <param name="g">Green value between 0-255</param>
    /// <param name="b">Blue value between 0-255</param>
    /// <param name="a">Alpha or Transparency value between 0-255</param>
    /// <returns></returns>
    public static Color ColorByte(byte r, byte g, byte b, byte a)
    {
        return new Color(r / 255F, g / 255F, b / 255F, a / 255F);
    }

    /// <summary>
    /// Returns a Color value from the inputted 8-bit integer values.
    /// </summary>
    /// <param name="r">Red value between 0-255</param>
    /// <param name="g">Green value between 0-255</param>
    /// <param name="b">Blue value between 0-255</param>
    /// <returns></returns>
    public static Color ColorByte(byte r, byte g, byte b)
    {
        return ColorByte(r, g, b, 255);
    }

    /// <summary>
    /// Returns a child GameObject with the given name. 
    /// </summary>
    /// <param name="name">The name of the child to be found</param>
    /// <param name="parent">The parent of the children</param>
    /// <returns>GameObject Child</returns>
    public static GameObject FindChild(string name, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.parent.name == name)
            {
                return child.gameObject;
            }
        }
        Debug.LogError("Cannot find " + name + " as a child of " + parent.name + "!");
        return null;
    }

    /// <summary>
    /// Returns a child GameObject with the given name. 
    /// </summary>
    /// <param name="name">The name of the child to be found</param>
    /// <param name="parent">The parent of the children</param>
    /// <returns>GameObject Child</returns>
    public static GameObject FindChild(string name, GameObject parent)
    {
        return FindChild(name, parent.transform);
    }

    /// <summary>
    /// Reloads all variable values.
    /// </summary>
    public static void ReloadAllEnvironmentVars()
    {
        camera = GameObject.Find("Main Camera");
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = core.GetComponent<GameManager>();
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
    /// Plays the soundtrack given with the appropriate loop times.
    /// </summary>
    /// <param name="bgm">The enumeration of the desired soundtrack.</param>
    public static void PlayBGM(Audio.bgm bgm)
    {
        bgmAudioSource.Stop();

        repeatBGM = true;

        switch (bgm)
        {
            case Audio.bgm.score:
                bgmAudioSource.clip = Resources.Load<AudioClip>("bgm/Player's Score");
                core.GetComponent<Environment>().bgmOffset = 759800;
                core.GetComponent<Environment>().offset2 = 1975857;
                bgmAudioSource.PlayScheduled(0);
                break;
            default:
                // TEMP HARDCODE BELOW -- YOU HAVE BEEN WARNED -- PREPARE YOUR EYES ////////////////////////////////////////////////////////////////////////////////////////////////
                bgmAudioSource.clip = Resources.Load<AudioClip>("bgm/Jelly Stone");
                bgmAudioSource.PlayScheduled(0);
                break;
        }
    }

    /// <summary>
    /// Plays a soundtrack based on the given file name and not looped.
    /// </summary>
    /// <param name="filename"></param>
    public static void PlayBGM(string filename)
    {
        bgmAudioSource.Stop();
        repeatBGM = false;
        bgmAudioSource.clip = Resources.Load<AudioClip>("bgm/" + filename);
        bgmAudioSource.PlayScheduled(0);
    }

    /// <summary>
    /// Pauses the game music. Need I say more?
    /// </summary>
    public static void PauseBGM()
    {
        if (!isPaused)
        {
            bgmAudioSource.UnPause();
        }
        else
        {
            bgmAudioSource.Pause();
        }
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
    /// Displays the inputted dialogue object. 
    /// </summary>
    /// <param name="dialogue">The dialogue to be displayed</param>
    public static void RunDialogue(Dialogue dialogue)
    {
        dialogueHandler.RunDialogue(dialogue);
    }

    /// <summary>
    /// Ends ALL dialogue, both speakers disappear.
    /// </summary>
    public static void EndDialogue()
    {
        dialogueHandler.EndDialogue();
    }

    /// <summary>
    /// Clears all bullets in the screen.
    /// </summary>
    /// <param name="includingPlayerShots">Set to true if all shots should be cleared indiscriminately.</param>
    /// <param name="addScore">Set to true for each bullet cleared adds 100 points to score.</param>
    public static void ClearAllShots(bool includingPlayerShots = false, bool addScore = false)
    {
        if (!includingPlayerShots)
        {
            foreach (GameObject shot in GameObject.FindGameObjectsWithTag("Shot"))
            {
                if (shot.layer == LayerMask.NameToLayer("EnemyShot"))
                {
                    shot.GetComponent<ShotHandler>().OnShotNullified();
                    if (addScore)
                    {
                        playerHandler.AddScore(100);
                    }
                }
            }
        }
        else
        {
            foreach (GameObject shot in GameObject.FindGameObjectsWithTag("Shot"))
            {
                shot.GetComponent<ShotHandler>().OnShotNullified();
                if (addScore)
                {
                    playerHandler.AddScore(100);
                }
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

    // JSON-related things
    /// <summary>
    /// Returns a properly formatted JSON array for top-level JSON arrays.
    /// </summary>
    /// <typeparam name="T">Type of object to be returned.</typeparam>
    /// <param name="jsonData">Path of the JSON file here.</param>
    /// <returns></returns>
    public static T[] JsonToArray<T>(string jsonData)
    {
        string newJson = "{ \"array\": " + jsonData + "}";
        Debug.Log(newJson);
        JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(newJson);
        return wrapper.array;
    }

    /// <summary>
    /// Wrapper class used to return an array of a type used by the function above.
    /// </summary>
    /// <typeparam name="T">Type of object to be returned.</typeparam>
    [System.Serializable]
    private class JsonWrapper<T>
    {
        public T[] array;
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

    /// <summary>
    /// Adds a small delay to run the code below this line for x seconds in real time, unaffected from Time.timeScale. Use 'IEnumerator delay = Enviroment.AddDelay(x, delegate {code})' to do so.
    /// </summary>
    /// <param name="seconds">Delay added in seconds as a float.</param>
    /// <param name="method">Insert pieces of code to run after the delay has elapsed. Use 'delegate {code}'.</param>
    /// <returns></returns>
    public static IEnumerator AddDelayRealtime(float seconds, System.Action method)
    {
        yield return new WaitForSecondsRealtime(seconds);
        method.Invoke();
    }

    /// <summary>
    /// Adds a small delay to run the code below this line for x seconds, unaffected from Time.timeScale. Use 'IEnumerator delay = Enviroment.AddDelay(x, delegate {code})' to do so.
    /// </summary>
    /// <param name="seconds">Delay added in seconds as an integer.</param>
    /// <param name="method">Insert lines of code to run after the delay has elapsed. Use 'delegate {code}'.</param>
    /// <returns></returns>
    public static IEnumerator AddDelayRealtime(int seconds, System.Action method)
    {
        yield return new WaitForSecondsRealtime(seconds);
        method.Invoke();
    }



// Private Backend Methods and Functions like Initialization

    /// <summary>
    /// Priority intialization, put VERY important things that need to be loaded first here.
    /// </summary>
    private static void PriorityInitialize()
    {
        // Loads important data and other crap...
        Time.timeScale = 1;
    }

    // Other functions such as adding tick to the time

    void Awake()
    {
        PriorityInitialize();
        core = gameObject;
    }

    void Start()
    {
        mainCanvas = GameObject.Find("Main Canvas");
        ReloadAllEnvironmentVars();
        GameInit.Initialize();
    }

    void FixedUpdate()
    {
        // Updates time
        time++;
    }

    void Update()
    {
        // Temporary code////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (bgmAudioSource.isPlaying && bgmAudioSource.timeSamples >= offset2 && repeatBGM)
        {
            // Loops audio
            Debug.Log("Looping Audio...");
            bgmAudioSource.timeSamples = bgmOffset + (bgmAudioSource.timeSamples - offset2);
            Debug.Log(bgmAudioSource.timeSamples);
        }
        if (Input.GetKeyDown(KeyCode.R) && (GameManager.gameOver || GameManager.bossDefeated || isPaused))
        {
            // Resets the scene
            lockInput = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        if (Input.GetKeyUp(KeyCode.Q) && (GameManager.gameOver || GameManager.bossDefeated || isPaused))
        {
            Application.Quit();
        }
    }
}
