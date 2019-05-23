using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages the player and their data. 
/// </summary>
public class PlayerHandler : MonoBehaviour {

// Variable Declaration

    // Gameplay Variables //////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> The number of lives the player has. This is the value that is modified during runtime. </summary>
    public int lives;

    /// <summary> The number of bombs the player currently possesses. This is the value that is modified during runtime. </summary>
    public int bombs;

    /// <summary> The number of power as a float (0 to 4). </summary>
    public float power;

    /// <summary> The number of blue point items. 1000 = 1. </summary>
    public int points;

    /// <summary> Number of bullets that grazed the player. </summary>
    public int graze;

    /// <summary> Player's current score as a ulong. Cast as int if need be. </summary>
    public ulong score;

    // Technical Variables //////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> The Player class for the character the player is playing as. </summary>
    public Player player;

    /// <summary> Player's raw speed. By default, this is the player's speed when not focussing. This is the value that is modified during runtime. </summary>
    public float rawSpeed;

    /// <summary> Player's hitbox size. This is the value that is modified during runtime. </summary>
    public float hitboxRadius;

    /// <summary> The character the player is playing as. </summary>
    public Player.Character character;

    /// <summary> The shot variant the player has equipped. </summary>
    public Player.CharacterShot characterShot;

    /// <summary> Backend variable. Only true when the player is automatically moving as a result of respawning. </summary>
    private bool playerRespawnTranslation = false;

    /// <summary> Backend variable. Linear interpolation delta time. DO not touch this, please. </summary>
    private float lerpTime = 0;

    // Conditional Variables ////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> Returns true if the player is alive. False if not. </summary>
    public bool isAlive = true;

    /// <summary> Returns true if the player is focussing. False if not. </summary>
    public bool isFocused;

    /// <summary> Returns true if the bomb is still in effect. </summary>
    public bool isBombing;

    /// <summary> Returns true if the player can bomb. This is really only used for deathbombing. </summary>
    public bool canBomb = true;

    /// <summary> Returns true if the player is invincible. </summary>
    public bool isInvincible;


// Initialization

    void Awake ()
    {
	    if (player == null)
        {
            // Here for debug purposes and early development when the player class isn't defined.
            player = new Player();
        }

        lives = player.lives;
        bombs = player.bombs;
        rawSpeed = player.rawSpeed;
        character = player.character;
        characterShot = player.characterShot;
        hitboxRadius = player.hitboxRadius;
        GetComponent<CircleCollider2D>().radius = hitboxRadius;
	}
	
// Functions and other crap below

// Hit-related functions, you will find the numerous reasons why the player dies

    /// <summary>
    /// OnHit Function #1, this is called when the player touches a shot.
    /// </summary>
    /// <param name="data"> HitData class to get informatoin about the shot </param>
    void OnHit(HitData data)
    {
        if (!isInvincible)
        {
            Environment.PlaySound(Audio.sfx.plDeath);
            isInvincible = true;
            StartCoroutine(Deathbomb(data));
        }
    }

    /// <summary>
    /// OnHit Function #2, this is called when the player touches something else that is not a shot.
    /// </summary>
    /// <param name="objectHit"> GameObject that the player touched </param>
    void OnHit(GameObject objectHit)
    {
        if (!isInvincible)
        {
            Environment.PlaySound(Audio.sfx.plDeath);
            isInvincible = true;
            StartCoroutine(Deathbomb(new HitData(objectHit)));
        }
    }

