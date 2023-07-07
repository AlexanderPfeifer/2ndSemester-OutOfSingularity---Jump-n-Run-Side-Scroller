using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWavesScript : MonoBehaviour
{
    [SerializeField] EnemySpawnerBehaviour enemySpawnerBehaviour;

    void StartSecondWave()
    {
        enemySpawnerBehaviour.SecondWave();
    }

    void StartThirdWave()
    {
        enemySpawnerBehaviour.ThirdWave();
    }

    void StartFourthWave()
    {
        enemySpawnerBehaviour.FourthWave();
    }
}
