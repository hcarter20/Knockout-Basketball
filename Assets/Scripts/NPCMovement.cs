using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    // Controls the NPC's movement
    // The transforms in the Unity editor, corresponding to the points
    public Transform leftTransform, rightTransform;
    // The two bound points that the npc moves between
    public Vector3 leftPoint, rightPoint;
    // Which direction it is currently going
    private bool left = true;
    // How quickly the NPC moves
    public float moveSpeed = 2.0f; // Arbitrary default value
    private float hesitanceTime;

    public void Start()
    {
        leftPoint = leftTransform.position;
        rightPoint = rightTransform.position;
    }

    private void FixedUpdate()
    {
        // Move the npc along its track
        Vector3 destPoint = left ? leftPoint : rightPoint;

        // Are we at the destination yet?
        if (transform.position == destPoint)
        {
            // Start going the other direction
            left = !left;

            // Generate a random pause timing for motion (so not all on a cycle)
            hesitanceTime = Random.Range(0.005f, 0.02f);

            // Pause for a random amount of time (fixed for npc)
            StartCoroutine(PauseMotion());

            // Continue moving towards our destination
            transform.position = Vector3.MoveTowards(transform.position, destPoint, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Continue moving towards our destination
            transform.position = Vector3.MoveTowards(transform.position, destPoint, moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator PauseMotion()
    {
        yield return new WaitForSeconds(hesitanceTime);
    }

    // Draws gizmos in the Unity editor (don't appear during gameplay)
    void OnDrawGizmos()
    {
        // To prevent error codes before we've added transforms in editor
        if (leftTransform != null && rightTransform != null)
        {
            leftPoint = leftTransform.position;
            rightPoint = rightTransform.position;

            // Draw the left and right points and a line between them
            Gizmos.DrawSphere(leftPoint, 0.1f);
            Gizmos.DrawSphere(rightPoint, 0.1f);
            Gizmos.DrawLine(leftPoint, rightPoint);

            // Gizmos.color = Color.red;
            // Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        }
        else
        {
            // Draw a big sphere at the NPC (to indicate when not yet set)
            Gizmos.DrawSphere(transform.position, 1.0f);
        }
    }
}
