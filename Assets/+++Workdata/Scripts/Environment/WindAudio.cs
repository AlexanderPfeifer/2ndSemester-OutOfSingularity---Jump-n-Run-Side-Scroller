using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]

public class WindAudio : MonoBehaviour
{
    /// <summary>
    /// emitter für den wind auf dem berg
    /// </summary>
    private StudioEventEmitter emitter;

    /// <summary>
    /// initialisiert den sound auf dem berg
    /// </summary>
    private void Start()
    {
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.windOnTopOfMountain, this.gameObject);
    }

    /// <summary>
    /// startet den sound wenn der spieler den bereicht betritt
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        emitter.Play();
    }

    /// <summary>
    /// stoppt sound wenn bereich verlassen wird
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            emitter.Stop();
    }
}
