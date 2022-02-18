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

    // Position when ball is initial thrown
    public Vector3 positionWhenThrown;
    public float forceWhenThrown;

    // Prefab of an explosion, triggered when thrown hard enough
    public GameObject explosionPrefab;

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

        // Save the thrown force, for explosion check later
        forceWhenThrown = throwVector.magnitude;

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
        audioManagement.instance.Play("pop");
        // Destroy this game object
        Destroy(gameObject);
        // FindObjectOfType<audioManagement>().Play("ballExplode");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isThrown)
        {
            if (other.gameObject.CompareTag("Hoop"))
            {
                // GOAL! Tell the GameManager we scored, and our location at the start
                GameManager.S.PlayerScored(positionWhenThrown);
                audioManagement.instance.Play("net");
                audioManagement.instance.Play("cheer");
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
                    // If we hit the NPC hard enough, trigger an explosion
                    if (forceWhenThrown > 13.5f)
                    {
                        GameObject explosion = Instantiate(explosionPrefab, transform);
                        explosion.transform.parent = null;
                    }

                    if (audioManagement.instance != null)
                    {
                        audioManagement.instance.Play("hitNPC1");
                        audioManagement.instance.Play("hitNPC2");
                        int r = Mathf.FloorToInt(Random.Range(1, 5.9f));
                        audioManagement.instance.Play(r.ToString());
                    }

                    // Destroy the ball immediately
                    SelfDestroy();
                }
            }
        }
    }
}