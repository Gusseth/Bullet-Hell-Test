using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // Variable Declaration

    // General Movement
    public float rawSpeed;                           // Base player speed
    float speed;                                     // Actual player speed
    public float xSpeed = 0;                         // Movement in the x-axis
    public float ySpeed = 0;                         // Movement in the y-axis

    // Controls
    KeyCode forward;                                 // Forward control
    KeyCode backward;                                // Backward control
    KeyCode left;                                    // Left control
    KeyCode right;                                   // Right control
    KeyCode focus;                                   // Focus button


    // Functions and other crap below

    /// <summary>
    /// [Deprecated] Class struct that assigns the controls.
    /// </summary>
    public PlayerMovement(KeyCode forwardButton = KeyCode.UpArrow, KeyCode backwardButton = KeyCode.DownArrow, KeyCode leftButton = KeyCode.LeftArrow, KeyCode rightButton = KeyCode.RightArrow, KeyCode focusButton = KeyCode.LeftShift)
    {
        forward = forwardButton;
        backward = backwardButton;
        left = leftButton;
        right = rightButton;
        focus = focusButton;
    }

    /// <summary>
    /// Overloaded empty class struct for debug purposes.
    /// </summary>
    public PlayerMovement()
    { 
        forward = KeyCode.UpArrow;
        backward = KeyCode.DownArrow;
        left = KeyCode.LeftArrow;
        right = KeyCode.RightArrow;
        focus = KeyCode.LeftShift;
    }

    private void Start()
    {
        rawSpeed = Environment.playerHandler.rawSpeed;
        speed = rawSpeed;

        // Debug controls below, remove later
        forward = KeyCode.UpArrow;
        backward = KeyCode.DownArrow;
        left = KeyCode.LeftArrow;
        right = KeyCode.RightArrow;
        focus = KeyCode.LeftShift;
    }

    // Function below is refreshed every frame

    private void FixedUpdate()
    {

        // Player's speed in the stated axes

        float xSpeed = Input.GetAxisRaw("Horizontal");
        float ySpeed = Input.GetAxisRaw("Vertical");

        if (!Environment.lockInput)
        {
            if (Input.GetKey(focus))
            {
                // When the focus button is pressed down

                speed = rawSpeed / 2F;
                Environment.playerHandler.isFocused = true;
                transform.Find("HitboxAssist").gameObject.SetActive(true);
            }
            else
            {
                speed = rawSpeed;
                Environment.playerHandler.isFocused = false;
                transform.Find("HitboxAssist").gameObject.SetActive(false);
            }

            // Translating the player's position

            if (xSpeed != 0 || ySpeed != 0)
            {
                if (Mathf.Abs(xSpeed) + Mathf.Abs(ySpeed) > 1)
                {
                    // If input is present in both axes, speed is split so that the player doesn't move faster when moving diagonally

                    float diagonalSpeed = speed * .75F;
                    transform.Translate(xSpeed * diagonalSpeed * Time.deltaTime, ySpeed * diagonalSpeed * Time.deltaTime, 0);
                    transform.position = Environment.ClampPositionToCamera(transform.position);
                    return;
                }

                //  Else, the raw values are inserted.
                transform.Translate(xSpeed * speed * Time.deltaTime, ySpeed * speed * Time.deltaTime, 0);
                transform.position = Environment.ClampPositionToCamera(transform.position);
            }

        }

        // Updates the animation
        GetComponent<Animator>().SetInteger("horizontalSpeed", (int)xSpeed);
    }
}
