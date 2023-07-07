using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour
{

    /// <summary>
    /// moving speed der platform
    /// </summary>
    public float movingSpeed;
    /// <summary>
    /// bool für ob enemies spawnen können
    /// </summary>
    public bool canSpawn = false;
    /// <summary>
    /// script vom enemy spawner behaviour
    /// </summary>
    [SerializeField] EnemySpawnerBehaviour enemySpawnerBehaviour;

    [SerializeField] ParallaxEffect parallaxEffect;
    /// <summary>
    /// GameObject welches aktiviert wird wenn der Spieler auf der platform steht, aktiviert dann sowas wie invisible collider, eine andere kamera etc.
    /// </summary>
    [SerializeField] GameObject activeFight;
    /// <summary>
    /// animator der platform
    /// </summary>
    Animator anim;

    /// <summary>
    /// kamera des spielers
    /// </summary>
    public CinemachineVirtualCamera playerVcam, battlefieldVcam;

    /// <summary>
    /// staret die position beim ersten punkt und bestimmt animator component
    /// </summary>
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// wenn platform betreten wird, kann sich die platform bewegen, enemies spawnen und es wird eine kamera aktiviert mit collidern was den spieler in der platform bleiben lässt
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerVcam.Priority = 0;
            battlefieldVcam.Priority = 10;
            AudioManager.instance.InitializeMusic(FMODEvents.instance.battleMusic);
            activeFight.SetActive(true);
            canSpawn = true;
            anim.SetBool("Moving", true);
            enemySpawnerBehaviour.waveId = 1;
            enemySpawnerBehaviour.WaveBehaviour();
            parallaxEffect.onPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            enemySpawnerBehaviour.waveId += 1;
            enemySpawnerBehaviour.WaveBehaviour();
        }
    }
}
