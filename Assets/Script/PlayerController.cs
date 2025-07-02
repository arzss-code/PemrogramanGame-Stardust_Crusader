using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public enum ControlScheme { Keyboard, Mouse }
    [Header("Control Settings")]
    public ControlScheme currentScheme = ControlScheme.Mouse;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float boostSpeed = 12f;
    public float stoppingDistance = 0.1f;

    private Vector3 lastMouseWorldPos;

    [Header("Shooting")]
    public float fireRate = 5f;
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

    [Header("Death Effect")]
    public GameObject deathEffect; // ➕ Tambahkan efek kematian

    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float lastVertical = 0f;
    private bool wasMovingVertically = false;
    private bool isBoosting = false;
    private bool boostExhausted = false;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    private bool isWeaponBoosted;
    public bool IsWeaponBoosted => isWeaponBoosted;

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
        lastMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        energy = maxEnergy;

        // Apply control scheme from settings
        if (SettingsManager.instance != null)
        {
            currentScheme = SettingsManager.instance.GetControlScheme();
            SettingsManager.instance.ApplySettingsToPlayer();
        }

        UIController.instance?.SetMaxEnergy(maxEnergy);
        UIController.instance?.SetMaxHealth(maxHealth);

        if (boostEffect != null)
            boostEffect.Stop();
    }

    private void Update()
    {
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

    private void HandleKeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;

        float currentMoveSpeed = isBoosting ? boostSpeed : moveSpeed;
        rb.linearVelocity = moveInput * currentMoveSpeed;
    }

    private void HandleMouseMovement()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDelta = (Vector2)mousePosition - (Vector2)lastMouseWorldPos;
        float mouseSpeed = mouseDelta.magnitude / Time.deltaTime;

        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        float minSpeed = moveSpeed;
        float maxSpeed = boostSpeed;
        float targetSpeed = Mathf.Clamp(mouseSpeed, minSpeed, maxSpeed);

        rb.linearVelocity = direction * targetSpeed;
        moveInput = direction;
        lastMouseWorldPos = mousePosition;
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

    private float shootDelay = 0.2f;
    private float lastShootTime = 0f;

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time - lastShootTime >= shootDelay)
        {
            nextFireTime = Time.time + 1f / fireRate;
            LaserWeapon.Instance.Shoot();
            lastShootTime = Time.time;
        }
    }

    private void UpdateAnimator()
    {
        switch (currentScheme)
        {
            case ControlScheme.Keyboard:
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
                    lastVertical = vertical;
                break;

            case ControlScheme.Mouse:
                float velocityThreshold = 1f;
                bool isMoving = rb.linearVelocity.magnitude > velocityThreshold;

                if (isMoving)
                    anim.SetFloat("Vertical", moveInput.y);

                if (wasMovingVertically && !isMoving)
                {
                    if (lastVertical > 0)
                        anim.Play("Idle-from-up");
                    else if (lastVertical < 0)
                        anim.Play("Idle-from-down");
                }

                wasMovingVertically = isMoving;
                if (isMoving)
                    lastVertical = moveInput.y;
                break;
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
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        Debug.Log("Health restored! Current Health: " + health);
        UIController.instance?.SetHealth(health);
    }

    public void RestoreEnergy(float amount)
    {
        energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
        Debug.Log("Energy restored! Current Energy: " + energy);
        UIController.instance?.SetEnergy(energy);
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

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UIController.instance?.SetHealth(health);

        spriteRenderer.material = whiteMaterial;
        StartCoroutine(ResetMaterial());

        if (health <= 0)
        {
            Debug.Log("Player Dead");

            // ➕ Efek partikel kematian
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            GameManager.instance.GameOver();
            gameObject.SetActive(false);
        }
    }

    IEnumerator ResetMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = defaultMaterial;
    }
}
