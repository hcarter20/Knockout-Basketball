using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Used by other methods to check what part of gameplay we're in
public enum GameState { setup, playing, victory, gameOver, development };

public class GameManager : MonoBehaviour
{
    // GameManager holds static reference to itself so all other scripts can access
    public static GameManager S;
    public GameState gameState = GameState.setup;

    // Gameplay variables (time, score, total basketballs left, etc.)
    public int totalTime = 90; // Total time for a single shot (in seconds)
    public float timeLeft;
    private float secondCountdown;
    public int totalBalls = 30; // Total number of basketballs at start
    public int ballsLeft;
    public int score; // Player's score in points
    public int koCount; // TODO: How to use this?
    public int npcsTouching;
    //egchan UI cue penalty
    // public bool flashing; // For now, let's leave it all red when penalized

    // UI Elements which display the gameplay variables
    public Text scoreText, timerText, ballText, scoreReport;
    public string endScreenName;
    public GameObject penaltyHighlight;

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
        // gameState = GameState.setup;

        // Check for UI variables
        if (timerText == null || scoreText == null || ballText == null)
            Debug.LogError("Forgot to include the UI text in GameManager.");

        // Initialize gameplay variables
        score = 0;
        koCount = 0;
        npcsTouching = 0;

        // Set the remaining variables
        timeLeft = totalTime;
        secondCountdown = 1.0f;
        ballsLeft = totalBalls;

        // Update the UI with initial gameplay values
        timerText.text = Mathf.FloorToInt(timeLeft / 60).ToString("0") + ":" + Mathf.FloorToInt(timeLeft % 60).ToString("00");
        ballText.text = ballsLeft.ToString("0");
        // Update score, just in case
        scoreText.text = score.ToString("0");

        // Start playing the game
        if (gameState != GameState.development)
            gameState = GameState.playing;
    }

    private void Update()
    {
        if (gameState == GameState.playing)
        {
            // Turn on the highlight if the player is being touched by NPC
            penaltyHighlight.SetActive(npcsTouching > 0);

            // TODO: Not the most elegant way to do this
            if (timeLeft > 0.0f)
            {
                // Decrement the second timer
                secondCountdown -= Time.deltaTime + (npcsTouching * 0.01f);
                if (secondCountdown <= 0.0f)
                {
                    // Decrease the timeLeft value
                    timeLeft -= 1.0f - secondCountdown;
                    // Reset the second countdown
                    secondCountdown = 1.0f;

                    // Play tick sound for second decrement
                    if (audioManagement.instance != null)
                        audioManagement.instance.Play("tick");
                }
            }

            if (timeLeft <= 0.0f)
            {
                // Trigger a game over when time runs out
                timerText.text = "0:00";
                StartCoroutine(GameOver());
            }
            else
            {
                timerText.text = Mathf.FloorToInt(timeLeft / 60).ToString("0") + ":" + Mathf.FloorToInt(timeLeft % 60).ToString("00");
            }
        }
    }

    /* When the player throws a basketball */
    public void BallThrown()
    {
        if (gameState == GameState.development)
            return;

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

    /* When the player gets the ball through the hoop */
    public void PlayerScored(Vector3 hoopPosition, Vector3 positionWhenThrown, bool wrong)
    {
        // Get the player's score for shot, based on distance from hoop
        float distance = Vector3.Distance(positionWhenThrown, hoopPosition);

        int points = 1;
        if (distance > 12.5f)
            points = 3;
        else if (distance > 7.0f)
            points = 2;

        // Get negative points if you scored in the wrong hoop
        if (wrong)
            score -= points;
        else
            score += points;

        // Update the score value in the UI
        scoreText.text = score.ToString("0");
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

        // Play the buzzer sound to indicate game is over
        if (audioManagement.instance != null)
            audioManagement.instance.Play("buzzer");

        // Destroy the player's ball, if one exists
        Destroy(PlayerControl.player.currentBall);

        // Wait for a couple seconds
        yield return new WaitForSeconds(2.0f); //egchan Changing time to 2, 3 is a bit long

        // Crowd cheer at the end
        if (audioManagement.instance != null)
            audioManagement.instance.Play("crowd");
        
        // TODO: Switch to the end scene instead
        SceneManager.LoadScene(endScreenName);

        // TODO: Update end screen values
        if (score == 1)
            scoreReport.text = "You scored 1 point! Nice Job!";
        else if (score == 0)
            scoreReport.text = "You scored no points. Nice Try!"; //egchan added
        else if (score == -1) 
            scoreReport.text = "You scored -1 point? Nice Work, I guess.";
        else if (score < 0)
            scoreReport.text = "You scored " + score + " points? Nice Work, I guess.";
        else
            scoreReport.text = "You scored " + score + " points! Nice Job!";
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
