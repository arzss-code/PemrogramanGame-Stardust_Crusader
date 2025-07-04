using UnityEngine;

public class Boss3Bullets : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed;

    public void Initialize(Vector2 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
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
}
