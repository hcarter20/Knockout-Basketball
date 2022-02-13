using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    // The rigidbody attached to this game object
    public Rigidbody rb;

    // The NPCMovement script attached to this game object
    public Component moveScript;

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
        if (moveScript != null)
        {
            // NPC should stop moving (TODO: Is this the best approach?)
            Destroy(moveScript);

            // TODO: Should the NPC disappear after a while?

            // Notify the GameManager that an opponent has been KO'ed
            GameManager.S.OpponentHit();

            // Tell the ball that it got a KO
            return true;
        }

        // If we've already been knocked out before, return false
        return false;
    }
}