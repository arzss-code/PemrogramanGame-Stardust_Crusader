using UnityEngine;
using System.Collections;

public class Boss3Controller : MonoBehaviour
{
    private enum State { Entering, Attacking }
    private State currentState = State.Entering;
    [SerializeField] private BossRegenShield regenShield;


    [Header("Movement")]
    public float entrySpeed = 3f;
    public float entryTargetX = 12f;

    [Header("Battle Area")]
    public BoxCollider2D battleArea;

    [Header("Attack Pattern")]
    [SerializeField] private Boss3LaserSpread laserSpread;
    [SerializeField] private Boss3LaserSpiral laserSpiral;
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private float spiralAttackDuration = 4f;

    [Header("Health")]
    [SerializeField] private int maxHP = 20;
    private int currentHP;

    [Header("Flash Effect")]
    [SerializeField] private Material whiteMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;

    private float attackTimer = 0f;
    private int currentPattern = 0;
    private Coroutine spiralCoroutine;

    private void Start()
    {
        currentHP = maxHP;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
        }

        if (battleArea != null)
        {
            entryTargetX = battleArea.bounds.max.x - 1f; // masuk sedikit ke dalam BattleArea
        }

        // Aktifkan UI HP dan Shield saat Boss muncul
        if (LevelController.Instance != null)
        {
            if (LevelController.Instance.BossShieldSlider != null)
                LevelController.Instance.BossShieldSlider.gameObject.SetActive(true);

            if (LevelController.Instance.BossShieldText != null)
                LevelController.Instance.BossShieldText.gameObject.SetActive(true);

            if (LevelController.Instance.BossHealthSlider != null)
                LevelController.Instance.BossHealthSlider.gameObject.SetActive(true);

            if (LevelController.Instance.BossHealthText != null)
                LevelController.Instance.BossHealthText.gameObject.SetActive(true);

            LevelController.Instance.BossHealthSlider.maxValue = maxHP;
            LevelController.Instance.BossHealthSlider.value = currentHP;
            LevelController.Instance.BossHealthText.text = currentHP.ToString();
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Entering:
                MoveIntoScreen();
                break;

            case State.Attacking:
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    attackTimer = 0f;
                    PerformAttack();
                }
                break;
        }
    }

    private void MoveIntoScreen()
    {
        transform.position += Vector3.left * entrySpeed * Time.deltaTime;

        if (transform.position.x <= entryTargetX)
        {
            transform.position = new Vector3(entryTargetX, transform.position.y, transform.position.z);
            currentState = State.Attacking;
            Debug.Log("🎯 Boss3 memasuki layar dan mulai menyerang!");
        }
    }

    private void PerformAttack()
    {
        switch (currentPattern)
        {
            case 0:
                laserSpread?.Shoot();
                break;

            case 1:
                if (spiralCoroutine == null)
                {
                    spiralCoroutine = StartCoroutine(FireSpiralTemporarily());
                }
                break;
        }

        currentPattern = (currentPattern + 1) % 2;
    }

    private IEnumerator FireSpiralTemporarily()
    {
        if (laserSpiral != null)
        {
            laserSpiral.enabled = true;
            yield return new WaitForSeconds(spiralAttackDuration);
            laserSpiral.enabled = false;
        }

        spiralCoroutine = null;
    }

    public void TakeDamage(int damage)
    {
        // Jika shield aktif, serap damage dulu
        if (regenShield != null && regenShield.IsActive())
        {
            int remaining = regenShield.AbsorbDamage(damage);
            if (remaining <= 0)
            {
                StartCoroutine(FlashEffect()); // Tetap kasih efek flash saat shield aktif
                return;
            }
            damage = remaining; // Damage sisa masuk ke Boss HP
        }

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        if (LevelController.Instance != null && LevelController.Instance.BossHealthSlider != null)
        {
            LevelController.Instance.BossHealthSlider.value = currentHP;
            LevelController.Instance.BossHealthText.text = currentHP.ToString();
        }

        StartCoroutine(FlashEffect());

        if (currentHP <= 0)
        {
            Die();
        }
    }


    private IEnumerator FlashEffect()
    {
        if (spriteRenderer != null && whiteMaterial != null)
        {
            spriteRenderer.material = whiteMaterial;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.material = defaultMaterial;
        }
    }

    private void Die()
    {
        Debug.Log("💀 Boss3 telah dikalahkan!");
        if (LevelController.Instance != null)
        {
            LevelController.Instance.OnBossDefeated();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

}
