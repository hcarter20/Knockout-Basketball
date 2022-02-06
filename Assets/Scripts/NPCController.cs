using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    // The rigidbody attached to this game object
    public Rigidbody rb;

    // The NPCMovement script attached to this game object
    public NPCMovement moveScript;

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
            Debug.LogError("You forgot to add the npc's movement script.");
            moveScript = GetComponent<NPCMovement>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // NPC should stop moving (TODO: Is this the best approach?)
            Destroy(moveScript);

            // TODO: Should the ball disappear, or at least not be able to pass to teammate?
            // Or is it funnier if you can pass to a teammate by bouncing off an NPC?
            Destroy(collision.gameObject);

            // TODO: Should the NPC disappear after a while?

        }
    }
}
