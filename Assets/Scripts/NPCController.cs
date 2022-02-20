using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    // The rigidbody attached to this game object
    public Rigidbody rb;

    // The NPCMovement script attached to this game object
    public NPCDynamicMovement moveScript;

    // The amount of time in seconds it takes for an NPC to reset when defeated
    public float respawnTime = 45.0f;
    public float stunTime = 2.0f;

    // Used to switch the npc's material between KO and default
    public MeshRenderer render;
    public Material defaultMat, koMat;

    // Used to allow a knock out when stunned
    private bool stunned = false;

    void Start()
    {
        // Try to find components if necessary
        if (rb == null)
        {
            Debug.LogError("You forgot to add the npc's rigidbody.");
            rb = GetComponent<Rigidbody>();
        }
        if (moveScript == null)
        {
            Debug.LogError("You forgot to add the npc's move script.");
        }
    }

    public bool Stun()
    {
        // If we are currently able to move, then not ko-ed or stunned
        if (moveScript.movementEnabled)
        {
            // Indicate that we're stunned
            stunned = true;

            // NPC should stop moving
            moveScript.movementEnabled = false;

            // Cancel the forces applied by the ball to the rigidbody
            rb.isKinematic = true;

            // Reset the position of this npc (to cancel falling over)
            Vector3 currentPosition = transform.position;
            transform.SetPositionAndRotation(new Vector3(currentPosition.x, 0.75f, currentPosition.z), Quaternion.identity);

            // Start preparing to move again
            StartCoroutine(StartMoving());

            // Tell the ball that it stunned us
            return true;
        }

        // Tell the ball we weren't stunned
        return false;
    }

    public IEnumerator StartMoving()
    {
        // Wait to start moving after the stun
        yield return new WaitForSeconds(stunTime);

        // Stop being stunned
        stunned = false;

        // Re-enable the rigidbody physics
        rb.isKinematic = false;

        // Re-enable the movement script
        moveScript.movementEnabled = true;
    }

    public bool KnockOut() {
        // If we haven't been hit before, moveScript should still be active
        if (moveScript.movementEnabled || stunned)
        {
            // Cancel the stun and transition to a knock out
            if (stunned)
            {
                // Cancel the stunned state on this npc
                stunned = false; 
                rb.isKinematic = false;
                StopCoroutine(StartMoving());
            }

            // NPC should stop moving (TODO: Is this the best approach?)
            moveScript.movementEnabled = false;

            if (moveScript.atPlayer)
            {
                // Stop affecting the player
                moveScript.atPlayer = false;
                GameManager.S.npcsTouching--;
            }

            // Notify the GameManager that an opponent has been KO'ed
            GameManager.S.OpponentHit();

            // Start preparing to respawn
            StartCoroutine(Respawn());

            // Tell the ball that it got a KO
            return true;
        }

        // If we've already been knocked out before, return false
        return false;
    }

    public IEnumerator Respawn()
    {
        // Switch to the knocked-out material
        render.material = koMat;

        // Wait to respawn for a while
        yield return new WaitForSeconds(respawnTime);

        // Reset the position of this npc
        Vector3 currentPosition = transform.position;
        transform.SetPositionAndRotation(new Vector3(currentPosition.x, 0.75f, currentPosition.z), Quaternion.identity);

        // Make sure we reset to our default material
        render.material = defaultMat;

        // Re-enable the movement script
        moveScript.movementEnabled = true;
    }
}