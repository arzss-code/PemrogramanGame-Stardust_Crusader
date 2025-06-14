using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator anim;
    private Vector2 moveInput;
    private float lastVertical = 0f;
    private bool wasMovingVertically = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Ambil input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical).normalized;

        // Gerakkan player
        transform.position += new Vector3(horizontal, vertical, 0f) * moveSpeed * Time.deltaTime;

        // Set parameter animator
        anim.SetFloat("Vertical", vertical);

        bool isMovingVertically = Mathf.Abs(vertical) > 0.01f;

        // Deteksi saat berhenti naik atau turun
        if (wasMovingVertically && !isMovingVertically)
        {
            // Cek apakah sebelumnya naik (W) atau turun (S)
            if (lastVertical > 0)
                anim.Play("Idle-from-up");
            else if (lastVertical < 0)
                anim.Play("Idle-from-down");
        }

        // Update status sebelumnya
        wasMovingVertically = isMovingVertically;
        lastVertical = vertical;

        // Batasi dalam layar
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
