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

    public bool KnockOut() {
        // If we haven't been hit before, moveScript should still be active
        if (moveScript.movementEnabled)
        {
            // NPC should stop moving (TODO: Is this the best approach?)
            moveScript.movementEnabled = false;

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
        // Wait to respawn for a while
        yield return new WaitForSeconds(respawnTime);

        // Reset the position of this npc
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, 0.75f, currentPosition.z);
        transform.rotation = Quaternion.identity;

        // Re-enable the movement script
        moveScript.movementEnabled = true;
    }
}