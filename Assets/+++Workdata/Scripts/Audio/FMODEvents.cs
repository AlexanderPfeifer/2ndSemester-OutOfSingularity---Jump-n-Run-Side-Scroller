using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    /// <summary>
    /// EventReference den floor der am ende des ersten chapters kaputt geht
    /// </summary>
    [field: Header("PlayerSFX")]
    [field: SerializeField] public EventReference breakFloor { get; private set; }
    /// <summary>
    /// EventReference für den charge attack sound effect.
    /// </summary>
    [field: SerializeField] public EventReference chargedAttack { get; private set; }
    /// <summary>
    /// EventReference für den Stein den man am anfang zerstört als attack tutorial.
    /// </summary>
    [field: SerializeField] public EventReference stoneDestroyed { get; private set; }
    /// <summary>
    /// EventReference für den dritten attack im combo attack.
    /// </summary>
    [field: SerializeField] public EventReference thirdAttack { get; private set; }
    /// <summary>
    /// EventReferences für die ersten beiden attacks, diese sind random von circa 5 sound effects.
    /// </summary>
    [field: SerializeField] public EventReference firstAndSecondAttack { get; private set; }
    /// <summary>
    /// EventReference für die footsteps vom Spieler. Da werden so ungefähr 20 verschiedene sounds geshuffled.
    /// </summary>
    [field: SerializeField] public EventReference footSteps { get; private set; }

    /// <summary>
    /// EventReference für den Wind oben auf dem Berg im ersten chapter.
    /// </summary>
    [field: Header("EnvironmentSFX")]
    [field: SerializeField] public EventReference windOnTopOfMountain { get; private set; }

    /// <summary>
    /// EventReference für die ambience in der cave.
    /// </summary>
    [field: SerializeField] public EventReference caveAmbience { get; private set; }

    /// <summary>
    /// EventReference für die main menu music.
    /// </summary>
    [field: Header("MusicSFX")]
    [field: SerializeField] public EventReference mainMenuMusic { get; private set; }
    /// <summary>
    /// EventReference für die cave music.
    /// </summary>
    [field: SerializeField] public EventReference caveMusic { get; private set; }

    [field: SerializeField] public EventReference battleMusic { get; private set; }

    /// <summary>
    /// EventReference den death des bosses im dritten chapter.
    /// </summary>
    [field: Header("EnemySFX")]
    [field: SerializeField] public EventReference bossDeath { get; private set; }
    /// <summary>
    /// EventReference für die attacks der enemies.
    /// </summary>
    [field: SerializeField] public EventReference enemyAttack { get; private set; }
    /// <summary>
    /// Eine Variable um von anderen scripts auf FMODEvents zuzugreifen.
    /// </summary>
    public static FMODEvents instance { get; private set; }

    /// <summary>
    /// In Awake setze ich instance gleich mit dem FMODEvents Script.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }
}
