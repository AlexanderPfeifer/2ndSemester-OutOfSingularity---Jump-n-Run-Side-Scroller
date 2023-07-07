using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondChapterBehaviour : MonoBehaviour
{
    /// <summary>
    /// player script variable
    /// </summary>
    [SerializeField] PlayerScript playerScript;
    [SerializeField] HealthbarScript healthbarScript;
    [SerializeField] Animator healUIAnim;

    /// <summary>
    /// startet music und ambiente des zweiten chapters
    /// </summary>
    public void SecondChapterStart()
    {
        AudioManager.instance.InitializeMusic(FMODEvents.instance.caveAmbience);
        AudioManager.instance.InitializeMusic(FMODEvents.instance.caveMusic);
        playerScript.currentHealthPlayer = 10;
        healthbarScript.SetHealth(playerScript.currentHealthPlayer);
    }

    /// <summary>
    /// startet wake up animation des spielers
    /// </summary>
    public void PlayerWakeUp()
    {
        playerScript.CallAnimationAction(16);
    }
}
