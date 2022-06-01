using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

   


    // Update is called once per frame
    void Update()
    {
          if (Input.GetKeyDown(KeyCode.Escape))
          {
            if (GameIsPaused)
            {
              Resume();
            } else
            {
              Pause();
            }
          }
    }

    public void Resume()
    {
        SceneManager.LoadScene("BoardMaker");
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        SceneManager.LoadScene("PauseMenu");
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
      Time.timeScale = 1f;
      SceneManager.LoadScene("Menu");
    }


}
