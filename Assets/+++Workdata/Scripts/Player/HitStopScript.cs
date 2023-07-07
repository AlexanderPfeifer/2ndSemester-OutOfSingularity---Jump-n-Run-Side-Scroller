using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopScript : MonoBehaviour
{
    /// <summary>
    /// bool ob das spiel gefreezed ist
    /// </summary>
    bool isFreezing;

    /// <summary>
    /// freezed das spiel
    /// </summary>
    /// <param name="duration"></param>
    public void FreezeTime(float duration)
    {
        if (isFreezing)
            return;
        Time.timeScale = 0.05f;
        StartCoroutine(DelayedUnfreeze(duration));
    }

    /// <summary>
    /// unfreezed das spiel wieder
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator DelayedUnfreeze(float duration)
    {
        isFreezing = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        isFreezing = false;
    }
}
