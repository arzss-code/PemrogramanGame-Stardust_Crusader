using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float boostSpeed = 12f;

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
        HandleInput();
        HandleBoosting();
        HandleShooting();
        MovePlayer();
        UpdateAnimator();
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

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LaserWeapon.Instance.Shoot();
        }

    }



    private void MovePlayer()
    {
        float verticalReduction = isBoosting ? 0.3f : 1f;
        Vector3 movement = new Vector3(moveInput.x, moveInput.y * verticalReduction, 0f).normalized;

        float speed = moveSpeed;
        transform.position += movement * speed * Time.deltaTime;
    }

    private void UpdateAnimator()
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
        lastVertical = vertical;
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
        StartCoroutine("ResetMaterial");

        if (health <= 0)
        {
            // TODO: Trigger death, game over, animation, etc.
            Debug.Log("Player Dead");
        }
    }

    IEnumerator ResetMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = defaultMaterial;
    }
}
