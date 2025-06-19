using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Singleton instance untuk akses mudah dari script lain
    public static PlayerController instance;

    [Header("Movement")]
    public float moveSpeed = 5f; // Kecepatan gerak normal pemain
    public float boostSpeed = 12f; // Kecepatan gerak saat boost

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
        // Panggil fungsi-fungsi untuk menangani berbagai aspek pemain
        HandleInput();
        HandleBoosting();
        HandleShooting();
        MovePlayer();
        UpdateAnimator();
        UpdateEnergyUI();
    }

    private void HandleInput()
    {
        // Ambil input horizontal dan vertikal
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;
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

    private void HandleShooting()
    {
        // Tembak jika tombol kiri mouse ditekan
        if (Input.GetMouseButtonDown(0))
        {
            LaserWeapon.Instance.Shoot();
        }
    }

    private void MovePlayer()
    {
        // Kurangi gerakan vertikal saat boost
        float verticalReduction = isBoosting ? 0.3f : 1f;
        Vector3 movement = new Vector3(moveInput.x, moveInput.y * verticalReduction, 0f).normalized;

        float speed = moveSpeed;
        // Pindahkan pemain
        transform.position += movement * speed * Time.deltaTime;
    }

    private void UpdateAnimator()
    {
        float vertical = moveInput.y;

        // Set parameter vertical di animator
        anim.SetFloat("Vertical", vertical);

        bool isMovingVertically = Mathf.Abs(vertical) > 0.01f;

        // Atur animasi idle berdasarkan gerakan vertikal sebelumnya
        if (wasMovingVertically && !isMovingVertically)
        {
            if (lastVertical > 0)
                anim.Play("Idle-from-up");
            else if (lastVertical < 0)
                anim.Play("Idle-from-down");
        }

        // Update status gerakan vertikal
        wasMovingVertically = isMovingVertically;
        lastVertical = vertical;
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

        // Ambil damage jika bertabrakan dengan obstacle
        TakeDamage(1);
    }

    private void TakeDamage(int damage)
    {
        // Kurangi kesehatan
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UIController.instance?.SetHealth(health);

        // Efek visual saat terkena damage
        spriteRenderer.material = whiteMaterial;
        StartCoroutine("ResetMaterial");

        // Cek apakah pemain mati
        if (health <= 0)
        {
            // TODO: Trigger death, game over, animation, etc.
            Debug.Log("Player Dead");
        }
    }

    IEnumerator ResetMaterial()
    {
        // Tunggu sebentar sebelum
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = defaultMaterial;
    }
}
