using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                return;
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                Environment.playerHandler.power--;
                return;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHandler>().TriggerAttack();
                return;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHandler>().StopBulletLoop();
                return;
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHandler>().bossHealth = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHandler>().healthTriggerPoint;
                GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHandler>().MoveToNextAttackStage();
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool isInvincible = Environment.playerHandler.isInvincible;
                if (isInvincible)
                {
                    Environment.playerHandler.isInvincible = false;
                    return;
                }
                else
                {
                    Environment.playerHandler.isInvincible = true;
                    return;
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                // Resets the scene
                Environment.lockInput = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
        }
    }
}
