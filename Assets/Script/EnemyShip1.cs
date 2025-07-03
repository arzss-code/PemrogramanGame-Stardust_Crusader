using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShip1 : MonoBehaviour
{
    private enum State
    {
        WaitingForObstacleClear,
        EnteringScreen,
        EnteringFormation,
        Waiting,
        Charging,
        Exiting
    }

    [Header("Formasi & Target")]
    public Vector2 formationPositionOffset = new Vector2(-5f, 0f);
    public float waitDuration = 1f;
    public float chargeSpeed = 10f;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float destroyXThreshold = -20f;
    public float entrySpeed = 15f;
    [SerializeField] private float colliderDelay = 1f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    
    [Header("Score Settings")]
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private string enemyType = "Enemy Ship";

    [Header("Hit Effect")]
    [SerializeField] private Material whiteMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 formationTargetPosition;
    private Transform player;
    private State currentState = State.WaitingForObstacleClear;
    private float waitTimer = 0f;

    private ObjectSpawner obstacleSpawner;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col.enabled = false;
        StartCoroutine(EnableColliderAfterDelay(colliderDelay));

        if (spriteRenderer != null)
            defaultMaterial = spriteRenderer.material;

        currentHealth = maxHealth;
        formationTargetPosition = (Vector2)transform.position + formationPositionOffset;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // HAPUS obstacleSpawner

        // LANGSUNG masuk ke state masuk layar
        currentState = State.EnteringScreen;
        rb.linearVelocity = Vector2.left * entrySpeed;
    }


    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (col != null) col.enabled = true;
    }

    private IEnumerator WaitForObstacleThenEnter()
    {
        if (obstacleSpawner != null)
        {
            yield return new WaitUntil(() => obstacleSpawner.finishedSpawning);
            yield return new WaitForSeconds(3f);
        }

        currentState = State.EnteringScreen;
        rb.linearVelocity = Vector2.left * entrySpeed;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.EnteringScreen:
                if (transform.position.x <= formationTargetPosition.x + 1f)
                {
                    rb.linearVelocity = Vector2.zero;
                    currentState = State.EnteringFormation;
                }
                break;

            case State.EnteringFormation:
                MoveToFormation();
                break;

            case State.Waiting:
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    currentState = State.Charging;
                }
                break;

            case State.Charging:
                ChargeAtPlayer();
                break;

            case State.Exiting:
                CheckForExit();
                break;
        }
    }

    private void MoveToFormation()
    {
        transform.position = Vector2.MoveTowards(transform.position, formationTargetPosition, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, formationTargetPosition) <= 0.1f)
        {
            currentState = State.Waiting;
            waitTimer = waitDuration;
        }
    }

    private void ChargeAtPlayer()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.left * chargeSpeed;
        }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * chargeSpeed;
        }

        currentState = State.Exiting;
    }

    private void CheckForExit()
    {
        if (transform.position.x < destroyXThreshold)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (whiteMaterial != null && spriteRenderer != null)
        {
            StartCoroutine(FlashHitEffect());
        }

        if (currentHealth <= 0)
        {
            // Give score when enemy dies
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddEnemyKillScore(enemyType);
            }
            
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashHitEffect()
    {
        spriteRenderer.material = whiteMaterial;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material = defaultMaterial;
    }
}
