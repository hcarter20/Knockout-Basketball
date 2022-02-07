using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Used below for choosing ball position, relative to camera
    public enum ShootHeight { High, Mid, Low };

    // Only ever one player in a scene, so static access from other classes
    public static PlayerControl player;

    // Mouse settings
    public float mouseSensitivity = 5.0f;

    // Used to control player movement
    public CharacterController controller;
    public float moveSpeed = 10.0f; // Arbitrary default value
    public float rotateSpeed = 3.0f; // Arbitrary default value

    // Used for throwing the ball
    public GameObject ballPrefab;
    public Vector3 lowThrowVector = new Vector3(0.0f, 1.0f, 4.0f); // Arbitrary default value
    public Vector3 midThrowVector = new Vector3(0.0f, 1.0f, 1.0f); // Arbitrary default value
    public Vector3 highThrowVector = new Vector3(0.0f, 2.0f, 1.0f); // Arbitrary default value
    private GameObject currentBall;
    // Set when the ball is about to be thrown
    private bool throwing = false;
    // Speed of the power meter (angle of the ball)
    public float meterSpeed = 1.0f; // Arbitrary default value
    public float throwSpeed = 8.0f; // Arbitrary default value
    public float minThrowSpeed = 5.0f; // Arbitrary default value
    public float maxThrowSpeed = 8.0f; // Arbitrary default value
    public bool lower = true;

    // Position of the ball relative to player camera based on mode
    // These are default values, otherwise customize through the editor
    public Vector3 lowPosition = new Vector3(0.0f, -0.8f, 1f);
    public Vector3 midPosition = new Vector3(0.0f, -0.4f, 1.5f);
    public Vector3 highPosition = new Vector3(0.0f, 0.5f, 1.7f);

    // Current ball position, represented as a mode (choice of target)
    public ShootHeight currentHeight;

    // Start is called before the first frame update
    void Start()
    {
        player = this;

        // Try to find the CharacterController if necessary
        if (controller == null)
        {
            Debug.LogError("You forgot to add the player's character controller.");
            controller = GetComponent<CharacterController>();
        }

        // Spawn the ball manually at the start
        SpawnBall();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Consider moving to FixedUpdate for performance?

        // Update the player's movement, based on keyboard input
        UpdateMovement();

        // Check ball positioning input
        if (currentBall != null)
            UpdateBall();
    }

    private void UpdateMovement()
    {
        // Move the character based on keyboard input
        // Get the player's movement input (through the keyboard)
        float xMove = Input.GetAxis("Horizontal") * moveSpeed;
        float zMove = Input.GetAxis("Vertical") * moveSpeed;

        // Create the motion vector, clamp to ensure diagonal isn't faster
        Vector3 playerMove = new Vector3(xMove, 0.0f, zMove);
        playerMove = Vector3.ClampMagnitude(playerMove, moveSpeed);

        // Use the CharacterController to move the player
        controller.Move(playerMove * Time.deltaTime);

        // TODO: Camera rotation (limit rotation?)
        float yRotation = Input.GetAxis("Turn") * rotateSpeed;
        transform.localRotation *= Quaternion.Euler(0f, yRotation, 0f);
    }

    private void FixedUpdate()
    {
        if (throwing)
        {
            // Deactivate the throwing flag
            throwing = false;

            // Throw the basketball by calling its script
            Throwable throwScript = currentBall.GetComponent<Throwable>();

            // TODO: based on accuracy of power meter (somehow)
            Vector3 throwVector = Vector3.zero;

            if (currentHeight == ShootHeight.High)
                throwVector = highThrowVector;
            else if (currentHeight == ShootHeight.Mid)
                throwVector = midThrowVector;
            else if (currentHeight == ShootHeight.Low)
                throwVector = lowThrowVector;

            throwScript.Throw(throwVector.normalized * throwSpeed);

            // This ball will prompt us to spawn a new one when it destroys itself
            currentBall = null;
        }
    }

    private void UpdateBall()
    {
        // TODO: Convert this to a power meter
        // Update the current throw power
        float goalThrowSpeed = lower ? minThrowSpeed : maxThrowSpeed;

        if (Mathf.Abs(goalThrowSpeed - throwSpeed) < 0.1f)
            lower = !lower;
        else
            throwSpeed = Mathf.MoveTowards(throwSpeed, goalThrowSpeed, meterSpeed * Time.deltaTime);

        // Check if the player wants to throw the ball
        if (Input.GetButtonDown("Fire"))
            throwing = true;
        else
        {
            // Get input on the arrows keys to indicate position swap
            // float vertArrow = Input.GetAxisRaw("VerticalAlt");
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentHeight == ShootHeight.High)
                {
                    currentHeight = ShootHeight.Mid;
                    currentBall.transform.localPosition = midPosition;
                }
                else  if (currentHeight == ShootHeight.Mid)
                {
                    currentHeight = ShootHeight.Low;
                    currentBall.transform.localPosition = lowPosition;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentHeight == ShootHeight.Low)
                {
                    currentHeight = ShootHeight.Mid;
                    currentBall.transform.localPosition = midPosition;
                }
                else if (currentHeight == ShootHeight.Mid)
                {
                    currentHeight = ShootHeight.High;
                    currentBall.transform.localPosition = highPosition;
                }
            }

            // TODO: Do we want to allow horizontal positioning?
            //float horizArrow = Input.GetAxisRaw("HorizontalAlt");
        }
    }

    public void SpawnBall()
    {
        // First, instantiate a new instance from the prefab
        currentBall = Instantiate(ballPrefab, transform);

        // By default, start out of view (bottom of the screen)
        currentHeight = ShootHeight.Low;
        currentBall.transform.localPosition = lowPosition;
    }

    public void Teleport(GameObject teammate)
    {
        // Get the new position of the player
        Vector3 teamPosition = teammate.transform.position;
        Vector3 currPosition = gameObject.transform.position;
        Quaternion currRotation = gameObject.transform.rotation;

        // Destroy the teammate object (so we don't teleport on top)
        Destroy(teammate);

        // Disable character controller for teleport
        controller.enabled = false;

        // Set our new position to replace the teammate
        transform.SetPositionAndRotation(new Vector3(teamPosition.x, currPosition.y, teamPosition.z), currRotation);

        // Re-enable character controller after teleport
        controller.enabled = true;
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
