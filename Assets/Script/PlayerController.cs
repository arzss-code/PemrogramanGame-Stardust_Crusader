using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    // Enum untuk memilih skema kontrol di Inspector Unity
    public enum ControlScheme { Keyboard, Mouse }
    [Header("Control Settings")]
    public ControlScheme currentScheme = ControlScheme.Keyboard;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float boostSpeed = 12f;
    public float stoppingDistance = 0.1f;

    [Header("Shooting")]
    public float fireRate = 5f; // Tembakan per detik
    private float nextFireTime = 0f;

    [Header("Energy")]
    [SerializeField] private float energy;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyRegen = 10f;
    [SerializeField] private float boostEnergyDrain = 20f;

    [Header("Health")]
    [SerializeField] private int health = 5;
    [SerializeField] private int maxHealth = 5;

    [Header("Effects")]
    public ParticleSystem boostEffect;

    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float lastVertical = 0f;
    private bool wasMovingVertically = false;
    private bool isBoosting = false;
    private bool boostExhausted = false;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    [SerializeField] private Material whiteMaterial;

    public float BoostMultiplier => isBoosting ? (boostSpeed / moveSpeed) : 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        energy = maxEnergy;

        UIController.instance?.SetMaxEnergy(maxEnergy);
        UIController.instance?.SetMaxHealth(maxHealth);

        if (boostEffect != null)
            boostEffect.Stop();
    }

    private void Update()
    {
        // Memilih logika berdasarkan skema kontrol yang aktif
        switch (currentScheme)
        {
            case ControlScheme.Keyboard:
                HandleKeyboardMovement();
                break;
            case ControlScheme.Mouse:
                HandleMouseMovement();
                break;
        }

        HandleBoosting();
        HandleShooting();
        UpdateAnimator();
        UpdateEnergyUI();
    }

    // Logika untuk kontrol Keyboard
    private void HandleKeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;
        
        float currentMoveSpeed = isBoosting ? boostSpeed : moveSpeed;
        rb.linearVelocity = moveInput * currentMoveSpeed;
    }

    // Logika untuk kontrol Mouse
    private void HandleMouseMovement()
    {
        // Mendapatkan posisi mouse di dunia game
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Menghitung arah dari pemain ke mouse
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
        
        // Menggerakkan pemain ke arah mouse
        float currentMoveSpeed = isBoosting ? boostSpeed : moveSpeed;
        rb.linearVelocity = direction * currentMoveSpeed;

        // Untuk keperluan animasi, kita anggap selalu bergerak maju jika mouse jauh
        moveInput = direction; 
    }

    private void HandleBoosting()
    {
        if (boostExhausted && !Input.GetMouseButton(1))
            boostExhausted = false;

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

    // Logika menembak yang sudah dimodifikasi
    private void HandleShooting()
    {
        // Cek jika tombol mouse kiri ditekan/ditahan dan sudah waktunya menembak lagi
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            // Set waktu tembakan berikutnya
            nextFireTime = Time.time + 1f / fireRate;
            LaserWeapon.Instance.Shoot();
        }
    }

    // Hapus method MovePlayer() yang lama karena sudah digantikan
    // private void MovePlayer() { ... }

    private void UpdateAnimator()
    {
        switch (currentScheme)
        {
            // LOGIKA ANIMASI UNTUK KEYBOARD (TETAP SAMA SEPERTI SEBELUMNYA)
            case ControlScheme.Keyboard:
                {
                    float vertical = moveInput.y;
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
                    if (isMovingVertically)
                    {
                        lastVertical = vertical;
                    }
                    break;
                }

            // LOGIKA ANIMASI BARU UNTUK MOUSE
            case ControlScheme.Mouse:
                {
                    // Cek jarak antara pemain dan posisi kursor mouse
                    float distanceToMouse = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                    // Jika jaraknya lebih besar dari stoppingDistance, berarti pemain sedang bergerak
                    bool isMoving = distanceToMouse > stoppingDistance;

                    if (isMoving)
                    {
                        // Atur float "Vertical" berdasarkan arah gerakan (moveInput.y)
                        // Ini membuat kapal tetap terlihat miring ke atas/bawah saat bergerak dengan mouse
                        anim.SetFloat("Vertical", moveInput.y);
                    }

                    // Jika sebelumnya bergerak dan sekarang berhenti (masuk dalam stoppingDistance)
                    if (wasMovingVertically && !isMoving)
                    {
                        // Mainkan animasi idle yang sesuai berdasarkan arah gerakan terakhir
                        if (lastVertical > 0)
                            anim.Play("Idle-from-up");
                        else if (lastVertical < 0)
                            anim.Play("Idle-from-down");
                    }

                    // Perbarui status dan arah terakhir HANYA jika sedang bergerak
                    wasMovingVertically = isMoving;
                    if (isMoving)
                    {
                        lastVertical = moveInput.y;
                    }
                    break;
                }
        }
    }

    private void UpdateEnergyUI()
    {
        UIController.instance?.SetEnergy(energy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("obstacle")) return;
        TakeDamage(1);
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UIController.instance?.SetHealth(health);
        spriteRenderer.material = whiteMaterial;
        StartCoroutine(ResetMaterial());

        if (health <= 0)
        {
            Debug.Log("Player Dead");
            // TODO: Trigger death, game over, animation, etc.
        }
    }

    IEnumerator ResetMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = defaultMaterial;
    }
}