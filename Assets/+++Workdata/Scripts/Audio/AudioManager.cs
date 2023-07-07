using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Float mit range 0 bis 1 f�r den master volume Slider in den Options.
    /// </summary>
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    /// <summary>
    /// Float mit range 0 bis 1 f�r den music volume Slider in den Options.
    /// </summary>
    [Range(0, 1)]
    public float musicVolume = 1;
    /// <summary>
    /// Float mit range 0 bis 1 f�r den ambience volume Slider in den Options.
    /// </summary>
    [Range(0, 1)]
    public float ambienceVolume = 1;
    /// <summary>
    /// Float mit range 0 bis 1 f�r den sfx volume Slider in den Options.
    /// </summary>
    [Range(0, 1)]
    public float SFXVolume = 1;

    /// <summary>
    /// bus der alle sounds des Spiels enth�lt:
    /// </summary>
    private Bus masterBus;
    /// <summary>
    /// bus der die musik sounds des Spiels enth�lt:
    /// </summary>
    private Bus musicBus;
    /// <summary>
    /// bus der die ambience sounds des Spiels enth�lt:
    /// </summary>
    private Bus ambienceBus;
    /// <summary>
    /// bus der die die sfx des Spiels enth�lt:
    /// </summary>
    private Bus sfxBus;

    /// <summary>
    /// Eine Variable um von anderen scripts auf AudioManager zuzugreifen.
    /// </summary>
    public static AudioManager instance { get; private set; }

    /// <summary>
    /// Eine List f�r alle eventInstances 
    /// </summary>
    private List<EventInstance> eventInstances;
    /// <summary>
    /// Eine List f�r alle eventEmitters
    /// </summary>
    private List<StudioEventEmitter> eventEmitters;

    /// <summary>
    /// Eine EventInstance f�r die Musik im Main Menu
    /// </summary>
    private EventInstance musicEventInstance;

    /// <summary>
    /// in Awake setze ich instance gleich mit AudioManager, erstelle zwei neue Listen und setze jeden bus gleich mit den zugeh�rigen busses in FMOD
    /// </summary>
    private void Awake()
    {
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    /// <summary>
    /// in Update setze das volume von jedem bus gleich mit den erstellen float variablen der slider
    /// </summary>
    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    /// <summary>
    /// In dieser Methode erstelle ich eine musik instanz welche ich der methode mitgebe und direkt starte
    /// </summary>
    /// <param name="musicEventReference"></param>
    public void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    /// <summary>
    /// Hier kann man einen Parameter der Musik im main menu ver�ndern indem man parameter name und value mit gibt
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    public void SetMusicParameter(string parameterName, float parameterValue)
    {
        musicEventInstance.setParameterByName(parameterName, parameterValue);
    }

    /// <summary>
    /// Hier gebe ich der methode einen sound und eine position von der dieser sound gespielt werden soll mit. Diesen Sound lasse ich dann abspielen.
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="worldPos"></param>
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    /// <summary>
    /// Mit dieser Methode erstelle f�ge ich der erstellten List einen Sound hinzu und gebe den Wert wieder zur�ck
    /// </summary>
    /// <param name="eventReference"></param>
    /// <returns></returns>
    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    /// <summary>
    /// Hier bekomme ich einen StudioEventEmitter von einem mitgegebenem GameObject und f�ge eine mitgegebene eventreference zu der emitter liste hinzu
    /// </summary>
    /// <param name="eventReference"></param>
    /// <param name="emitterGameObject"></param>
    /// <returns></returns>
    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }


    /// <summary>
    /// in CleanUp gehe ich alle evenetInstances und emitter in den jeweiligen Listen durch und stoppe diese.
    /// </summary>
    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    /// <summary>
    /// in OnDestroy wird CleanUp ausgef�hrt damit keine Sounds mehr aus der vorherigen Szene gespielt werden
    /// </summary>
    private void OnDestroy()
    {
        CleanUp();
    }
}
