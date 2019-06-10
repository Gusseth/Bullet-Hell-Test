using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is immediately run after Environment is up and running. This is where the rest of the assets are loaded.
/// </summary>
public sealed class EnvironmentPostInit : MonoBehaviour {

    void Start()
    {

        // Loads sound effects into memory
        Environment.sfx.AddRange(Resources.LoadAll<AudioClip>("sfx"));
        Environment.sfx.AsReadOnly(); // Forces the sfx list to be read-only.

        // Loads items into memory
        Environment.itemList.AddRange(Resources.LoadAll<GameObject>("gameObjects/Items"));
        Environment.itemList.AsReadOnly();
    }
}
