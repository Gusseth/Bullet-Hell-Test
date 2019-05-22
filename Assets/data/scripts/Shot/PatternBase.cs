using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Base and struct class for all bullet patterns and attack components.
/// </summary>
[System.Serializable]
public class PatternBase : object
{
    /// <summary>Amount of bullets that will be divided evenly to fill a given angle.</summary>
    public int amount;

    /// <summary>Speed of each shot.</summary>
    public float shotSpeed;

    /// <summary>Range of angle in degrees that each shot will be distributed in.</summary>
    public float angleRange;

    /// <summary>Insert the parent via transform.parent.</summary>
    public GameObject parent;

    /// <summary>Shot used as a prefab.</summary>
    public GameObject bulletPrefab;

    /// <summary>Sound effect used after the pattern is made.</summary>
    public Audio.sfx soundEffect;

    /// <summary>The colour of the sprite, modifies the suffix so that it changes colour based on this value.</summary>
    public int spriteType;

    /// <summary>Set to false if the pattern should start either at angle zero or the given angle offset.</summary>
    public bool centreAngle;

    /// <summary>Where the angle in degrees should start relative to straight down (straight down = angle 0).</summary>
    public float angleOffset;

    /// <summary>The time between the previous pattern and this in seconds.</summary>
    public float delay;


    // Class constructor

    /// <summary>
    /// Construct a pattern base for various things!
    /// </summary>
    /// <param name="amount">Amount of bullets that will be divided evenly to fill a given angle.</param>
    /// <param name="shotSpeed">Speed of each shot.</param>
    /// <param name="angleRange">Range of angle in degrees that each shot will be distributed in.</param>
    /// <param name="delay">The time between the previous pattern and this in seconds.</param>
    /// <param name="parent">Insert the parent via transform.parent.</param>
    /// <param name="bulletPrefab">Shot used as a prefab.</param>
    /// <param name="soundEffect">Sound effect used after the pattern is made.</param>
    /// <param name="spriteType">The colour of the sprite, modifies the suffix so that it changes colour based on this value.</param>
    /// <param name="centreAngle">Set to false if the pattern should start either at angle zero or the given angle offset.</param>
    /// <param name="angleOffset">Where the angle in degrees should start relative to straight down (straight down = angle 0).</param>
    public PatternBase(int amount, float shotSpeed, float angleRange, float delay, GameObject parent, GameObject bulletPrefab, Audio.sfx soundEffect, int spriteType = 0, bool centreAngle = true, float angleOffset = 0)
    {
        this.amount = amount;
        this.shotSpeed = shotSpeed;
        this.angleRange = angleRange;
        this.parent = parent;
        this.soundEffect = soundEffect;
        this.spriteType = spriteType;
        this.centreAngle = centreAngle;
        this.angleOffset = angleOffset;
        this.delay = delay;
    }

    // Functions and other crap below

    /// <summary>
    /// Executionary function that does what the pattern is supposed to do given the information from the class.
    /// </summary>
    public void RainDownHell()
    {
        for (int i = 0; i < amount; i++)
        {
            // 'for' loop that spawns every individual bullet into one grouped barrage
            GameObject shot = GameObject.Instantiate(bulletPrefab); // Spawns the shot
            shot.transform.position = parent.transform.position; 

            // Sets angle 0 as pointing straight down. Further modification is done by the angleoffset value.
            float angle = 180F + angleOffset;

            float shotRotation; // Used to rotate the shot towards its designated direction

            // Setting the shot rotations below

            if (centreAngle)
            {
                // If the resultant angle should be at the centre of the barrage
                shotRotation = 
                angle - (angleRange / 2F)                                    // This takes the resultant angle and subtracts the starting point of the barrage by half of the angle range, bisecting it on the resultant angle
                +                                                            // Adding the position needed to space out the bullets
                i * (angleRange / Mathf.Clamp(amount - 1, 1, int.MaxValue)); // Basically takes the gap needed to fill the angle with the given amount of bullets. 
                                                                             // The newly spawned bullet is placed by taking the place of the bullet in the 'for' loop and multiplying it by the gap; evenly distributing the bullets in the given angle.
            }
            else
            {
                // If the resultant angle should be the starting position of the barrage
                shotRotation = angle + i * (angleRange / Mathf.Clamp(amount - 1, 1, int.MaxValue));
                // In short, the gap as explained in the wall of text above is added on to the existing angle, making the first bullet in the pattern always face the resultant angle.
            }
            shot.GetComponent<ShotHandler>().source = parent; // Sets the parent to trace back the source of this bullet.
            shot.transform.Rotate(new Vector3(0, 0, shotRotation)); // Sets the direction of where this shot is headed.
            shot.GetComponent<ShotHandler>().displacement = Environment.CalculateShotDisplacement(shotSpeed); // Sets the rate in which the bullet displaces itself in world space.
        }
        Environment.sfxAudioSource.PlayOneShot(Audio.Parse(soundEffect), Environment.sfxVolume * .4F); // Plays audio.
    }
}

/// <summary>
/// Data structure responsible for storing bullet patterns into one list; an attack stage.
/// </summary>
[System.Serializable]
public class AttackStage : List<PatternBase>
{
    /// <summary>
    /// The name of this attack.
    /// </summary>
    public string name;
    
    /// <summary>
    /// The boss' health requirement to move on to the next Attack Stage.
    /// </summary>
    public float healthTriggerPoint;

    /// <summary>
    /// True if this Attack Stage is a spellcard.
    /// </summary>
    public bool isSpellcard;
    
    /// <summary>
    /// The attack stage list that contains all bullet patterns in the stage.
    /// </summary>
    public List<PatternBase> Stage = new List<PatternBase>();

    /// <summary>
    /// The items dropped once the AttackStage is over.
    /// </summary>
    public List<Item.ItemType> LootTable = new List<Item.ItemType>();

    /// <summary>
    /// Constructs an attack stage from the given list of patterns.
    /// </summary>
    /// <param name="name">The name of this attack.</param>
    /// <param name="healthTrigger">The boss' health requirement to move on to the next Attack Stage.</param>
    /// <param name="isSpellcard">True if this Attack Stage is a spellcard.</param>
    /// <param name="patterns">Constructs an attack stage from the given list of patterns.</param>
    public AttackStage(string name, float healthTrigger, bool isSpellcard, List<PatternBase> patterns)
    {
        Stage = patterns;
        healthTriggerPoint = healthTrigger;
        this.isSpellcard = isSpellcard;
        this.name = name;
    }
    
    /*
    async void ExecutionLoop(int index, float waitSeconds)
    {
        float delay = Stage[index].RainDownHell();
        await Task.Delay((int)(waitSeconds * 1000));
        Debug.Log("test " + index + " " + delay);
        if (index < Stage.Capacity)
        {
            ExecutionLoop(index++, delay);
        }
        else
        {
            ExecutionLoop(0, delay);
        }
    }
    */
}
