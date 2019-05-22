using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This Monobehaviour script manages the health bar on the top of the screen.
/// </summary>
public class BossHealthHandler : MonoBehaviour
{

    public BossHandler bossHandler;

    public Slider healthSlider;

// Public Functions and Methods

    public void UpdateBossHandler()
    {
        bossHandler = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateBossHandler();
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossHandler.isDead)
        {
            // If the boss is not defeated yet, map out the health
            healthSlider.value = Environment.Map(bossHandler.bossHealth, 0, bossHandler.maxHealth, 0, 1);
        }
        else
        {
            // Else if the boss is defeated, lock the value at 0.
            healthSlider.value = 0;
        }
    }
}
