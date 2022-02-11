using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
     public GameObject Settings;

    public void PlayGame () 
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame ()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void SettingsLoad ()
    {
        Settings.SetActive(true);
    }

    public void SettingsReturn ()
    {
        Settings.SetActive(false);
    }
}
