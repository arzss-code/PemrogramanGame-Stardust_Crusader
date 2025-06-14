using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator anim;
    private Vector2 moveInput;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {

        Debug.Log("Vertical: " + Input.GetAxis("Vertical"));

        // Ambil input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveInput = new Vector2(horizontal, vertical);

        // Gerakkan player
        transform.position += new Vector3(horizontal, vertical, 0f) * moveSpeed * Time.deltaTime;

        // Set parameter animator
        anim.SetFloat("Vertical", vertical);

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
