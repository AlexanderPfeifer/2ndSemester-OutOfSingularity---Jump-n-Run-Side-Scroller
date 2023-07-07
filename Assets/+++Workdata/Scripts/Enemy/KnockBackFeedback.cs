using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KnockBackFeedback : MonoBehaviour
{
    /// <summary>
    /// Der RigidBody von dem GameObject welches KnockBack bekommt
    /// </summary>
    [SerializeField] private Rigidbody2D rb;
    /// <summary>
    /// St�rke und Zeit welche vergehen soll bis der n�chste knockback eintreffen darf
    /// </summary>
    [SerializeField] private float strength, delay;
    /// <summary>
    /// UnityEvents f�r wenn der Knockback angefangen hat und aufgeh�rt hat
    /// </summary>
    public UnityEvent OnBegin, OnDone;

    /// <summary>
    /// Hier gebe ich ein GameObject mit von welchem der knockback aus kommt. Dann Stoppe ich alle Coroutines, Invoke OnBegin UnityEvent, bestimme die direction von der 
    /// aus der Schlag kommt, gebe den knockback dann an den RigidBody und starte die reset coroutine 
    /// </summary>
    /// <param name="sender"></param>
    public void PlayFeedback(GameObject sender)
    {
        StopAllCoroutines();
        OnBegin?.Invoke();
        Vector2 direction = (transform.position - sender.transform.position).normalized;
        rb.AddForce(direction * strength, ForceMode2D.Impulse);
        StartCoroutine(Reset());
    }

    /// <summary>
    /// In Reset wird kurz ein delay abgewartet und dann wird die velocity dem rb auf null gesetzt damit diese nicht zu weit fliegt und dann wird das unity event ondone gestartet
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero;
        OnDone?.Invoke();
    }
}
