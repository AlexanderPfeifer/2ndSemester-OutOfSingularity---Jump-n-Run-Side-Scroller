using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour
{
    public float movingSpeed;
    public int startingPoint;
    public Transform[] points;
    private int i;

    private void Start()
    {
        transform.position = points[startingPoint].position;
    }

    private void FixedUpdate()
    {
        if(Vector2.Distance(transform.position, points[i].position) < Mathf.Epsilon)
        {
            i++;
            if(i == points.Length)
            {
                i = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, movingSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
    }
}
