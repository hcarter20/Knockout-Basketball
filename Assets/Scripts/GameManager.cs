using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Used by other methods to check what part of gameplay we're in
public enum GameState { setup, playing, victory, gameOver };

public class GameManager : MonoBehaviour
{
    // GameManager holds static reference to itself so all other scripts can access
    public static GameManager S;
    public GameState gameState;

    // Gameplay variables (time, score, total basketballs left, etc.)
    public int totalTime = 90; // Total time for a single shot (in seconds)
    public float timeLeft;
    public int totalBalls = 30; // Total number of basketballs at start
    public int ballsLeft;
    public int score; // Player's score in points
    public int round; // How many times the player has scored
    public int koCount; // TODO: How to use this?
    public int npcsTouching;
    //egchan UI cue penalty
    public bool flashing;

    // UI Elements which display the gameplay variables
    public Text scoreText, timerText, ballText, scoreReport;
    public GameObject endScreen;
    public GameObject penaltyHighlight;

    // Position of the hoop, for determining score of a throw
    public GameObject hoop;

    private void Awake()
    {
        // Ensures there is exactly 1 game manager across all scene reloads
        if (S == null)
            S = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Start in the setup phase
        gameState = GameState.setup;

        // Check for UI variables
        if (timerText == null || scoreText == null || ballText == null)
            Debug.LogError("Forgot to include the UI text in GameManager.");

        // Initialize gameplay variables
        score = 0;
        koCount = 0;
        npcsTouching = 0;

        // Set the remaining variables
        StartRound();
    }

    private void StartRound()
    {
        // Initialize gameplay variables based on the current round

        // TODO: Set the total time: decrease by 5 sec each round
        timeLeft = totalTime - (round * 5.0f);

        // TODO: Reduce total number of basketballs by 1 each round
        ballsLeft = totalBalls - round;

        // Update the UI with initial gameplay values
        timerText.text = "0:" + Mathf.FloorToInt(timeLeft % 60).ToString();
        ballText.text = ballsLeft.ToString("0");
        // Update score, just in case
        scoreText.text = score.ToString("0");

        // Start playing the game
        gameState = GameState.playing;
    }

    private void Update()
    {
        if (gameState == GameState.playing)
        {
            // TODO: Not the most elegant way to do this
            if (timeLeft > 0.0f)
            {
                // Continually decrement the timer
                timeLeft -= Time.deltaTime + (npcsTouching * 0.01f);
                StartCoroutine(Penalty()); //egchan, update penalty visual
            }

            if (timeLeft <= 0.0f)
            {
                audioManagement.instance.Play("buzzer");
                // Trigger a game over when time runs out
                StartCoroutine(GameOver());
            }
            else
            {
                //audioManagement.instance.Play("tick");
                timerText.text = Mathf.FloorToInt(timeLeft / 60).ToString("0") + ":" + Mathf.FloorToInt(timeLeft % 60).ToString("00");
            }
        }
    }

    /* When the player throws a basketball */
    public void BallThrown()
    {
        // Decrease the total number of balls left
        ballsLeft--;

        // Update the UI to reflect the decrease
        ballText.text = ballsLeft.ToString("0");
    }

    public void OpponentHit()
    {
        // egchan KO count
        koCount += 1;

        // UI element not currently implemented
        // koAmount.text = totalKO.ToString("0");
    }

    public IEnumerator Penalty() //egchan penalty marker on clock
    {
        if (npcsTouching > 0)
        {
            flashing = true;
            //Debug.LogError("watch out for the kids!");
        }
        else if (npcsTouching == 0)
        {
            flashing = false;
            //Debug.LogError("no threat at the moment");
        }

        if (flashing == true)
        {
            penaltyHighlight.SetActive(true);
            yield return new WaitForSeconds(.5f);
            penaltyHighlight.SetActive(false);
            yield return new WaitForSeconds(.5f);
        }
    }

    /* When the player gets the ball through the hoop */
    public void PlayerScored(Vector3 positionWhenThrown)
    {
        // Update the round counter
        round++;

        // Add to the player's total score, based on distance from hoop
        float distance = Vector3.Distance(positionWhenThrown, hoop.transform.position);

        int points = 1;
        if (distance > 12.5f)
            points = 3;
        else if (distance > 7.0f)
            points = 2;

        // TODO: Apply basketball score rules
        score += points;

        // Update the score value in the UI
        scoreText.text = score.ToString("0");
        // scoreReport.text = "You scored " + score.ToString("0") + " points! Nice Job!";
        // Debug.Log("You just scored " + 1 + " points! Your new total score is " + score + ".");

        // Celebrate the basket, then reset the scene
        // StartCoroutine(Celebrate());
    }

    public IEnumerator Celebrate()
    {
        // Switch to the victory state
        gameState = GameState.victory;
        
        // TODO: Play a victory sound effect?
        // TODO: Put up UI image?

        // Wait for a couple seconds
        yield return new WaitForSeconds(3.0f);

        // Reset the field for the next round
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Reset the gameplay variables for the new round
        // StartRound();
    }

    public IEnumerator GameOver()
    {
        // Switch to the game over state
        gameState = GameState.gameOver;

        // Wait for a couple seconds
        yield return new WaitForSeconds(2.0f); //egchan Changing time to 2, 3 is a bit long

        // Crowd cheer at the end
        audioManagement.instance.Play("crowd");
        

        // Destroy the player's ball, if one exists
        Destroy(PlayerControl.player.currentBall);

        // TODO: Activate the end screen
        endScreen.SetActive(true);

        // TODO: Update end screen values
        if (score == 1)
            scoreReport.text = "You scored 1 point! Nice Job!";
        else if (score == 0)
            scoreReport.text = "You scored no points. Nice Try!"; //egchan added
        else
            scoreReport.text = "You scored " + score + " points! Nice Job!";
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
