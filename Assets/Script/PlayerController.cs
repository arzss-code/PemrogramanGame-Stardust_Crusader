using UnityEngine;

public class PlayerController : MonoBehaviour
{


    [Header("Movement")]
    public float moveSpeed = 5f;
    public float boostSpeed = 12f;

    [Header("Energy")]
    [SerializeField] private float energy;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyRegen = 10f;
    [SerializeField] private float boostEnergyDrain = 20f;

    [Header("Effects")]
    public ParticleSystem boostEffect; // Assign via Inspector

    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float lastVertical = 0f;
    private bool wasMovingVertically = false;
    private bool isBoosting = false;
    private bool boostExhausted = false;
    public static PlayerController instance;  // Tambahkan ini di atas
    public float BoostMultiplier => isBoosting ? (boostSpeed / moveSpeed) : 1f;  // Tambahkan getter ini
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Jika sudah ada instance lain, hancurkan yang ini
        }
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        energy = maxEnergy;

        // Inisialisasi Energy UI
        UIController.instance?.SetMaxEnergy(maxEnergy);

        if (boostEffect != null)
            boostEffect.Stop();
    }

    private void Update()
    {
        HandleInput();
        HandleBoosting();
        MovePlayer();
        UpdateAnimator();
        //ClampToScreen();
        UpdateEnergyUI();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;
    }

    private void HandleBoosting()
    {
        // Cek jika energy habis sebelumnya dan tombol sudah dilepas
        if (boostExhausted && !Input.GetMouseButton(1))
        {
            boostExhausted = false; // Reset agar bisa boosting lagi
        }

        // Jika boost tidak kelelahan dan klik kanan ditekan
        if (Input.GetMouseButton(1) && energy > 0f && !boostExhausted)
        {
            isBoosting = true;
            energy -= boostEnergyDrain * Time.deltaTime;
            energy = Mathf.Max(energy, 0f);

            if (boostEffect != null && !boostEffect.isPlaying)
                boostEffect.Play();

            if (energy == 0f)
            {
                isBoosting = false;
                boostExhausted = true;

                if (boostEffect != null && boostEffect.isPlaying)
                    boostEffect.Stop();
            }
        }
        else
        {
            isBoosting = false;
            energy = Mathf.Min(energy + energyRegen * Time.deltaTime, maxEnergy);

            if (boostEffect != null && boostEffect.isPlaying)
                boostEffect.Stop();
        }
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Jika Boost aktif, tetap izinkan gerakan player manual, tapi tidak paksa gerak ke kanan
        if (isBoosting)
        {
            moveY *= 0.3f;  // Kalau kamu masih mau mengurangi gerakan vertikal saat boost
        }

        Vector3 movement = new Vector3(moveX, moveY, 0f).normalized;
        float speed = moveSpeed; // Boost tidak meningkatkan kecepatan player

        transform.position += movement * speed * Time.deltaTime;
    }


    private void UpdateAnimator()
    {
        float vertical = Input.GetAxisRaw("Vertical");

        anim.SetFloat("Vertical", vertical);

        bool isMovingVertically = Mathf.Abs(vertical) > 0.01f;

        if (wasMovingVertically && !isMovingVertically)
        {
            if (lastVertical > 0)
                anim.Play("Idle-from-up");
            else if (lastVertical < 0)
                anim.Play("Idle-from-down");
        }

        wasMovingVertically = isMovingVertically;
        lastVertical = vertical;
    }

    //private void ClampToScreen()
    //{
    //    Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
     //   pos.x = Mathf.Clamp01(pos.x);
    //    pos.y = Mathf.Clamp01(pos.y);
    //    transform.position = Camera.main.ViewportToWorldPoint(pos);
    //}

    private void UpdateEnergyUI()
    {
        UIController.instance?.SetEnergy(energy);
    }
}
