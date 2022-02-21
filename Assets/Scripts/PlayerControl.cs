using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    // Used below for choosing ball position, relative to camera
    public enum ShootHeight { High, Low };

    // Only ever one player in a scene, so static access from other classes
    public static PlayerControl player;

    // Used for animating the player's arms
    public GameObject lowArmModel, highArmModel;
    public Animator lowArmAnimator, highArmAnimator;

    // Used to control player movement
    public CharacterController controller;
    public float moveSpeed = 10.0f; // Arbitrary default value
    public float rotateSpeed = 1.0f; // Arbitrary default value

    // Used for throwing the ball
    public GameObject ballPrefab;
    public Vector3 lowThrowVector = new Vector3(0.0f, 1.0f, 4.0f); // Arbitrary default value
    public Vector3 highThrowVector = new Vector3(0.0f, 2.0f, 1.0f); // Arbitrary default value
    public GameObject currentBall;
    private GameObject thrownBall;
    // Set when the ball is about to be thrown (meter moving)
    public bool isThrowing = false;
    // Speed of the power meter (angle of the ball)
    public float meterSpeed = 1.0f; // Arbitrary default value
    public float throwSpeed; // Arbitrary default value
    public float minThrowSpeed = 7.0f; // Arbitrary default value
    public float maxThrowSpeed = 14.0f; // Arbitrary default value
    public float maxPowerTimeTotal = 1.0f;
    private float maxPowerTime;

    // Position of the ball relative to player camera based on mode
    // These are default values, otherwise customize through the editor
    public Vector3 lowPosition = new Vector3(0.0f, -0.8f, 1f);
    public Vector3 highPosition = new Vector3(0.0f, 0.5f, 1.7f);

    // Current ball position, represented as a mode (choice of target)
    public ShootHeight currentHeight = ShootHeight.Low; // Start at the bottom of the screen (out of view)

    // Used by other NPC's to check if the player is moving
    public bool isMoving;

    // Keeps track of how many NPC's are currently touching
    public int npcsTouching = 0;

    private void Awake()
    {
        player = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial state
        isThrowing = false;
        isMoving = false;
        currentHeight = ShootHeight.Low;
        lowArmModel.SetActive(true);
        highArmModel.SetActive(false);

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
        if (GameManager.S.gameState != GameState.gameOver)
        {
            // Update the player's movement, based on keyboard input
            UpdateMovement();

            // Can only throw the ball during gameplay?
            if (GameManager.S.gameState == GameState.playing || GameManager.S.gameState == GameState.development)
            {
                // When player isn't holding ball, check if need to spawn one
                if (currentBall == null)
                {
                    // If player starts to throw when ball isn't present,
                    // automatically destroy previous ball (and therefore create a new one)
                    if (Input.GetButtonDown("Fire") || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        // Experimental: try leaving the previous ball
                        // Destroy the previously thrown ball
                        // thrownBall.GetComponent<Throwable>().SelfDestroy();

                        // Spawn a new ball immediately
                        SpawnBall();
                    }
                    // If the thrown ball is also destroyed already, spawn a new one automatically
                    else if (thrownBall == null)
                        SpawnBall();
                }

                // Update the current ball's position/throw power, or throw the ball
                // (Separate if statement - should trigger same frame as above)
                if (currentBall != null)
                    UpdateBall();
            }
        }
    }

    private void UpdateMovement()
    {
        // Move the character based on keyboard input
        // Get the player's movement input (through the keyboard)
        float xMove = Input.GetAxis("Horizontal") * moveSpeed;
        float zMove = Input.GetAxis("Vertical") * moveSpeed;

        // Create the motion vector, clamp to ensure diagonal isn't faster
        Vector3 playerMove = new Vector3(xMove, 0.0f, zMove);
        playerMove = transform.TransformDirection(playerMove);
        playerMove = Vector3.ClampMagnitude(playerMove, moveSpeed);

        // Apply gravity manually
        if (!controller.isGrounded)
            playerMove.y = -9.8f;

        if (playerMove != Vector3.zero)
        {
            // Note that the player is moving
            isMoving = true;

            // Use the CharacterController to move the player (SimpleMove prevents flying)
            controller.Move(playerMove * Time.deltaTime);
            //audioManagement.instance.Pause("squeak");
        }
        else
        {
            // Note that the player isn't moving
            isMoving = false;
            if (audioManagement.instance != null)
                audioManagement.instance.Play("squeak"); //egchan sound
        }

        // TODO: Camera rotation (limit rotation?)
        float yRotation = Input.GetAxisRaw("HorizontalAlt") * rotateSpeed * Time.deltaTime;
        transform.localRotation *= Quaternion.Euler(0f, yRotation, 0f);
    }

    private void UpdateBall()
    {
        // Check if the fire button was first pushed this frame
        if (Input.GetButtonDown("Fire"))
        {
            // Set the throwing flag (for below)
            isThrowing = true;

            // Initialize the meter at minimum power
            throwSpeed = minThrowSpeed;
        }
        // Check if the fire button was first released this frame
        else if (Input.GetButtonUp("Fire"))
        {
            // The throwing flag should always be set if we try to throw
            if (!isThrowing)
                Debug.LogError("Attempting to throw the ball without throwing flag set.");

            // Throw the ball
            ThrowBall();

            // Deactivate the throwing flag
            isThrowing = false;

            // Reset the meter to minimum power
            throwSpeed = minThrowSpeed;
        }

        // If currently trying holding down button to throw, then update the power
        if (isThrowing)
        {
            // Reset the meter back to the bottom when reach the max
            if (Mathf.Abs(maxThrowSpeed - throwSpeed) < 0.1f)
            {
                // Stay near max power for a little bit before resetting
                maxPowerTime -= Time.deltaTime;
                if (maxPowerTime < 0.0f)
                {
                    maxPowerTime = maxPowerTimeTotal;
                    throwSpeed = minThrowSpeed;
                }
            }
            else
                // Otherwise, update the throw speed
                throwSpeed = Mathf.MoveTowards(throwSpeed, maxThrowSpeed, meterSpeed * Time.deltaTime);
        }
       
        // Player can adjust the height of the ball while charging up
        if (currentBall != null)
        {
            // Get input on the arrows keys to indicate position swap
            // float vertArrow = Input.GetAxisRaw("VerticalAlt");

            if (Input.GetKeyDown(KeyCode.DownArrow) && currentHeight == ShootHeight.High)
            {
                currentHeight = ShootHeight.Low;
                currentBall.transform.localPosition = lowPosition;
                lowArmModel.SetActive(true);
                highArmModel.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && currentHeight == ShootHeight.Low)
            {
                currentHeight = ShootHeight.High;
                currentBall.transform.localPosition = highPosition;
                lowArmModel.SetActive(false);
                highArmModel.SetActive(true);
            }
        }
    }

    public void ThrowBall()
    {
        // Throw the basketball by calling its script
        Throwable throwScript = currentBall.GetComponent<Throwable>();

        // TODO: based on accuracy of power meter (somehow)
        Vector3 throwVector = Vector3.zero;

        if (currentHeight == ShootHeight.High)
            throwVector = highThrowVector;
        else if (currentHeight == ShootHeight.Low)
            throwVector = lowThrowVector;

        throwScript.Throw(throwVector.normalized * throwSpeed);

        // Trigger the throwing animation
        if (currentHeight == ShootHeight.High)
        {
            highArmAnimator.ResetTrigger("Throw");
            highArmAnimator.SetTrigger("Throw");
        }
        else if (currentHeight == ShootHeight.Low)
        {
            lowArmAnimator.ResetTrigger("Throw");
            lowArmAnimator.SetTrigger("Throw");
        }

        // This ball will prompt us to spawn a new one when it destroys itself
        thrownBall = currentBall;
        currentBall = null;

        // Tell the GameManager that we've used one of our basketballs
        GameManager.S.BallThrown();
    }

    public void SpawnBall()
    {
        // Check if there is a ball left from the game manager
        if (GameManager.S.ballsLeft <= 0)
        {
            // To prevent the method from double-calling the game over state
            if (GameManager.S.gameState == GameState.playing)
            {
                // Trigger a game over when player runs out of basketballs
                StartCoroutine(GameManager.S.GameOver());
            }
        }
        else
        {
            // First, instantiate a new instance from the prefab
            currentBall = Instantiate(ballPrefab, transform);

            // When spawn a new ball, maintain the same height as the previous throw
            if (currentHeight == ShootHeight.Low)
                currentBall.transform.localPosition = lowPosition;
            else if (currentHeight == ShootHeight.High)
                currentBall.transform.localPosition = highPosition;
            else
                Debug.LogError("Can't position ball at undefined height.");
        }
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
}
