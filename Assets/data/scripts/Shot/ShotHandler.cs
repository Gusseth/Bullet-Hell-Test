using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages the bullet GameObject.
/// </summary>
public class ShotHandler : MonoBehaviour {

// Variable Declaration

    public Shot shot;
    Vector3 rotation;

    public Vector3 displacement;

    public GameObject source;
    string sourceName = "This is not supposed to be empty!";
    public float speed;
    public bool isRotating;
    private bool hit = false;

// Class-exclusive functions

    public void OnShotNullified()
    {
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
        if (collision.gameObject.name == "Graze Area" && gameObject.layer != LayerMask.NameToLayer("PlayerShot"))
        {
            // If the affected area is a graze area and the shot is not from the player
            Environment.playerHandler.graze++;
            Environment.PlaySound(Audio.sfx.graze, Audio.sfxNormalPriority * Environment.sfxMasterVolume);
        }
    }

    // When the bullet is out of the camera's view, it gets deleted

    void OnBecameInvisible()
    {
        //StartCoroutine(selfDestructTimeout());
        Destroy(gameObject, 0.5F);
    }
}
