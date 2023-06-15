using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopScript : MonoBehaviour
{
    bool isFreezing;

    public void FreezeTime(float duration)
    {
        if (isFreezing)
            return;
        Time.timeScale = 0.05f;
        StartCoroutine(DelayedUnfreeze(duration));
    }

    IEnumerator DelayedUnfreeze(float duration)
    {
        isFreezing = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        isFreezing = false;
    }
}
