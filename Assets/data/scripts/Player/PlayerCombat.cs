using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

// Variable Declaration

    // General Variables
    Shot playerShot;
    public GameObject test;
    private bool canShoot = true;
    private bool doShootSFX = true;

    // Player variables
    protected GameObject player;
    protected PlayerHandler playerHandler;
    protected Player.CharacterShot shotType;

    // Controls
    KeyCode shootButton = KeyCode.Z;                     // Button to shoot
    KeyCode bombButton  = KeyCode.X;                     // Button to bomb


// Functions and other crap below


    private void Start()
    {
        player = Environment.player;
        playerHandler = Environment.playerHandler;
        shotType = playerHandler.characterShot;
    }

    /// <summary>
    /// Function to shoot, only called when the player holds down the fire key. You shouldn't be referencing this.
    /// </summary>
    private void Shoot()
    {
        canShoot = false;
        int power = (int)playerHandler.power;
        switch (shotType)
        {

            case Player.CharacterShot.A:
                if (playerHandler.character == Player.Character.Reimu)
                {
                    break;
                }
                else if (playerHandler.character == Player.Character.Marisa)
                {
                    break;
                }
                else
                {
                    Debug.LogError("Player tried to shoot with an invalid character!", this);
                }
                break;


            case Player.CharacterShot.B:
                if (playerHandler.character == Player.Character.Reimu)
                {
                    for (int i = 0; i <= power; i++)
                    {
                        float rotateOffset = (i + 1) * (1F / (power + 2));
                        GameObject newShot = Instantiate(test, new Vector2(playerHandler.transform.position.x, playerHandler.transform.position.y + 0.5F), test.transform.rotation * Quaternion.Euler(0,0,(-10F + 20F * rotateOffset)));
                        newShot.GetComponent<ShotHandler>().source = gameObject; // Assigns the player as the source of the bullet
                        newShot.GetComponent<ShotHandler>().displacement = Environment.CalculateShotDisplacement(playerHandler.player.shotSpeed1);

                        // If the shot is not visible on camera, it is deleted immediately.
                    }
                    canShoot = true;
                    break;
                }
                else if (playerHandler.character == Player.Character.Marisa)
                {
                    break;
                }
                else
                {
                    Debug.LogError("Player tried to shoot with an invalid character!", this);
                }
                break;

            case Player.CharacterShot.C:
                if (playerHandler.character == Player.Character.Reimu)
                {
                    break;
                }
                else if (playerHandler.character == Player.Character.Marisa)
                {
                    break;
                }
                else
                {
                    Debug.LogError("Player tried to shoot with an invalid character!", this);
                }
                break;
        }

        // Plays the shooting sound effect
        if (doShootSFX)
        {
            doShootSFX = false;
            StartCoroutine(PlayShootSFX(0.075F)); // Is preferred to only run every 4 frames. This is not frame-locked, however,
        }
    }

    /// <summary>
    /// Function to bomb, only called when the player pushes the bomb key. You shouldn't be referencing this.
    /// </summary>
    private void Bomb()
    {
        playerHandler.bombs--;
        Debug.Log("Player has bombed. What a scrub.");
        playerHandler.canBomb = false;
        playerHandler.isBombing = true;
        playerHandler.isInvincible = true;
        Environment.PlaySound(Audio.sfx.masterSpark, 1F);
        Environment.CollectAllItems();
        StartCoroutine(BombCooldown());
    }

    /// <summary>
    /// Interface that enforces the sound effect is only played in defined intervals.
    /// </summary>
    IEnumerator PlayShootSFX(float interval)
    {
        Environment.PlaySound(Audio.sfx.plShoot, Audio.sfxNormalPriority * Environment.sfxMasterVolume);
        yield return new WaitForSecondsRealtime(interval);
        doShootSFX = true;
    }

    /// <summary>
    /// Interface that enforces cooldown during bombing.
    /// </summary>
    IEnumerator BombCooldown()
    {
        yield return new WaitForSeconds(5F);
        playerHandler.isBombing = false;
        yield return new WaitForSeconds(1F);
        playerHandler.isInvincible = false;
        playerHandler.canBomb = true;
    }


    /* Use this for initialization
    
	void Start ()
    {
		
	}
    */

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (!Environment.lockInput)
        {
            if (Input.GetKey(shootButton))
            {
                Shoot();
            }

            if (Input.GetKeyDown(bombButton) && playerHandler.canBomb)
            {
                Bomb();
            }
        }
	}

 
}
