using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    // The target position for this hoop
    public GameObject hoopObject;

    // The radius of effect to try and pull balls in
    public float pullRadius;
    public float pullPower = 1.0f; // Arbitrary default value

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Throwable ball = other.gameObject.GetComponent<Throwable>();
            if (ball != null && ball.rb != null && !ball.rb.isKinematic)
            {
                // Apply a one-off reverse-explosive force, which pulls the ball towards the inside of the hoop
                ball.rb.AddExplosionForce(-pullPower, hoopObject.transform.position, pullRadius);
            }
        }
    }
}
