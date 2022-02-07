using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager holds static reference to itself so all other scripts can access
    public static GameManager S;

    // TODO: Gameplay variables (time, score, total basketballs left, etc.)

    // Maintains access to all NPC's in the scene (set in Editor)
    public GameObject[] defenders1, defenders2, defenders3;

    // Section represents how far the player is from the hoop,
    // and how many points they get for a successful shot at this distance.
    public int section = 3;
    public int score = 0;

    private void Awake()
    {
        S = this;

        if (defenders1 == null || defenders2 == null || defenders3 == null)
        {
            Debug.Log("You forgot to set the defenders in the GameManger.");
        }
    }

    void Start()
    {
        // Initialize gameplay variables


        // Start the first row of defenders
        ActivateRow(defenders1, null);
    }

    /* When the player gets the ball through the hoop */
    public void PlayerScored()
    {
        // Add to the player's total score, based on section
        score += section;

        Debug.Log("You just scored " + section + " points! Your new total score is " + score + ".");
    }

    /* When a player teleports to their teammate's position */
    public void BeginNextSection(int teamId)
    {
        if (teamId == 2)
        {
            section = 2;
            ActivateRow(defenders2, defenders1);
        }
        else if (teamId == 1)
        {
            section = 1;
            ActivateRow(defenders3, defenders2);
        }
        else
        {
            Debug.LogError("Player has begun section below Section 1.");
        }
    }

    private void ActivateRow(GameObject[] activeDefenders, GameObject[] disableDefenders)
    {
        // Activate the given set of defenders
        if (activeDefenders != null)
        {   
            foreach (GameObject defender in activeDefenders)
            {
                // Defender may have been destroyed by a ball
                if (defender != null)
                {
                    NPCMovement move = defender.GetComponent<NPCMovement>();
                    if (move != null)
                        move.enabled = true;
                }
            }
        }

        // Disable the given set of defenders
        if (disableDefenders != null)
        {
            foreach (GameObject defender in disableDefenders)
            {
                // Defender may have been destroyed by a ball
                if (defender != null)
                {
                    // TODO: Is this the correct approach?
                    // Also, make sure this doesn't trigger any sort of points/credit for player
                    Destroy(defender);
                }
            }
        }
    }
}
