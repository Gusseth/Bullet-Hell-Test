using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour script that manages the hitbox assist ring when the player focusses.
/// </summary>
public class HitboxAssistHandler : MonoBehaviour {

    /// <summary> Rotation delta, base rotation value. </summary>
    private Vector3 rotation;

    // This method is called every time the GameObject is activated, times where the player is focussing
    void OnEnable()
    {
        rotation = new Vector3(0, 0, 10);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {

        // Decelerating effect when the player just starts focussing
        if (rotation.z > 1F)
        {
            rotation.z = rotation.z - 0.4F;
        }

        // Perform the rotation
        transform.Rotate(rotation); // "Top" Hitbox Assist, rotates counterclockwise
        transform.GetChild(0).Rotate(rotation * -2); // "Bottom" Hitbox Assist, rotates clockwise at twice the rate of the bsae to counteract the top's existing rotation.
	}
}
