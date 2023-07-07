using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ChapterChange : MonoBehaviour
{
    /// <summary>
    /// animation des levelloader
    /// </summary>
    public Animator transition;

    /// <summary>
    /// zeit in welcher die transition abspielt
    /// </summary>
    public float transitionTime = 1f;

    /// <summary>
    /// erster text vom ersten kapitel
    /// </summary>
    [SerializeField] GameObject firstAscentText;
    /// <summary>
    /// zweiter text vom ersten kapitel
    /// </summary>
    [SerializeField] GameObject secondAscentText;

    /// <summary>
    /// erster text vom zweiten kapitel
    /// </summary>
    [SerializeField] GameObject firstHollowText;
    /// <summary>
    /// zweiter text vom zweiten kapitel
    /// </summary>
    [SerializeField] GameObject secondHollowText;

    /// <summary>
    /// aktiviert die ersten schriften und deaktiviert die zweiten, startet transition animation und lädt eine szene
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    public IEnumerator LoadLevel(string levelIndex)
    {
        firstAscentText.SetActive(false);

        secondAscentText.SetActive(false);

        firstHollowText.SetActive(true);

        secondHollowText.SetActive(true);

        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
