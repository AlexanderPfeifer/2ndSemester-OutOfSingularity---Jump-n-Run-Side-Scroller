using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSpiraleBulletPattern : MonoBehaviour
{
    private float angle = 0;

    private Vector2 bulletMoveDirection;

    private void Fire()
    {
        for(int i = 0; i <= 1; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin(((angle + 180f * i) * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos(((angle + 180f * i) * Mathf.PI) / 180f);

            Vector3 bulMoveVecor = new Vector3(bulDirX, bulDirY, 0);
            Vector2 bulDir = (bulMoveVecor - transform.position).normalized;

            GameObject bul = BulletPool.bulletPoolInstance.GetBullet();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetMoveDirection(bulDir);

            angle += 10f;

            if(angle >= 360f)
            {
                angle = 0f;
            }
        }
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
