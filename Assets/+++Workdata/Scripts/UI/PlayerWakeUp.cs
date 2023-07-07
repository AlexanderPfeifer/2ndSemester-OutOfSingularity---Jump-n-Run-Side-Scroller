using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWakeUp : MonoBehaviour
{
    /// <summary>
    /// script vom player
    /// </summary>
    public PlayerScript playerScript;

    /// <summary>
    /// Starts animation of player waking up
    /// </summary>
    public void PlayerWakeUpAnimation()
    {
        playerScript.CallAnimationAction(16);
    }
}
