using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that enforces culling. Deletes this GameObject once it has passed the technical (backend) boundary.
/// </summary>
public class DestroyOnBorder : MonoBehaviour
{
    /// <summary>
    /// The sole function of this script. Checks any trigger exit event if it's the technical boundary. If true, then delete the GameObject.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Environment.cullBoundary)
        {
            Destroy(gameObject);
        }
    }
}
