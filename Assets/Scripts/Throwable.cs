using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    // The rigidbody attached to this game object
    public Rigidbody rb;

    // How long the ball should stick around after thrown
    public float lifespanTimer = 5.0f; // arbitrary default value

    // Prevent ball from acting while still held
    public bool isThrown = false;

    public Vector3 positionWhenThrown;

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

        // Save our current position, for scoring
        positionWhenThrown = transform.position;

        // Apply physics force to the ball
        rb.AddRelativeForce(throwVector, ForceMode.VelocityChange);

        // Remove control of the game object from player
        transform.parent = null;

        // In air behavior now applies
        isThrown = true;

        // Start countdown timer for self-destruct
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        // Wait out the timer
        yield return new WaitForSeconds(lifespanTimer);

        // In a separate method, so we can avoid the timer if necessary
        SelfDestroy();
    }

    public void SelfDestroy()
    {
        // TODO: Animation? Sound effect?

        // Destroy this game object
        Destroy(gameObject);
        FindObjectOfType<audioManagement>().Play("ballExplode");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isThrown)
        {
            if (other.gameObject.CompareTag("Hoop"))
            {
                // GOAL! Tell the GameManager we scored, and our location at the start
                GameManager.S.PlayerScored(positionWhenThrown);
                FindObjectOfType<audioManagement>().Play("net");
                FindObjectOfType<audioManagement>().Play("cheer");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
        {
            if (collision.gameObject.CompareTag("Opponent"))
            {
                // Tell the NPC that it's been hit
                NPCController npc = collision.gameObject.GetComponent<NPCController>();

                // If we knock out an NPC, then destroy this ball
                if (npc.KnockOut())
                {
                    // Destroy the ball immediately
                    SelfDestroy();
                    FindObjectOfType<audioManagement>().Play("hitNPC1");
                    FindObjectOfType<audioManagement>().Play("hitNPC2");
                    int r = random.Next(1,5);
                    FindObjectOfType<audioManagement>().Play(r.ToString());
                }
            }
        }
    }
}