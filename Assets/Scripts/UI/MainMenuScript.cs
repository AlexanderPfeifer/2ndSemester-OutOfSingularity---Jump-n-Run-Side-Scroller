using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private string spaceShipLevel = "SpaceShipTutorial";
    private string options = "OptionsMenuScene";
    private string mainMenu = "MainMenuScene";
    private string credits = "CreditScene";

    public void PlayGame()
    {
        SceneManager.LoadScene(spaceShipLevel);
    }

    public void OptionsMenu()
    {
        SceneManager.LoadScene(options);
    }

    public void CreditsMenu()
    {
        SceneManager.LoadScene(credits);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
