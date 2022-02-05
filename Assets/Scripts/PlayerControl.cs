using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Mouse settings
    public float mouseSensitivity = 5.0f;

    // Used to control player movement
    public CharacterController controller;
    public float moveSpeed = 10.0f; // Arbitrary default value

    // Used for throwing the ball
    public GameObject ballPrefab;
    public Vector3 throwVector = new Vector3(0, 10, 20); // Arbitrary default value
    public GameObject currentBall;
    private bool throwing = false;

    // Start is called before the first frame update
    void Start()
    {
        // Try to find the CharacterController if necessary
        if (controller == null)
        {
            Debug.LogError("You forgot to add the player's character controller.");
            controller = GetComponent<CharacterController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Consider shifting the actual movement to FixedUpdate (for physics interactions)
        // Move the character based on keyboard input
        // Get the player's movement input (through the keyboard)
        float xMove = Input.GetAxis("Horizontal") * moveSpeed;
        float zMove = Input.GetAxis("Vertical") * moveSpeed;

        // Create the motion vector, clamp to ensure diagonal isn't faster
        Vector3 playerMove = new Vector3(xMove, 0.0f, zMove);
        playerMove = Vector3.ClampMagnitude(playerMove, moveSpeed);

        // Use the CharacterController to move the player
        controller.Move(playerMove * Time.deltaTime);

        // Check if we need a new basketball
        if (currentBall == null)
        {
            currentBall = Instantiate(ballPrefab, transform);
        }
        // Check if the player wants to throw the ball
        else if (Input.GetButtonDown("Fire"))
        {
            throwing = true;
        }
    }

    private void FixedUpdate()
    {
        if (throwing)
        {
            // Deactivate the throwing flag
            throwing = false;

            // Throw the basketball by calling its script
            Throwable throwScript = currentBall.GetComponent<Throwable>();
            throwScript.Throw(throwVector);

            // currentBall == null when this ball destroys itself
        }
    }

    // TODO: When the player clicks on a part of the screen, use raycast to get direction,
    // then spawn a basketball and apply force to send it in that direction.

    // When basketball collides with an NPC, it knocks them over, cardboard cutout style.

    // TODO: Player is able to move right/left, forward/back, and the camera follows them.
    // Also allow for rotation?
    // Control the camera with the mouse, or with the keyboard? Or both?

    // TODO: Does your cursor position determine the camera's rotation,
    // or do you control that independently with the keyboard?
    // With keyboard is possibly more complex, but allows for more freedom of throwing.
    // Otherwise, will always throw at center of screen, with keyboard prompt or click?
}
