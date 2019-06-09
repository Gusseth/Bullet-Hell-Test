using System.Collections;
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
    public bool isInvincible = true;

    /// <summary> True if the boss' death function is triggered.. </summary>
    public bool isDead = false; // Isn't this always true for undead bosses?

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

    /// <summary>
    /// This method is called by GameManager.BossAttack()
    /// </summary>
    public void TriggerAttack()
    {
        isInvincible = false;
        StartCoroutine(ExecutionLoop(0, 0));
    }

    public void StopBulletLoop()
    {
        StopAllCoroutines();
        playDamageSound = true;
    }

    /// <summary>
    /// Removes the current Attack Stage from the AttackTable and inserts, then plays the next.
    /// </summary>
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
                Environment.PlaySound(Audio.sfx.spellcard, Audio.sfxNormalPriority * Environment.sfxMasterVolume);
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

            if (!midboss)
            {
                // if the boss is not a midboss...
                Environment.PlaySound(Audio.sfx.bossDeath, Audio.sfxHighPriority * Environment.sfxMasterVolume);
                Vector3 pos = transform.position;
                GameManager.bossDefeated = true;
                StartCoroutine(Environment.AddDelay(.5F, delegate
                {
                    DialogueHandler.StartDialogue();
                }));
                Destroy(gameObject, 1);
            }
        }
    }

    /// <summary>
    /// This method is called when the boss is hit by a player shot.
    /// </summary>
    /// <param name="data">Data provided by the shot</param>
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
                            // When the boss' hp is only 1/5th of the healthbar, play a slightly different damage sound.
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage1, Audio.sfxLowPriority * Environment.sfxMasterVolume);
                            StartCoroutine(Environment.AddDelay(.1F,
                            delegate
                            {
                                playDamageSound = true;
                            }));
                        }
                        else
                        {
                            // Otherwise, play the default damage sound
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage0, Audio.sfxLowPriority * Environment.sfxMasterVolume);
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
                    // Else, if the spellcard only makes up a fraction of the health bar, it reduces damage by 1/24.
                    bossHealth = Mathf.Clamp(bossHealth - (data.damage / 24F), healthTriggerPoint, maxHealth);

                    if (playDamageSound)
                    {
                        if (bossHealth <= (healthTriggerPoint) + (previousTriggerPoint - healthTriggerPoint) / 5F)
                        {
                            // When the boss' hp is only 1/5 of the healthbar with the reduced top health accounted for, play a slightly different damage sound.
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage1, Audio.sfxLowPriority * Environment.sfxMasterVolume);
                            StartCoroutine(Environment.AddDelay(.1F,
                            delegate
                            {
                                playDamageSound = true;
                            }));
                        }
                        else
                        {
                            // Otherwise, play the default damage sound.
                            playDamageSound = false;
                            Environment.PlaySound(Audio.sfx.damage0, Audio.sfxLowPriority * Environment.sfxMasterVolume);
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
                // Else if the Attack Stage is a normal attack, no damage reduction is accounted for.
                bossHealth = Mathf.Clamp(bossHealth - data.damage, healthTriggerPoint, maxHealth);

                if (playDamageSound)
                {
                    if (bossHealth <= (healthTriggerPoint) + ((maxHealth - healthTriggerPoint) / 5F))
                    {
                        // When the boss' hp is only 1/5 of the healthbar with the reduced bottom health accounted for, play a slightly different damage sound.
                        playDamageSound = false;
                        Environment.PlaySound(Audio.sfx.damage1, Audio.sfxLowPriority * Environment.sfxMasterVolume);
                        StartCoroutine(Environment.AddDelay(.1F,
                        delegate
                        {
                            playDamageSound = true;
                        }));
                    }
                    else
                    {
                        // Otherwise, play the default damage sound.
                        playDamageSound = false;
                        Environment.PlaySound(Audio.sfx.damage0, Audio.sfxLowPriority * Environment.sfxMasterVolume);
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
                // If the boss' health is 0, move to the next attack stage
                bossHealth = maxHealth;
                MoveToNextAttackStage();
            }
            else if (bossHealth == healthTriggerPoint && !spellcardActive)
            {
                // Same thing except that if the hp hits the trigger hp
                bossHealth = healthTriggerPoint;
                MoveToNextAttackStage();
            }
        }
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
