using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiraleBulletPattern : MonoBehaviour
{
    private float angle = 0;

    private Vector2 bulletMoveDirection;

    private void Fire()
    {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;

            GameObject bul = BulletPool.bulletPoolInstance.GetBullet();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetMoveDirection(bulDir);

            angle += 10f;
    }

    public void StartFire()
    {
        InvokeRepeating("Fire", 0, 0.1f);

    }

    public void StopFire()
    {
        CancelInvoke("Fire");
    }
}
