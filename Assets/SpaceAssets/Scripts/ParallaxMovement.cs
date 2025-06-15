using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMovement : MonoBehaviour {
    //Set the direction that the screen or the camera is moving
    ScrollDirection direction;
    //This speed value create the parallax effect
    //Note: This speed affect the movement of the object based on the camera speed
    public float minSpeed = 0.2f;
    public float maxSpeed = 0.6f;
    Vector3 speed;
    float scrollValue;
    float lastScrollValue;

    public enum BehaviourOnExit { Destroy, Regenerate };
    //Define if the object is destroyed or regenerate when the object is out of the screen
    public BehaviourOnExit behaviourOnExit = BehaviourOnExit.Regenerate;

    Transform cameraTransform;
    //Determine the value offScreen that the object has to be to consider out of screen
    //It also is used to regenerate it
    //if the value is 1f is the screen's width or heigth depending on the direction   
    public float limitOffScreen = 1f;

    void Start()
    {
        if (SpaceManager.instance != null)
            direction = SpaceManager.instance.scrollDirection;

        // Set kecepatan tetap karena tidak berdasarkan gerakan kamera
        float randomSpeed = Random.Range(minSpeed, maxSpeed);

        switch (direction)
        {
            case ScrollDirection.LeftToRight:
                speed = new Vector3(randomSpeed, 0f, 0f);
                break;
            case ScrollDirection.RightToLeft:
                speed = new Vector3(-randomSpeed, 0f, 0f);
                break;
            case ScrollDirection.DownToUp:
                speed = new Vector3(0f, randomSpeed, 0f);
                break;
            case ScrollDirection.UpToDown:
                speed = new Vector3(0f, -randomSpeed, 0f);
                break;
        }
    }


    void Regenerate()
    {
        Vector3 currentPos = transform.position;

        // Misalnya arah RightToLeft
        switch (direction)
        {
            case ScrollDirection.RightToLeft:
                float newY = currentPos.y; // Tetap di posisi Y yang sama
                transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1f + limitOffScreen, Camera.main.WorldToViewportPoint(transform.position).y, 10f));
                break;

                // Tambahkan logika lain jika pakai arah lain
        }

        // Hilangkan semua random jika tidak perlu
        // (Jika kamu hanya ingin looping tanpa rotasi atau ubah ukuran)
    }


    void Update()
    {
        // Gerakkan objek berdasarkan waktu, bukan pergerakan kamera
        float multiplier = PlayerController.instance != null ? PlayerController.instance.BoostMultiplier : 1f;
        transform.position += speed * multiplier * Time.deltaTime;


        // Cek apakah keluar layar, dan regenerasi
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        switch (direction)
        {
            case ScrollDirection.LeftToRight:
                if (viewportPos.x > 1f + limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;

            case ScrollDirection.RightToLeft:
                if (viewportPos.x < -limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;

            case ScrollDirection.DownToUp:
                if (viewportPos.y > 1f + limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;

            case ScrollDirection.UpToDown:
                if (viewportPos.y < -limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;
        }
    }

}
