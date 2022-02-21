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

    // Any force below this is just a stun
    public float stunForceCutoff = 8.0f;
    // Any force above this is an explosion
    public float explosiveForceCutoff = 13.0f;

    // Position when ball is initial thrown
    private Vector3 positionWhenThrown;
    private float forceWhenThrown;

    // Prefab of an explosion, triggered when thrown hard enough
    public GameObject explosionPrefab;
    // The radius of effect for the explosive force
    public float explosionRadius = 1.0f; // Arbitrary default value
    public float explosionPower = 1.0f;  // Arbitrary default value
    public float explosionLift = 1.0f;   // Arbitrary default value

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

        // Apply just a little bit of angular momentum, so it appears to rotate
        rb.angularVelocity = new Vector3(1.0f, 1.0f, 1.0f);

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
        if (audioManagement.instance != null)
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
                GameManager.S.PlayerScored(other.gameObject.transform.position, positionWhenThrown, false);

                if (audioManagement.instance != null)
                {
                    audioManagement.instance.Play("net");
                    audioManagement.instance.Play("cheer");
                }
            }
            else if (other.gameObject.CompareTag("WrongHoop"))
            {
                // GOAL! Tell the GameManager we scored, and our location at the start
                GameManager.S.PlayerScored(other.gameObject.transform.position, positionWhenThrown, true);

                if (audioManagement.instance != null)
                {
                    audioManagement.instance.Play("net");
                    // TODO: Audience boos you?
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore any collisions while still in the player's hands
        if (!isThrown)
            return;

        if (collision.gameObject.CompareTag("Opponent"))
        {
            // Tell the NPC that it's been hit
            NPCController hitNPC = collision.gameObject.GetComponent<NPCController>();

            // If we hit the NPC too gently, then only stun
            if (forceWhenThrown < stunForceCutoff)
            {
                // If we stun an NPC, then destroy this ball
                if (hitNPC.Stun())
                {
                    // Play stun hit sound effect(s)
                    if (audioManagement.instance != null)
                    {
                        audioManagement.instance.Play("hitNPC1");
                        audioManagement.instance.Play("hitNPC2");
                    }

                    // Destroy the ball immediately
                    SelfDestroy();
                }
            }
            else
            {
                // If we knock out an NPC, then destroy this ball
                if (hitNPC.KnockOut())
                {
                    if (audioManagement.instance != null)
                    {
                        audioManagement.instance.Play("hitNPC1");
                        audioManagement.instance.Play("hitNPC2");
                        int r = Mathf.FloorToInt(Random.Range(1, 5.9f));
                        audioManagement.instance.Play(r.ToString());
                    }

                    // If we hit the NPC hard enough, trigger an explosion
                    if (forceWhenThrown > explosiveForceCutoff)
                    {
                        //egchan explodesound
                        audioManagement.instance.Play("explode");
                        // Instantiate the explosion particle effect
                        GameObject explosion = Instantiate(explosionPrefab, transform);
                        explosion.transform.parent = null;

                        // Find all opponents within the explosion radius
                        Collider[] colliders = Physics.OverlapSphere(explosion.transform.position, explosionRadius);

                        // Apply explosive force to them and knock them out
                        foreach (Collider npcCollider in colliders)
                        {
                            // Check if we can knock this NPC out
                            NPCController npc = npcCollider.gameObject.GetComponent<NPCController>();
                            if (npc != null)
                            {
                                // Apply the explosive force to this NPC & knock out
                                npc.KnockOut(); // Since this isn't a conditional, explosion force applies to "corpses"
                                npc.rb.AddExplosionForce(explosionPower, explosion.transform.position, explosionRadius + 2.0f, explosionLift, ForceMode.Impulse);
                            }
                        }
                    }

                    // Destroy the ball immediately
                    SelfDestroy();
                }
            }
        }
    }
}