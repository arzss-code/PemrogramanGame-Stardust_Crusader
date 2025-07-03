using UnityEngine;

public class Enemy3Bullets : MonoBehaviour
{
    private Vector2 moveDirection;
    private float speed;
    private float damage;

    public void Initialize(Vector2 direction, float speed, float damage)
    {
        this.moveDirection = direction.normalized;
        this.speed = speed;
        this.damage = damage;
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage((int)damage);
            }
            Destroy(gameObject);
        }
    }
}
