using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all bullets in the game, contains the bullet's information, but not the script itself.
/// </summary>
public class Shot {

    // Variable Declaration

    // Physics Variables
    /// <summary> The speed of this shot. </summary>
    public float speed;

    /// <summary> The transform rotation of this shot. </summary>
    public float rotation;

    // Appearance Variables

    /// <summary> The colour of this shot. </summary>
    public Color32 colour;

    /// <summary> The sprite of the shot. </summary>
    public Sprite sprite;

    // Shot Variables

    /// <summary> The damage dealt to the entity that this shot hits. </summary>
    public float damage = 1F;

    /// <summary> The type of shot: special, ball, laser </summary>
    public ShotType shotType;

    /// <summary> The shot sub-type: special, tiny shot, small shot, big shot </summary>
    public ShotSubType subType;

    // Gameplay Variables
    /// <summary> Should this shot penetrate through entities? </summary>
    public bool penetrate = false;

    /// <summary> Returns true of this shot's transform rotation changes. </summary>
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

    // Variable Declaration

    /// <summary> The damage dealt by the shot received. </summary>
    public float damage;

    /// <summary> The GameObject of the shot's source. </summary>
    public GameObject source;

    /// <summary> The Shot class of the shot received. </summary>
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

