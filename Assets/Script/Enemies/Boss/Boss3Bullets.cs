using UnityEngine;

public class Boss3Bullets : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed;
    private int damage = 1;

    public void Initialize(Vector2 direction, float speed, int damageAmount = 1)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        damage = damageAmount;
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        // Destroy jika keluar layar (misal x < -30)
        if (transform.position.x < -30f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Berikan damage ke player melalui antarmuka IDamageable
            other.GetComponent<IDamageable>()?.TakeDamage(damage);
            Destroy(gameObject); // Hancurkan peluru setelah mengenai player
        }
    }
}
