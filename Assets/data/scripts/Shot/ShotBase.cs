using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all bullets in the game, contains the bullet's information, but not the script itself.
/// </summary>
public class Shot {

// Variable Declaration

    // Physics Variables
    public float speed;
    public float rotation;

    // Appearance Variables
    public Color32 colour;
    protected Sprite sprite;

    // Shot Variables
    public float damage = 1F;
    public ShotType shotType;
    public ShotSubType subType;

    // Gameplay Variables
    public bool penetrate = false;
    public bool isRotating = false;

    public enum ShotType
    {
        special, ball, laser
    }

    public enum ShotSubType
    {
        special, tinyShot, smallShot, bigShot
    }

// Functions and other crap below

    /// <summary>
    /// Class struct #1, function below creates a generic bullet for debug purposes
    /// </summary>
    public Shot()
    {
        speed = 5.0F;
        rotation = 0;
        colour = Color.red;
    }

    /// <summary>
    /// Class struct #2, function below creates a custom bullet in the most specific way possible
    /// </summary>
    public Shot(float speedValue, float rotationValue, Color32 colourValue, Sprite shotSprite, float damageValue, ShotType shotTypeValue, ShotSubType shotSubTypeValue)
    {
        speed = speedValue;
        rotation = rotationValue;
        colour = colourValue;
        sprite = shotSprite;
        damage = damageValue;
        shotType = shotTypeValue;
        subType = shotSubTypeValue;
    }
}

/// <summary>
/// Class that contains information about the bullet that hit the entity.
/// </summary>
public class HitData
{
    public float damage;
    public GameObject source;
    public Shot shot;
    public HitDataType hitType;
    public enum HitDataType
    {
        shotHit, entityHit
    }

    /// <summary>
    /// Class struct #1, Creates a new HitData entry that can be used to extract information about the shot. This one only includes the source of the damage.
    /// </summary>
    public HitData(GameObject hitSource)
    {
        source = hitSource;
        hitType = HitDataType.entityHit;
        damage = int.MaxValue;
    }

    /// <summary>
    /// Class struct #2, Creates a new HitData entry that can be used to extract information about the shot.
    /// </summary>
    public HitData(float damageValue, GameObject hitSource)
    {
        damage = damageValue;
        source = hitSource;
        hitType = HitDataType.shotHit;
    }

    /// <summary>
    /// Class struct #3, Creates a new HitData entry that can be used to extract information about the shot that includes the Shot class.
    /// </summary>
    public HitData(float damageValue, GameObject hitSource, Shot shotData)
    {
        damage = damageValue;
        source = hitSource;
        shot = shotData;
        hitType = HitDataType.shotHit;
    }
}

