using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialManager : MonoBehaviour
{
    // Holds the UI text object which displays instructions
    public Text popUpText;

    // How long do we pause when moving to the next instruction?
    public float pauseTime;

    // The list of text instructions for each instruction step
    private readonly string[] instrText = 
        {
            "Move with WASD",
            "Rotate with left and right arrows",
            "Angle shot with up and down arrows",
            "Hold spacebar to charge throw",
            "Release spacebar to throw"
        };

    // Which control are we currently introducing
    private int popUpIndex;
    // Used to transition between instructions with delay
    private bool switching;


    private void Start()
    {
        // Start with wasd movement controls
        popUpIndex = 0;
        popUpText.text = instrText[0];
        switching = false;
    }

    private void Update() {
        // If we're switching, then player has already used the correct controls
        if (!switching)
        {
            // Check if the player is using the controls we indicate
            bool usedControl = (popUpIndex == 0 && (Input.GetAxisRaw("Vertical") != 0.0f || Input.GetAxisRaw("Horizontal") != 0.0f))
                || (popUpIndex == 1 && Input.GetAxisRaw("HorizontalAlt") != 0.0f)
                || (popUpIndex == 2 && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow)))
                || (popUpIndex == 3 && Input.GetButtonDown("Fire"))
                || (popUpIndex == 4 && Input.GetButtonUp("Fire"));
            
            if (usedControl)
            {
                // Move on to the next instruction
                switching = true;
                StartCoroutine(NextStep());
            }
        }
    }

    private IEnumerator NextStep()
    {
        // TODO: Play audio cue? Signal success?

        // Wait for a moment
        yield return new WaitForSeconds(pauseTime);

        // Move on to the next instruction
        switching = false;
        popUpIndex++;

        if (popUpIndex < instrText.Length)
            popUpText.text = instrText[popUpIndex];
        else
        {
            // TODO: How to end the tutorial?
            popUpText.text = "";
        }
    }
}
