using UnityEngine;
using System.Collections;

public class EnemyShip3 : MonoBehaviour
{
    private enum State { Entering, Battling }
    private State currentState = State.Entering;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float zigzagAmplitude = 1f;
    public float zigzagFrequency = 2f;

    [Header("Shooting")]
    [SerializeField] private EnemyLaserWeapon weapon;
    public float shootInterval = 2f;

    [Header("Battle Area")]
    public BoxCollider2D battleArea;
    [SerializeField] private float entryOffsetX = 1f; // seberapa jauh masuk ke BattleArea

    [Header("Health")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("Flash Hit Effect")]
    [SerializeField] private Material flashMaterial;
    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;

    private float shootTimer = 0f;
    private float startY;
    private float moveDirX = 1f;

    private float minY, maxY;
    private float minX, maxX;
    private float entryTargetX;

    private void Start()
    {
        startY = transform.position.y;
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }

        if (battleArea != null)
        {
            Bounds bounds = battleArea.bounds;
            minY = bounds.min.y;
            maxY = bounds.max.y;
            minX = bounds.min.x;
            maxX = bounds.max.x;

            entryTargetX = maxX - entryOffsetX;
        }
        else
        {
            Debug.LogWarning("⚠️ Battle Area belum di-assign pada EnemyShip3!");
            minY = -5f;
            maxY = 5f;
            minX = -15f;
            maxX = 15f;
            entryTargetX = 12f;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Entering:
                MoveIntoBattleArea();
                break;

            case State.Battling:
                ZigZagMove();
                HandleShooting();
                break;
        }
    }

    private void MoveIntoBattleArea()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x <= entryTargetX)
        {
            currentState = State.Battling;
            transform.position = new Vector3(entryTargetX, transform.position.y, transform.position.z);
        }
    }

    private void ZigZagMove()
    {
        float offsetY = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
        float newY = Mathf.Clamp(startY + offsetY, minY, maxY);

        transform.position += Vector3.right * moveDirX * moveSpeed * Time.deltaTime;

        float posX = transform.position.x;

        if (posX <= minX)
        {
            moveDirX = 1f;
            posX = minX;
        }
        else if (posX >= maxX)
        {
            moveDirX = -1f;
            posX = maxX;
        }

        transform.position = new Vector3(posX, newY, transform.position.z);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (flashMaterial != null && spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashEffect()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = originalMaterial;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void HandleShooting()
    {
        if (weapon == null) return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            weapon.Shoot();
            shootTimer = 0f;
        }
    }
}
