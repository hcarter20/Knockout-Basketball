using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    // The rigidbody attached to this game object
    public Rigidbody rb;

    void Start()
    {
        // Try to find the Rigidbody component if necessary
        if (rb == null)
        {
            Debug.LogError("You forgot to add the npc's rigidbody.");
            rb = GetComponent<Rigidbody>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {

        }
    }
}
