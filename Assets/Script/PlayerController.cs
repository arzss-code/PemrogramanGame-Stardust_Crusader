using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Singleton instance untuk akses mudah dari script lain
    public static PlayerController instance;

    // Enum untuk memilih skema kontrol di Inspector Unity
    public enum ControlScheme { Keyboard, Mouse }
    [Header("Control Settings")]
    public ControlScheme currentScheme = ControlScheme.Keyboard;

    [Header("Movement")]
    public float moveSpeed = 5f; // Kecepatan gerak normal pemain
    public float boostSpeed = 12f; // Kecepatan gerak saat boost
    public float stoppingDistance = 0.1f;
    
    private Vector3 lastMouseWorldPos;

    [Header("Shooting")]
    public float fireRate = 5f; // Tembakan per detik
    private float nextFireTime = 0f;

    [Header("Energy")]
    [SerializeField] private float energy; // Energi saat ini
    [SerializeField] private float maxEnergy = 100f; // Energi maksimum
    [SerializeField] private float energyRegen = 10f; // Kecepatan regenerasi energi
    [SerializeField] private float boostEnergyDrain = 20f; // Kecepatan pengurangan energi saat boost

    [Header("Health")]
    [SerializeField] private int health = 5; // Kesehatan saat ini
    [SerializeField] private int maxHealth = 5; // Kesehatan maksimum

    [Header("Effects")]
    public ParticleSystem boostEffect; // Efek partikel untuk boost

    private Animator anim; // Komponen animator
    private Rigidbody2D rb; // Komponen rigidbody 2D
    private Vector2 moveInput; // Input gerakan dari pemain
    private float lastVertical = 0f; // Menyimpan input vertikal terakhir
    private bool wasMovingVertically = false; // Menandai apakah pemain bergerak vertikal sebelumnya
    private bool isBoosting = false; // Menandai apakah pemain sedang boost
    private bool boostExhausted = false; // Menandai apakah boost sudah habis
    private SpriteRenderer spriteRenderer; // Komponen sprite renderer
    private Material defaultMaterial; // Material default untuk sprite
    private bool isWeaponBoosted;
    public bool IsWeaponBoosted => isWeaponBoosted;
    [SerializeField] private Material whiteMaterial; // Material putih untuk efek damage

    // Property untuk mendapatkan pengali boost
    public float BoostMultiplier => isBoosting ? (boostSpeed / moveSpeed) : 1f;

    private void Awake()
    {
        // Implementasi singleton
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Inisialisasi komponen-komponen
        lastMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Inisialisasi posisi mouse
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        energy = maxEnergy;

        // Set nilai maksimum energi dan kesehatan di UI
        UIController.instance?.SetMaxEnergy(maxEnergy);
        UIController.instance?.SetMaxHealth(maxHealth);

        // Hentikan efek boost jika ada
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
        // Ambil input horizontal dan vertikal
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

        // Hitung delta posisi mouse (kecepatan kursor)
        Vector2 mouseDelta = (Vector2)mousePosition - (Vector2)lastMouseWorldPos;
        float mouseSpeed = mouseDelta.magnitude / Time.deltaTime;

        // Normalisasi arah dari pemain ke mouse
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        // Atur kecepatan minimum dan maksimum player
        float minSpeed = moveSpeed;
        float maxSpeed = boostSpeed;

        // Skala kecepatan player berdasarkan kecepatan mouse, clamp agar tidak terlalu lambat/cepat
        float targetSpeed = Mathf.Clamp(mouseSpeed, minSpeed, maxSpeed);

        // Terapkan kecepatan ke rigidbody
        rb.linearVelocity = direction * targetSpeed;

        // Untuk keperluan animasi
        moveInput = direction;

        // Simpan posisi mouse untuk frame berikutnya
        lastMouseWorldPos = mousePosition;
    }

    private void HandleBoosting()
    {
        // Reset boost exhausted jika tombol boost dilepas
        if (boostExhausted && !Input.GetMouseButton(1))
            boostExhausted = false;

        // Aktifkan boost jika tombol ditekan, energi cukup, dan tidak exhausted
        if (Input.GetMouseButton(1) && energy > 0f && !boostExhausted)
        {
            isBoosting = true;
            energy -= boostEnergyDrain * Time.deltaTime;
            energy = Mathf.Max(energy, 0f);

            // Aktifkan efek boost
            if (boostEffect != null && !boostEffect.isPlaying)
                boostEffect.Play();

            // Nonaktifkan boost jika energi habis
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
            // Nonaktifkan boost dan regenerasi energi
            isBoosting = false;
            energy = Mathf.Min(energy + energyRegen * Time.deltaTime, maxEnergy);

            // Hentikan efek boost
            if (boostEffect != null && boostEffect.isPlaying)
                boostEffect.Stop();
        }
    }

    private float shootDelay = 0.2f; // Waktu delay antar tembakan (dalam detik)
    private float lastShootTime = 0f;

    private void HandleShooting()
    {
        // Tembak selama tombol kiri mouse ditekan dan sudah melewati delay
        if (Input.GetMouseButton(0) && Time.time - lastShootTime >= shootDelay)
        {
            // Set waktu tembakan berikutnya
            nextFireTime = Time.time + 1f / fireRate;
            LaserWeapon.Instance.Shoot();
            lastShootTime = Time.time;
        }
    }

    // Hapus method MovePlayer() yang lama karena sudah digantikan
    // private void MovePlayer() { ... }

    private void UpdateAnimator()
    {
        switch (currentScheme)
        {
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

            case ControlScheme.Mouse:
                {
                    // Gunakan kecepatan nyata player untuk menentukan status bergerak
                    float velocityThreshold = 1f;
                    bool isMoving = rb.linearVelocity.magnitude > velocityThreshold;

                    if (isMoving)
                    {
                        anim.SetFloat("Vertical", moveInput.y);
                    }

                    if (wasMovingVertically && !isMoving)
                    {
                        if (lastVertical > 0)
                            anim.Play("Idle-from-up");
                        else if (lastVertical < 0)
                            anim.Play("Idle-from-down");
                    }

                    wasMovingVertically = isMoving;
                    if (isMoving)
                    {
                        lastVertical = moveInput.y;
                    }
                    break;
                }
        }
    }

    public void ActivateWeaponBoost(float duration)
    {
        StopCoroutine(nameof(WeaponBoostCoroutine));
        StartCoroutine(WeaponBoostCoroutine(duration));
    }
    private IEnumerator WeaponBoostCoroutine(float duration)
    {
        isWeaponBoosted = true;
        yield return new WaitForSeconds(duration);
        isWeaponBoosted = false;
    }

    public void RestoreHealth(int amount)
    {
        // Tambah health, tapi jangan sampai melebihi maxHealth
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        Debug.Log("Health restored! Current Health: " + health);

        // Perbarui UI health
        UIController.instance?.SetHealth(health);
    }

    public void RestoreEnergy(float amount)
    {
        // Tambah energy, tapi jangan sampai melebihi maxEnergy
        energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
        Debug.Log("Energy restored! Current Energy: " + energy);

        // Perbarui UI energy
        UIController.instance?.SetEnergy(energy);
    }

    private void UpdateEnergyUI()
    {
        // Update UI energi
        UIController.instance?.SetEnergy(energy);
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek tabrakan dengan obstacle
        if (!collision.gameObject.CompareTag("obstacle")) return;
        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        // Kurangi kesehatan
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UIController.instance?.SetHealth(health);

        // Efek visual saat terkena damage
        spriteRenderer.material = whiteMaterial;
        StartCoroutine(ResetMaterial());

        // Cek apakah pemain mati
        if (health <= 0)
        {
            Debug.Log("Player Dead");
            // TODO: Trigger death, game over, animation, etc.
        }
    }

    IEnumerator ResetMaterial()
    {
        // Tunggu sebentar sebelum
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = defaultMaterial;
    }
}