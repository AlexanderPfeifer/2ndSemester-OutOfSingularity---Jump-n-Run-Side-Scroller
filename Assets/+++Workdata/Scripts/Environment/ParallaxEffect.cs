using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxEffect : MonoBehaviour
{
    /// <summary>
    /// transform der main camera
    /// </summary>
    public Transform mainCam;

    /// <summary>
    /// x speed des parallax effect
    /// </summary>
    [SerializeField] float parallaxSpeedX;
    /// <summary>
    /// y speed des parallax effect
    /// </summary>
    [SerializeField] float parallaxSpeedY;

    /// <summary>
    /// startposition x und y des sprites vom parallax effect
    /// </summary>
    float startPositionX, startPositionY;
    /// <summary>
    /// sprite grö0e x
    /// </summary>
    float spriteSizeX;

    public bool onPlatform = false;

    /// <summary>
    /// bestimmt camera main transform, gameObject transform position x und y und sprite size x
    /// </summary>
    private void Start()
    {
        startPositionX = transform.position.x;
        startPositionY = transform.position.y;
        spriteSizeX = GetComponent<SpriteRenderer>().bounds.size.x;
    }
     
    /// <summary>
    /// bewegt die bilder mit der kamera geschwindigkeit x und y, au0erdem wird ein sprite neben dem nächsten gesetzt wenn der spieler zu weit geht
    /// </summary>
    private void Update()
    {
        if(!onPlatform)
        {
            float relativeDistX = mainCam.transform.position.x * parallaxSpeedX;
            float relativeDistY = mainCam.transform.position.y * parallaxSpeedY;
            transform.position = new Vector3(startPositionX + relativeDistX, startPositionY + relativeDistY, transform.position.z);
        }
        else
        {
            float relativeDistX = Time.deltaTime * parallaxSpeedX;
            float relativeDistY = Time.deltaTime * parallaxSpeedY;
            transform.position = new Vector3(startPositionX + relativeDistX, startPositionY + relativeDistY, transform.position.z);
        }

        float relativeCameraDist = mainCam.transform.position.x * (1 - parallaxSpeedX);
        if(relativeCameraDist > startPositionX + spriteSizeX)
        {
            startPositionX += spriteSizeX;
        }
        else if(relativeCameraDist < startPositionX - spriteSizeX)
        {
            startPositionX -= spriteSizeX;
        }
    }
}
