﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages stage midbosses and bosses.
/// </summary>
public class BossHandler : MonoBehaviour {

    // Variable Declaration

    /// <summary> The name of the boss. </summary>
    public string bossName;

    /// <summary> The name of the boss in Japanese. </summary>
    public string bossNameJP;

    /// <summary> The boss' health. </summary>
    public float bossHealth;

    /// <summary> The boss' maximum health. </summary>
    public float maxHealth = ushort.MaxValue;

    /// <summary> True if the parent is a midboss. </summary>
    public bool midboss = false;

    /// <summary> True if a spellcard is activated. </summary>
    public bool spellcardActive = false;

    /// <summary> True if a spellcard is activated. </summary>
    public bool isInvincible = false;

    /// <summary> True if the boss' death function is triggered.. </summary>
    public bool isDead = false; // Is this always true for undead bosses?

    /// <summary> The health threshold that will trigger a switch in the boss' Attack Stage. </summary>
    public float healthTriggerPoint;

    /// <summary> This is the previous value of healthTriggerPoint before a switch in AttackStage. </summary>
    private float previousTriggerPoint;

    /// <summary> List of all attacks of the boss, including non-spell card attacks. Each object, an AttackStage, is a nested list that contains the bullet patterns. </summary>
    public List<AttackStage> AttackTable = new List<AttackStage>();

    bool playDamageSound = true;

    /// <summary>
    /// This is the main body of the Attack Table system. Loops the contents of an AttackStage.
    /// </summary>
    /// <param name="stageIndex">The index of the AttackStage list also known as the position of the pattern to be accessed in the AttackStage list.</param>
    /// <param name="waitSeconds">Number of seconds for the delay present in between the previous pattern and the upcoming pattern.</param>
    /// <returns></returns>
    IEnumerator ExecutionLoop(int stageIndex, float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);    // Waits for the given number of seconds.
        yield return new WaitForFixedUpdate();           // Waits for the next FixedUpdate frame so patterns don't boil down into discord after a slight lag.
        AttackTable[0].Stage[stageIndex].RainDownHell(); // Runs the bullet spawning function of the pattern.
        float delay;
        int stageCapacity = AttackTable[0].Stage.Capacity - 1; 
        // This is subtracted by one because the capacity of lists start at 1 while list indexes, the value between square brackets in lists, start at 0. 
        
        // Say hello to recursion!

        if (stageIndex < stageCapacity)
        {
            // If the index is smaller than the AttackStage capacity or if the pattern is not the last in the AttackStage in layman's terms.
            delay = AttackTable[0].Stage[stageIndex + 1].delay;   // Take the delay of the next pattern to be used in the next generation of this loop.
            StartCoroutine(ExecutionLoop(stageIndex + 1, delay)); // Continue the loop by running this same function again, but using the next pattern as the subject.
        }

