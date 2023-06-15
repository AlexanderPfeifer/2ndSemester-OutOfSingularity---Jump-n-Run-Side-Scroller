using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;

public class FadeInScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] BoxCollider2D boxCollider2D;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Color c = sr.material.color;
        c.a = 0f;
        sr.material.color = c;
    }

    IEnumerator Fadein()
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
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
            startFadingIn();
    }

    public void startFadingIn()
    {
        StartCoroutine("Fadein");
    }
}
