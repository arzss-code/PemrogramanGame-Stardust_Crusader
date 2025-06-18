using UnityEngine;

public class LaserBullets : MonoBehaviour
{
    private Vector2 direction = Vector2.right;

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * LaserWeapon.Instance.speed * Time.deltaTime);

        if (transform.position.x > 55)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
