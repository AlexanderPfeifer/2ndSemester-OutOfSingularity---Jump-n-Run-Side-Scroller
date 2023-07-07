using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heavyAttackBossBehaviour : MonoBehaviour
{
    [SerializeField] BossEnemyScript bossEnemyScript;

    private void Update()
    {
        bossEnemyScript.HeavyAttack();
    }
}
