using UnityEngine;

public class PowerupMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f; // Kecepatan powerup ke kiri
    [SerializeField] private float destroyX = -12f; // Batas X untuk destroy

    private void Update()
    {
        float speedMultiplier = 1f;
        if (PlayerController.instance != null)
        {
            speedMultiplier = PlayerController.instance.BoostMultiplier;
        }
        transform.position += Vector3.left * moveSpeed * speedMultiplier * Time.deltaTime;
        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
