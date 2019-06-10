using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages generic entities and generic enemies.
/// </summary>
public class ItemHandler : MonoBehaviour {

    // Variable Declaration

    /// <summary> The type of this item (smallPower, bigPower, etc.). </summary>
    public Item.ItemType itemType;

    /// <summary> Conditional variable that turns true if it entered the player's detection collider. </summary>
    public bool followingEntity;

    /// <summary> The entity that it is following as a GameObject. </summary>
    public GameObject target;

    /// <summary> How amplified the force is applied towards the target once a target has been defined. </summary>
    private float forceMult;

    private Rigidbody2D rb;


    // Use this for initialization adn setting variables
    void Awake ()
    {
        // Initialization stuff, please ignore
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = Environment.itemGravityScale;

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

    // Use this for behavioural changes after the GameObject has properly loaded
    void Start()
    {
        // Immediately heads towrds the player if the item spawns and the bomb is still activated
        if (PlayerHandler.isBombing)
        {
            PlayerCollect();
        }

        if (rb.velocity == Vector2.zero)
        {
           rb.velocity = new Vector2(0, Random.Range(1F, 8F));
        }
    }

    /// <summary> This function directs the item towards the player. </summary>
    public void PlayerCollect()
    {
        target = Environment.player;
        rb.gravityScale = 0;
        forceMult = 100F;
        followingEntity = true;
    }

    /// <summary> This is called if the item enters an item trigger zone </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Item Trigger")
        {
            // Once a hit is detected, the target is set and gravity is ignored
            target = collision.transform.parent.gameObject;
            rb.gravityScale = 0;
            forceMult = 100F;
            followingEntity = true;
        }
    }

    /// <summary> This is called if the item enters touches a valid collision entity. </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == target)
        {
            // If the item touches the entity it's targeting, it finds its item response function GetItem()
            collision.gameObject.SendMessage("GetItem", itemType);
            Destroy(gameObject);
        }
    }

    /// <summary> This is resets the state of the item back to freefall mode. </summary>
    void ResetItemState()
    {
        followingEntity = false;
        target = null;
        rb.gravityScale = Environment.itemGravityScale;
    }

    void Update()
    {
        if (target != null && followingEntity)
        {
            // If the item is following a valid target
            if (target == Environment.player && !PlayerHandler.isAlive)
            {
                // If the target is the player and if the player dies, the item is sent back to freefall mode
                ResetItemState();
                return;
            }

            // Otherwise, gravitate towards the target
            Vector2 deltaPos = (Vector2)target.transform.position - (Vector2)transform.position;
            rb.AddForce(deltaPos.normalized * forceMult * rb.mass);
        }
        else if (target == null && followingEntity)
        {
            // If the item is following an invalid target (target that died or deleted), item is back to freefall
            ResetItemState();
        }
    }
}
