using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    // Used to keep track of which teammate this is
    public int id;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // TODO: Catch the ball? For now destroy it
            Destroy(collision.gameObject);

            // Player has successively passed the ball to teammate
            PlayerControl.player.Teleport(gameObject);

            // Tell the GameManager to activate the next row
            GameManager.S.BeginNextSection(id);
        }
    }
}
