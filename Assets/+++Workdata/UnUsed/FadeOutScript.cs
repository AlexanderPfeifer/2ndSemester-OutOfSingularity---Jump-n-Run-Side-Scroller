using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutScript : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] SpriteRenderer sr;

    void Start()
    {
        Color c = sr.material.color;
        c.a = 0f;
        sr.material.color = c;
    }

    IEnumerator FadeOut()
    {
        for (float f = 1f; f >= -0.05; f -= 0.05f)
        {
            Color c = sr.material.color;
            c.a = f;
            sr.material.color = c;
            yield return new WaitForSeconds(0.05f);
            boxCollider2D.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            startFadingOut();
    }

    public void startFadingOut()
    {
        StartCoroutine(FadeOut());
    }
}
