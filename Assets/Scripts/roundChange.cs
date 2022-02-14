using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class roundChange : MonoBehaviour
{
    public void nextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quitToMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