    /// <summary>
    /// OnCollision function, this is called when the player touches an enemy, shot, or other collision objects.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Entity" || collision.gameObject.tag == "Boss")
        {
            if (!isInvincible)
            {
                // If the player is not invincible, the death cycle is called.
                OnHit(collision.gameObject);
            }
            else if (collision.gameObject.tag == "Entity")
            {
                // Otherwise, if the player is invincible and the enemy is not a boss, the enemy dies instead.
                collision.gameObject.SendMessage("OnHit", new HitData(gameObject));
            }
        }
    }

    /// <summary>
    /// Kill function that fires when the deathbomb window closes. Need I say more?
    /// </summary>
    private void Kill()
    {
        lives--;
        power = Mathf.Clamp(power - 1, 0, player.maxPower);
        Environment.SpawnItem(Item.ItemType.bigPower, gameObject, true, Random.Range(10F, 15F));
        Environment.SpawnItem(Item.ItemType.bigPower, gameObject, true, Random.Range(10F, 15F));
        Environment.SpawnItem(Item.ItemType.bigPower, gameObject, true, Random.Range(10F, 15F));
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        isFocused = false;
        transform.Find("HitboxAssist").gameObject.SetActive(false);
        transform.position = new Vector3(0, -5.5F, 0);
        isAlive = false;
        StartCoroutine(PlayerRespawn());
    }

    /// <summary>
    /// Interface responsible for allowing the player to deathbomb.
    /// </summary>
    IEnumerator Deathbomb(HitData data)
    {
        yield return new WaitForSecondsRealtime(0.2F);
        if (!isBombing)
        {
            Environment.lockInput = true;
            canBomb = false;

            string hitSourceName;

            if (data.source != null)
            {
                hitSourceName = data.source.name;
            }
            else
            {
                hitSourceName = "a dead enemy";
            }

            if (data.hitType == HitData.HitDataType.shotHit)
            {
                Debug.Log("Player has died by a shot that dealt " + data.damage + " damage from " + hitSourceName + ". Lives remaining: " + lives);
            }
            else
            {
                Debug.Log("Player has died by touching " + hitSourceName + "! What a shame. Lives remaining: " + lives);
            }
            Kill();
        }
        else
        {
            Debug.Log("Player has successfully pulled a deathbomb. Congratulations, you lucky bastard.");
        }
    }

    /// <summary>
    /// Interface responsible for player respawn.
    /// </summary>
    IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(1);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        isAlive = true;
        lerpTime = 0;
        playerRespawnTranslation = true;
        yield return new WaitForSeconds(2);
        Environment.lockInput = false;
        canBomb = true;
        yield return new WaitForSeconds(2);
        isInvincible = false;
        Debug.Log("Respawn cycle complete.");
    }

    // Reserved section for something else


    // Methods for other things

    /// <summary>
    /// Method for processing an item when a player picks them up.
    /// </summary>
    /// /// <param name="itemType"> The type of item that is processed. </param>
    public void GetItem(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.smallPower:
                AddPower(0.01F);
                AddScore(100);
                Environment.PlaySound(Audio.sfx.itemPickup);
                break;
            case Item.ItemType.bigPower:
                AddPower(0.05F);
                AddScore(1000);
                Environment.PlaySound(Audio.sfx.itemPickup);
                break;
            case Item.ItemType.point:
                points++;
                AddScore(10000);
                Environment.PlaySound(Audio.sfx.itemPickup);
                break;
            case Item.ItemType.bomb:
                bombs++;
                AddScore(1000);
                Environment.PlaySound(Audio.sfx.extend);
                break;
            case Item.ItemType.life:
                lives++;
                AddScore(1000);
                Environment.PlaySound(Audio.sfx.extend);
                break;
            case Item.ItemType.fullPower:
                AddPower(player.maxPower);
                AddScore(5000);
                break;
            default:
                Debug.Log("PlayerHalder.AddItem() was called with an invalid item. Did you somehow create a new item?");
                break;
        }

    }

    /// <summary>
    /// Method for processing an item when a player picks them up.
    /// </summary>
    /// /// <param name="item"> The item class of the item that is processed. </param>
    public void GetItem(Item item)
    {
        GetItem(item.GetItemType());
    }

    /// <summary>
    /// Method for calculating and adding an int score.
    /// </summary>
    public void AddScore(int value)
    {
        score += (ulong)(value * GameManager.pointMultiplier);
    }

    /// <summary>
    /// Method for calculating and adding a ulong score.
    /// </summary>
    public void AddScore(ulong value)
    {
        score += (ulong)(value * GameManager.pointMultiplier);
    }

    /// <summary>
    /// Method for adding power.
    /// </summary>
    public void AddPower(float value)
    {
        if ((int)power < (int)(power + value))
        {
            Environment.PlaySound(Audio.sfx.powerUp);
        }

        power = Mathf.Clamp(power + value, 0, player.maxPower);
    }

    /// <summary>
    /// Handles any trigger event that touches the player.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Collect All Items Trigger")
        {
            Environment.CollectAllItems();
        }
    }

    private void FixedUpdate()
    {
        if (playerRespawnTranslation)
        {
            lerpTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp(lerpTime, 0, 2) / 2F;
            transform.position = Vector3.Lerp(new Vector3(0, -5.5F, 0), new Vector3(0, -4F, 0), lerpValue);
            if (lerpValue == 1F)
            {
                playerRespawnTranslation = false;
            }
        }
    }
}
