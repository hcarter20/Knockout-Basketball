using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    // The rigidbody attached to this game object
    public Rigidbody rb;

    // How long the ball should stick around after thrown
    public float lifespanTimer = 3.0f; // arbitrary default value

    void Start()
    {
        // Try to find components if necessary
        if (rb == null)
        {
            Debug.LogError("You forgot to add the ball's rigidbody.");
            rb = GetComponent<Rigidbody>();
        }
    }

    public void Throw(Vector3 throwVector)
    {
        // Determine the throw velocity to hit the target position
        // Vector3 throwVector = CalculateForceVector(transform.position, targetPosition, throwForce);
        // Debug.Log("Throw Vector from " + transform.position + " to " + targetPosition + " is " + throwVector);
        // Vector3 targetDir = targetPosition - transform.position;
        // float angle = Vector3.Angle(throwVector, transform.forward);
        // Debug.Log(angle);

        // Re-activate physics (this also re-activates gravity)
        rb.isKinematic = false;

        // Apply physics force to the ball
        rb.AddRelativeForce(throwVector, ForceMode.VelocityChange);

        // Remove control of the game object from player
        transform.parent = null;

        // Start countdown timer for self-destruct
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        // Wait out the timer
        yield return new WaitForSeconds(lifespanTimer);

        // TODO: Animation? Sound effect?

        // Destroy this game object
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        // Tell the player to spawn a new ball
        PlayerControl.player.SpawnBall();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hoop"))
        {
            // GOAL! Tell the GameManager we scored
            GameManager.S.PlayerScored();
        }
    }
}