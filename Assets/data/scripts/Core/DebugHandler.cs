using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains all Debugging controls and tools.
/// </summary>
public class DebugHandler : MonoBehaviour
{
    public string debugDialogueMessage;

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
                bool isInvincible = PlayerHandler.isInvincible;
                if (isInvincible)
                {
                    PlayerHandler.isInvincible = false;
                    return;
                }
                else
                {
                    PlayerHandler.isInvincible = true;
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
            if (Input.GetKeyDown(KeyCode.L))
            {
                string msg = debugDialogueMessage;
                if (msg == "")
                {
                    msg = Random.Range(int.MinValue, int.MaxValue).ToString();
                }
                Environment.dialogueHandler.RunDialogue(new Dialogue((DialogueEvent.Speaker)Mathf.RoundToInt(Random.Range(1, 3)), (Dialogue.Face)Mathf.RoundToInt(Random.Range(0, 8)), msg));
                return;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                Environment.EndDialogue();
                return;
            }
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                Environment.PlayBGM(Audio.bgm.stg01b);
            }
        }
    }
}
