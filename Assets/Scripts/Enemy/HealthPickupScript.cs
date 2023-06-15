using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
    HealthbarScript healthbarScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerScript playerScript = collision.GetComponent<PlayerScript>();

        if (playerScript.currentHealthPlayer < 100)
        {
            playerScript.currentHealthPlayer = playerScript.maxHealthPlayer;
            healthbarScript.SetHealth(playerScript.maxHealthPlayer);
            Destroy(gameObject);
        }
    }
}
