using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages stage midbosses and bosses.
/// </summary>
public class BossHandler : MonoBehaviour {

    // Variable Declaration

    /// <summary> The boss' health. </summary>
    public float bossHealth;

    /// <summary> True if the parent is a midboss. </summary>
    public bool midboss = false;

    /// <summary> List of all attacks of the boss, including non-spell card attacks. </summary>
    public List<AttackStage> AttackTable = new List<AttackStage>();

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
    }

    public void MoveToNextAttackStage()
    {
        StopBulletLoop();
        AttackTable.Remove(AttackTable[0]);
        IEnumerator smallDelay = Environment.AddDelay(3);
        TriggerAttack();
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