        else
        {
            // If the index is equal to the AttackStage capacity or if the pattern is the last...
            delay = AttackTable[0].Stage[0].delay;      // Take the delay of the very first pattern as it will be the next pattern after this.
            StartCoroutine(ExecutionLoop(0, delay));    // Continue the loop by running the first pattern in the AttackStage.
        }
    }

    public void TriggerAttack()
    {
        StartCoroutine(ExecutionLoop(0, 0));
    }

    public void StopBulletLoop()
    {
        StopAllCoroutines();
        playDamageSound = true;
    }

    public void MoveToNextAttackStage()
    {
        StopBulletLoop();
        Environment.ClearAllShots();

        foreach (Item.ItemType item in AttackTable[0].LootTable)
        {
            // Spawns in every item in the loot table
            Environment.SpawnItem(item, transform.position);
        }

        try
        {
            // Try removing the current attack stage
            AttackTable.Remove(AttackTable[0]);
            isInvincible = true;
            spellcardActive = AttackTable[0].isSpellcard;

            if (spellcardActive)
            {
                Environment.PlaySound(Audio.sfx.spellcard, Environment.sfxVolume * .4F);
            }

            IEnumerator smallDelay = Environment.AddDelay(3,
            delegate
            {
                // This part will only error out with an ArgumentOutOfRangeException if the attack table is empty, triggering the catch statement below
                previousTriggerPoint = healthTriggerPoint;
                healthTriggerPoint = AttackTable[0].healthTriggerPoint;
                isInvincible = false;
                TriggerAttack();
            });
            StartCoroutine(smallDelay);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            // If the boss dies...
            isDead = true;
            Destroy(GetComponent<CircleCollider2D>());
            Debug.Log(bossName + " has been defeated!");
            Environment.PlaySound(Audio.sfx.bossDeath, Environment.sfxVolume * .4F);
            Vector3 pos = transform.position;
            Destroy(gameObject, 1);
        }
    }

    public void OnHit(HitData data)
    {
        if (!isInvincible)
        {
            if (spellcardActive)
            {
                if (healthTriggerPoint == maxHealth)
                {
                    // If the current Attack Stage is a spellcard and it is a spellcard that occupies the entire health bar, it reduces damage by 1/6.
                    bossHealth = Mathf.Clamp(bossHealth - (data.damage / 4F), 0, maxHealth);
                    if (playDamageSound)
                    {
                        if (bossHealth <= maxHealth / 5F)
                        {
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage1, Environment.sfxVolume * 0.4F);
                            StartCoroutine(Environment.AddDelay(.1F,
                            delegate
                            {
                                playDamageSound = true;
                            }));
                        }
                        else
                        {
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage0, Environment.sfxVolume * 0.4F);
                            StartCoroutine(Environment.AddDelay(.1F,
                            delegate
                            {
                                playDamageSound = true;
                            }));
                        }
                    }
                }
                else
                {
                    // Else, it reduces damage by 1/24.
                    bossHealth = Mathf.Clamp(bossHealth - (data.damage / 24F), healthTriggerPoint, maxHealth);

                    if (playDamageSound)
                    {
                        if (bossHealth <= (healthTriggerPoint) + (previousTriggerPoint - healthTriggerPoint) / 5F)
                        {
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage1, Environment.sfxVolume * 0.4F);
                            StartCoroutine(Environment.AddDelay(.1F,
                            delegate
                            {
                                playDamageSound = true;
                            }));
                        }
                        else
                        {
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage0, Environment.sfxVolume * 0.4F);
                            StartCoroutine(Environment.AddDelay(.1F,
                            delegate
                            {
                                playDamageSound = true;
                            }));
                        }
                    }
                }
            }
            else
            {
                // Else if the Attack Stage is a normal attack, no damage reduction is taken.
                bossHealth = Mathf.Clamp(bossHealth - data.damage, healthTriggerPoint, maxHealth);

                if (playDamageSound)
                {
                    if (bossHealth <= (healthTriggerPoint) + ((maxHealth - healthTriggerPoint) / 5F))
                    {
                        playDamageSound = false;
                        Environment.PlaySound(Audio.sfx.damage1, Environment.sfxVolume * 0.4F);
                        StartCoroutine(Environment.AddDelay(.1F,
                        delegate
                        {
                            playDamageSound = true;
                        }));
                    }
                    else
                    {
                        playDamageSound = false;
                        Environment.PlaySound(Audio.sfx.damage0, Environment.sfxVolume * 0.4F);
                        StartCoroutine(Environment.AddDelay(.1F,
                        delegate
                        {
                            playDamageSound = true;
                        }));
                    }
                }
            }


            if (bossHealth == 0)
            {
                bossHealth = maxHealth;
                MoveToNextAttackStage();
            }
            else if (bossHealth == healthTriggerPoint && !spellcardActive)
            {
                bossHealth = healthTriggerPoint;
                MoveToNextAttackStage();
            }
        }
    }

    // Use this for initialization
    void Start () {
        TriggerAttack();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
