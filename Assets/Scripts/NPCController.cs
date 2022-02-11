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

    //egchan trying to count knocked NPCs
    [SerializeField] Text koAmount;
    public int koed = 0;

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

    public void Collide(GameObject collisionObject) {
        // NPC should stop moving (TODO: Is this the best approach?)
        Destroy(moveScript);

        // TODO: Should the ball disappear, or at least not be able to pass to teammate?
        // Or is it funnier if you can pass to a teammate by bouncing off an NPC?
        Destroy(collisionObject);

        // TODO: Should the NPC disappear after a while?

        // Notify the GameManager that an opponent has been KO'ed
        GameManager.S.OpponentHit();
    }

    /* egchan : take score value to ui */
    private void updateKoUI()
    {
        koAmount.text = koed.ToString("0");
    }
}