using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {   
    
    public static bool GamePaused = false;
    public static PauseMenu menu;
    
    public GameObject PauseScreen;
    public GameObject ControlScreen;
    public GameObject SettingScreen;

    private void Awake()
    {
        // Prevent the UI from being destroyed when we reload the scene
        if (menu == null)
            menu = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            } 
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
        GamePaused = false;
    }


    void Pause()
    {
        PauseScreen.SetActive(true);
        Time.timeScale = 0;
        GamePaused = true;
    }

    public void ControlLoad()
    {
        PauseScreen.SetActive(false);
        ControlScreen.SetActive(true);
    }

    public void ControlReturn()
    {
        ControlScreen.SetActive(false);
        PauseScreen.SetActive(true);
    }

    public void SettingLoad()
    {
        PauseScreen.SetActive(false);
        SettingScreen.SetActive(true);
    }

    public void SettingReturn()
    {
        SettingScreen.SetActive(false);
        PauseScreen.SetActive(true);
    }

    public void ReturnMenu()
    {
        Time.timeScale = 1;
        GamePaused = false;
        SceneManager.LoadScene("TitleScreen");
    }
}
