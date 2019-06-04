using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages generic entities and generic enemies.
/// </summary>
public class EntityHandler : MonoBehaviour {

// Variable Declaration

    // Gameplay Variables
    /// <summary> This entity's health. Need I say more? </summary>
    public float health;

    /// <summary> The base score added when this entity is killed. </summary>
    public int scoreAdd;

    /// <summary> Entity loot table, these are dropped once this entity dies. </summary>
    public List<Item.ItemType> lootTable = new List<Item.ItemType>();

    // Technical Variables
    /// <summary> The entity class attatched to this handler. Basically, what the entity is supposed to be. </summary>
    public Entity entity;

    /// <summary> The type of entity/enemy this entity is. </summary>
    public EntityType entityType = EntityType.generic;

    public enum EntityType
    {
        generic, boss, blueFairy, redFairy, greenFairy, yellowFairy, blueFairyScythe, redFairyScythe, greenFairyScythe, yellowFairyScythe
    }


    // Functions and other crap below

    void OnHit(HitData data)
    {
        health -= data.damage;
        if (health <= 0F)
        {
            if (entityType != EntityType.boss)
            {
                Destroy(gameObject);
                // If the entity is not a stage boss or a miniboss
                Environment.PlaySound(Audio.sfx.enemyDeath, Environment.sfxMasterVolume * 0.6F);

                // Spawns the items in the loot table when the entity's health is at or less than 0.
                foreach (Item.ItemType item in lootTable)
                {
                    Environment.SpawnItem(item, gameObject);
                }
            }
            else
            {
                // If the entity is indeed a boss/miniboss...

                // Nothing here yet...
            }

            // Adds the score to the player
            Environment.playerHandler.AddScore(scoreAdd);
        }
        else
        {
            // If health is not below 0...
            Environment.PlaySound(Audio.sfx.damage0, Audio.sfxLowPriority * Environment.sfxMasterVolume);
        }

    }

    // Use this for initialization
    void Start ()
    {
		if (entity == null)
        {
            entity = new Entity();
        }

        // Default loot table drops and variable assignment according to the enmy type ex. blue fairies always drop blue points

        // Variable assignments

        if (health == 0)
        {
            health = entity.health;
        }

        // Loot table related things

        if (entity.GetType() == typeof(Enemy))
        {
            Enemy enemy = (Enemy)entity;

           /* switch (enemy.enemyType)
            {
                case (Enemy.EnemyType.blueFairy || Enemy.EnemyType.blueFairyScythe):
                    lootTable.Add(Item.ItemType.point);
                    break;
                case 
            }*/
        }

        // Inserted loot table drops if no special loot table is assigned for this specific enemy.
        if (lootTable.Count == 0)
        {

        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}
}
