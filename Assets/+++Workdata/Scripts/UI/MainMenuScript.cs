using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    /// <summary>
    /// parameter von sound effekten 
    /// </summary>
    [SerializeField] private string parameterName;
    /// <summary>
    /// wert der parameter von sound effekten
    /// </summary>
    [SerializeField] private float parameterValue;

    /// <summary>
    /// erstes chapter 
    /// </summary>
    private string firstChapter = "1.Chapter";
    /// <summary>
    /// main menu 
    /// </summary>
    public GameObject MainMenuCanvas;
    /// <summary>
    /// options menu
    /// </summary>
    public GameObject OptionsMenuCanvas;
    /// <summary>
    /// menu f�r die credits
    /// </summary>
    public GameObject CreditsMenuCanvas;

    /// <summary>
    /// animator von dem level loader 
    /// </summary>
    public Animator transition;

    /// <summary>
    /// zeit der level loader transition
    /// </summary>
    public float transitionTime = 1f;

    /// <summary>
    /// l�dt anderes level und startet transition 
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    IEnumerator LoadLevel(string levelIndex)
    {
        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

    /// <summary>
    /// macht die musik im main menu leise
    /// </summary>
    public void DeactivateMusic()
    {
        AudioManager.instance.SetMusicParameter(parameterName, parameterValue);
    }

    /// <summary>
    /// l�dt das erste chapter
    /// </summary>
    public void PlayGame()
    {
        StartCoroutine(LoadLevel(firstChapter));
    }

    /// <summary>
    /// aktiviert options menu canvas
    /// </summary>
    public void OptionsMenu()
    {
        OptionsMenuCanvas.SetActive(true);
        CreditsMenuCanvas.SetActive(false);
        MainMenuCanvas.SetActive(false);

    }

    /// <summary>
    /// aktiviert credits menu canvas
    /// </summary>
    public void CreditsMenu()
    {
        CreditsMenuCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
        OptionsMenuCanvas.SetActive(false);

    }

    /// <summary>
    /// aktiviert main menu canvas
    /// </summary>
    public void MainMenu()
    {
        MainMenuCanvas.SetActive(true);
        CreditsMenuCanvas.SetActive(false);
        OptionsMenuCanvas.SetActive(false);
    }

    /// <summary>
    /// verl�sst das spiel
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
