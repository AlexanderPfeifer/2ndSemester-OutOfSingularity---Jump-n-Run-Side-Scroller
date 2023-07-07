using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    /// <summary>
    /// unity event für wenn der trigger betreten wird
    /// </summary>
    public UnityEvent onTriggerEnter;
    /// <summary>
    /// unity event für wenn der trigger verlassen wird
    /// </summary>
    public UnityEvent onTriggerExit;

    /// <summary>
    /// tag des objekt welches den trigger betritt
    /// </summary>
    public string triggerTag;

    /// <summary>
    /// startet unity event beim betreten
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(triggerTag))
        {
            onTriggerEnter?.Invoke();
        }
    }

    /// <summary>
    /// startet unity event beim verlassen
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(triggerTag))
        {
            onTriggerExit?.Invoke();
        }
    }
}
