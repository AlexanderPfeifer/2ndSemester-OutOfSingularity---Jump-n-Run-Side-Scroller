using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerBehaviour : MonoBehaviour
{
    /// <summary>
    /// linker spawnpoint der enemies
    /// </summary>
    [SerializeField] Transform leftSpawnPoint;
    /// <summary>
    /// mittlerer spawnpoint der enemies
    /// </summary>
    [SerializeField] Transform middleSpawnPoint;
    /// <summary>
    /// rechter spawnpoint der enemies
    /// </summary>
    [SerializeField] Transform rightSpawnPoint;


    /// <summary>
    /// Prefab von dem kleinen enemy der gespawnt werden soll
    /// </summary>
    [SerializeField] GameObject smallEnemyPrefab;

    /// <summary>
    /// Prefab vom den mittleren enemy der gespawnt werden soll
    /// </summary>
    [SerializeField] GameObject middleEnemyPrefab;

    /// <summary>
    /// Prefab vom den boss enemy
    /// </summary>
    [SerializeField] GameObject bossEnemyPrefab;

    /// <summary>
    /// Script der moving platform
    /// </summary>
    MovingPlatformBehaviour movingPlatformBehaviour;

    [SerializeField] Animator firstTextAnim;

    [SerializeField] Animator secondTextAnim;

    [SerializeField] Animator thirdTextAnim;

    [SerializeField] Animator fourthTextAnim;


    public int waveId = 0;

    /// <summary>
    /// In start, starte ich die Spawner coroutine
    /// </summary>
    private void Start()
    {
        movingPlatformBehaviour = GetComponentInParent<MovingPlatformBehaviour>();
    }

    public void WaveBehaviour()
    {
        switch (waveId)
        {
            case 1:
                firstTextAnim.SetTrigger("waveTrigger");
                break;
            case 3:
                secondTextAnim.SetTrigger("waveTrigger");
                break;
            case 6:
                thirdTextAnim.SetTrigger("waveTrigger");
                break;
            case 10:
                fourthTextAnim.SetTrigger("waveTrigger");
                break;
        }
    }

    /// <summary>
    /// Hier wird solange canSpawn true ist nach der spawnrate ein small enemy gespawned
    /// </summary>
    /// <returns></returns>
    public void FirstWave()
    {
        Instantiate(smallEnemyPrefab, leftSpawnPoint.position, Quaternion.identity);
        Instantiate(smallEnemyPrefab, rightSpawnPoint.position, Quaternion.identity);
    }

    public void SecondWave()
    {
        Instantiate(smallEnemyPrefab, leftSpawnPoint.position, Quaternion.identity);
        Instantiate(middleEnemyPrefab, rightSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnDelayedSmallEnemyLeft());
    }

    public void ThirdWave()
    {
        Instantiate(middleEnemyPrefab, leftSpawnPoint.position, Quaternion.identity);
        Instantiate(middleEnemyPrefab, rightSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnDelayedMiddleEnemyLeft());
        StartCoroutine(SpawnDelayedSmallEnemyRight());
    }

    public void FourthWave()
    {
        Instantiate(bossEnemyPrefab, middleSpawnPoint.position, Quaternion.identity);
    }

    public IEnumerator SpawnDelayedSmallEnemyLeft()
    {
        yield return new WaitForSeconds(10);
        Instantiate(smallEnemyPrefab, leftSpawnPoint.position, Quaternion.identity);
    }

    public IEnumerator SpawnDelayedSmallEnemyRight()
    {
        yield return new WaitForSeconds(10);
        Instantiate(smallEnemyPrefab, rightSpawnPoint.position, Quaternion.identity);
    }

    public IEnumerator SpawnDelayedMiddleEnemyLeft()
    {
        yield return new WaitForSeconds(15);
        Instantiate(smallEnemyPrefab, leftSpawnPoint.position, Quaternion.identity);
    }
}
