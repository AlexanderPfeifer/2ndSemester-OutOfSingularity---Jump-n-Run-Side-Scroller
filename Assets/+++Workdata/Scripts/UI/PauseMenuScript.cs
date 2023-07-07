using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    /// <summary>
    /// static bool um auf den pause state des spiels zu zugreifen
    /// </summary>
    public static bool gameIsPaused = false;

    /// <summary>
    /// ingame pause menu
    /// </summary>
    public GameObject pauseMenuUI;

    /// <summary>
    /// ui mit healthbar etc
    /// </summary>
    public GameObject UICanvas;

    /// <summary>
    /// ingame optionsmenu
    /// </summary>
    public GameObject OptionsCanvas;

    /// <summary>
    /// pausiert das spiel bei button click
    /// </summary>
    /// <param name="context"></param>
    public void PauseGame(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// lässt das spiel nach dem pausieren weiter laufen
    /// </summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        OptionsCanvas.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        UICanvas.SetActive(true);
    }

    /// <summary>
    /// pausiert das spiel
    /// </summary>
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        UICanvas.SetActive(false);
    }

    /// <summary>
    /// aktiviert ingame options menu
    /// </summary>
    public void Options()
    {
        OptionsCanvas.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    /// <summary>
    /// verlässt das spiel
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
