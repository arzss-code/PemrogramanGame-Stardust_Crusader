using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroids : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float naturalDriftY = 0.5f; // Gerakan Y alami
    [SerializeField] private float rotationTorque = 30f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Random sprite
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        // Drift natural ke atas/bawah
        float driftY = Random.Range(-naturalDriftY, naturalDriftY);
        rb.linearVelocity = new Vector2(0, driftY);

        // Rotasi acak
        float torque = Random.Range(-rotationTorque, rotationTorque);
        rb.AddTorque(torque);

        // Gravity, drag, dll dinonaktifkan via Inspector
    }

    void Update()
    {
        // Efek worldSpeed dan BoostMultiplier tetap dipakai
        float moveX = (GameManager.instance.worldSpeed * PlayerController.instance.BoostMultiplier) * Time.deltaTime;
        transform.position += new Vector3(-moveX, 0);

        // Hancurkan jika terlalu kiri
        if (transform.position.x < -60f)
        {
            Destroy(gameObject);
        }
    }
}
