using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages the bullet GameObject.
/// </summary>
public class ShotHandler : MonoBehaviour {

// Variable Declaration

    /// <summary> Shot class of this shot. </summary>
    public Shot shot;

    /// <summary> The current rotation of this shot. </summary>
    Vector3 rotation;

    /// <summary> Temporary viariable. </summary>
    public Vector3 displacement;

    /// <summary> The source GameObject of this shot. </summary>
    public GameObject source;

    /// <summary> The name of this shot's source. </summary>
    string sourceName = "This is not supposed to be empty!";

    /// <summary> The speed of this shot. </summary>
    public float speed;

    /// <summary> True of the actual GameObject of this shot is rotating. </summary>
    public bool isRotating;

    /// <summary> Returns true if it hits a valid entity. </summary>
    private bool hit = false;

    /// <summary> Sets to false if this shot as already grazed the player. </summary>
    private bool canGraze = true;

// Class-exclusive functions

    public void OnShotNullified()
    {
        // When a message is sent to this shot to destroy itself
        Destroy(gameObject);
    }

 // Other functions
    void Awake()
    {

        /*if (speed <= 0F)
        {
            speed = 3F;
        }*/

        if (rotation == null)
        {
            rotation = new Vector3(0, 0, 0);
        }

        if (shot == null)
        {
            shot = new Shot();
        }

        displacement = Environment.CalculateShotDisplacement(speed);

        // Finds if a DestroyOnBorder script exists as a redundancy. Creates one if one doesn't exist.
        try
        {
            GetComponent<DestroyOnBorder>().enabled = true;
        }
        catch (System.NullReferenceException)
        {
            gameObject.AddComponent<DestroyOnBorder>();
        }

        
    }

    // FixedUpdate is called once per frame
    void FixedUpdate ()
    {
        if (!hit)
        {
            transform.Rotate(rotation);
            transform.Translate(displacement);
        }
        if (shot.isRotating || isRotating)
        {
            transform.GetChild(0).Rotate(0, 0, 15);
        }
	}

    // This is only called if the bullet hits the player, an enemy, or other things, listed as an entity.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.transform.GetComponent<EntityHandler>() || collision.transform.GetComponent<BossHandler>() || collision.transform.GetComponent<PlayerHandler>()) && collision.gameObject != source)
        {
            // Checks if the hit object is a valid entity, boss, or player AND it isn't the source of this shot
            hit = true;
            collision.gameObject.SendMessage("OnHit", new HitData(shot.damage, source, shot)); // Fires the OnHit function in the entity
            if (!shot.penetrate)
            {
                Destroy(gameObject);
            }
        }
    }

    // Only used for grazing, nothing else for now.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Graze Area" && gameObject.layer != LayerMask.NameToLayer("PlayerShot") && canGraze)
        {
            // If the affected area is a graze area AND the shot is not from the player AND can still count for graze
            Environment.playerHandler.graze++;
            Environment.PlaySound(Audio.sfx.graze, Audio.sfxNormalPriority * Environment.sfxMasterVolume);
            canGraze = false;
        }
    }

    // When the bullet is out of the camera's view, it gets deleted

    void OnBecameInvisible()
    {
        //StartCoroutine(selfDestructTimeout());
        Destroy(gameObject, 0.5F);
    }
}
