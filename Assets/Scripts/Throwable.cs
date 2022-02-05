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
        // Try to find the Rigidbody component if necessary
        if (rb == null)
        {
            Debug.LogError("You forgot to add the ball's rigidbody.");
            rb = GetComponent<Rigidbody>();
        }
    }

    public void Throw(Vector3 throwVector)
    {
        // Re-activate physics
        rb.isKinematic = false;

        // Apply physics force to the ball
        //rb.AddForce(throwVector, ForceMode.VelocityChange);
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
}
