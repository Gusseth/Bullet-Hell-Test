using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all Debugging controls and tools.
/// </summary>
public class DebugHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Environment.debugMode)
        {
            if (Input.GetKeyDown(KeyCode.Plus))
            {
                Environment.playerHandler.power++;
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                Environment.playerHandler.power--;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameObject.Find("Eika Ebisu").GetComponent<BossHandler>().TriggerAttack();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject.Find("Eika Ebisu").GetComponent<BossHandler>().StopBulletLoop();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                GameObject.Find("Eika Ebisu").GetComponent<BossHandler>().bossHealth = GameObject.Find("Eika Ebisu").GetComponent<BossHandler>().healthTriggerPoint;
                GameObject.Find("Eika Ebisu").GetComponent<BossHandler>().MoveToNextAttackStage();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool isInvincible = Environment.playerHandler.isInvincible;
                if (isInvincible)
                {
                    Environment.playerHandler.isInvincible = false;
                }
                else
                {
                    Environment.playerHandler.isInvincible = true;
                }
            }
        }
    }
}
