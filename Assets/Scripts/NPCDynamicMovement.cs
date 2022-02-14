using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDynamicMovement : MonoBehaviour
{
    // How quickly the NPC moves
    public float moveSpeed = 2.0f; // Arbitrary default value

    // How close does the NPC try to get to the player?
    public float goalDist = 2.0f; // Arbitrary default value

    // Should the NPC try to approach the player?
    public bool approachPlayer = true;
    public bool atPlayer = false;

    public bool movementEnabled = true;

    private void LateUpdate()
    {
        if (movementEnabled)
        {
            // Turn to look at the player at all times?
            transform.rotation = Quaternion.Euler(0f, PlayerControl.player.transform.rotation.eulerAngles.y, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (movementEnabled)
        {// Are we trying to get near the player?
            if (approachPlayer /* && PlayerControl.player.isMoving */ && GameManager.S.gameState == GameState.playing)
            {
                // Get the current position of the player
                Vector3 playerPosition = PlayerControl.player.transform.position;

                float dist = Vector3.Distance(transform.position, playerPosition);

                // Are we within our goal distance from the player?
                if (dist < goalDist)
                {
                    if (!atPlayer)
                    {
                        // Affect the player in negative way.
                        atPlayer = true;
                        GameManager.S.npcsTouching++;
                    }
                }
                else
                {
                    if (atPlayer)
                    {
                        // Stop affecting the player
                        atPlayer = false;
                        GameManager.S.npcsTouching--;
                    }
                    // Move closer to the player
                    transform.position = Vector3.MoveTowards(transform.position, playerPosition, moveSpeed * Time.deltaTime);
                }

                // Turn to face the player

            }
        }
    }
}
