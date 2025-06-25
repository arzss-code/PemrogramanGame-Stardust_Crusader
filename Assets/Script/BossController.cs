using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BossController : MonoBehaviour
{
    // Enum untuk State Utama
    public enum BossState { Entering, Fighting, Dying }
    public BossState currentState;

    // Enum untuk Sub-State saat Bertarung
    private enum FightingSubState { Patrolling, Charging, Returning }
    private FightingSubState fightingState;

    [Header("Boss Stats")]
    public int health = 100;
    private int maxHealth;

    [Header("Boss Movement")]
    public float entranceSpeed = 5f;
    public float moveSpeed = 2f;
    public float moveDistance = 5f;
    private Vector2 startPosition;
    private Vector2 targetBattlePosition;

    [Header("Charge Attack")]
    [Tooltip("Kecepatan saat melesat maju.")]
    public float chargeSpeed = 20f;
    [Tooltip("Berapa lama boss berpatroli sebelum melakukan charge.")]
    public float patrolDuration = 4f;
    private float patrolTimer;
    private Vector2 chargeTargetPosition;
    private Transform playerTransform;

    // ... (sisa deklarasi variabel UI dan Effects tetap sama)
    [Header("UI References")]
    public Slider healthBarSlider;
    public TextMeshProUGUI healthText;
    [Header("Effects")]
    public GameObject destructionEffect;
    public Material whiteFlashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;


    public void Initialize(Vector2 battlePosition)
    {
        this.targetBattlePosition = battlePosition;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        maxHealth = health;
        
        // Cari transform player untuk target charge
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Start()
    {
        currentState = BossState.Entering;
    }

    private void Update()
    {
        // State machine utama
        switch (currentState)
        {
            case BossState.Entering:
                HandleEntrance();
                break;
            case BossState.Fighting:
                HandleFightingBehavior(); // Ganti ke method baru
                break;
            case BossState.Dying:
                break;
        }
    }

    private void HandleEntrance()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetBattlePosition, entranceSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetBattlePosition) < 0.01f)
        {
            startPosition = transform.position;
            SetupHealthBar();

            // Setup awal untuk state Fighting
            currentState = BossState.Fighting;
            fightingState = FightingSubState.Patrolling; // Mulai dengan patroli
            patrolTimer = 0f;
        }
    }

    // METHOD BARU untuk mengatur semua perilaku saat bertarung
    private void HandleFightingBehavior()
    {
        // State machine untuk pertarungan
        switch (fightingState)
        {
            case FightingSubState.Patrolling:
                // Gerakan naik-turun
                float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
                transform.position = new Vector2(startPosition.x, newY);

                // Hitung waktu patroli
                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolDuration)
                {
                    // Waktu patroli habis, siapkan untuk charge
                    if (playerTransform != null)
                    {
                        // Kunci target: di ujung kiri layar dengan ketinggian Y pemain saat itu
                        chargeTargetPosition = new Vector2(-60f, playerTransform.position.y);
                    }
                    else // Fallback jika player tidak ditemukan
                    {
                        chargeTargetPosition = new Vector2(-60f, transform.position.y);
                    }
                    fightingState = FightingSubState.Charging; // Ganti state ke charging
                }
                break;

            case FightingSubState.Charging:
                // Melesat lurus ke target
                transform.position = Vector2.MoveTowards(transform.position, chargeTargetPosition, chargeSpeed * Time.deltaTime);

                // Jika sudah sampai di tujuan charge
                if (Vector2.Distance(transform.position, chargeTargetPosition) < 0.01f)
                {
                    fightingState = FightingSubState.Returning; // Ganti state untuk kembali
                }
                break;

            case FightingSubState.Returning:
                // Kembali ke posisi patroli awal
                Vector2 returnPosition = new Vector2(startPosition.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, returnPosition, chargeSpeed * Time.deltaTime);

                // Jika sudah sampai di posisi awal
                if (Mathf.Abs(transform.position.x - startPosition.x) < 0.01f)
                {
                    patrolTimer = 0f; // Reset timer patroli
                    fightingState = FightingSubState.Patrolling; // Kembali patroli
                }
                break;
        }
    }
    
    // ... (Sisa script TakeDamage, OnCollisionEnter2D, Die, UI, dan FlashWhite tetap sama persis)
    public void TakeDamage(int damageAmount){if(currentState==BossState.Dying)return;health-=damageAmount;health=Mathf.Max(health,0);UpdateHealthBar();StartCoroutine(FlashWhite());if(health<=0){currentState=BossState.Dying;Die();}}
    private void OnCollisionEnter2D(Collision2D collision){if(collision.gameObject.CompareTag("Player")){PlayerController player=collision.gameObject.GetComponent<PlayerController>();if(player!=null){player.TakeDamage(1);}}}
    private void Die(){Debug.Log("BOSS DIKALAHKAN!");if(healthBarSlider!=null){healthBarSlider.gameObject.SetActive(false);}if(destructionEffect!=null){Instantiate(destructionEffect,transform.position,Quaternion.identity);}GameManager.instance.LevelCompleted();Destroy(gameObject);}
    void SetupHealthBar(){if(healthBarSlider!=null){healthBarSlider.gameObject.SetActive(true);healthBarSlider.maxValue=maxHealth;UpdateHealthBar();}}
    void UpdateHealthBar(){if(healthBarSlider!=null){healthBarSlider.value=health;if(healthText!=null){healthText.text=health+" / "+maxHealth;}}}
    private IEnumerator FlashWhite(){if(spriteRenderer!=null&&whiteFlashMaterial!=null){spriteRenderer.material=whiteFlashMaterial;yield return new WaitForSeconds(0.1f);spriteRenderer.material=defaultMaterial;}}
}