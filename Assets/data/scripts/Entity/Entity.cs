using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for all entities (player, enemy, boss, anything you can shoot). There other derivative classes such as Enemy or  Player for such purposes.
/// </summary>
public class Entity {

    /// <summary> This is the health for this entity. 1 = 1 Reimu shot; 1.5 = 1 Marisa shot. </summary>
    public float health = 1;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

/// <summary>
/// This is the base class for all enemies, but not bosses or midbosses.
/// </summary>
public class Enemy : Entity
{

    /// <summary>
    /// Class Struct #1, creates a generic enemy for debug purposes, or spam
    /// </summary>
    public Enemy()
    {

    }

    /// <summary>
    /// Class Struct #2, creates a generic enemy with modifiable sprites and 
    /// </summary>
}

/// <summary>
/// This is the base class for all bosses and midbosses in the game.
/// </summary>
public class Boss : Enemy
{
    /// <summary> This is the health for the enemy. 1 = 1 Reimu shot; 1.5 = 1 Marisa shot. </summary>
    //public ushort health = 1;
}
