using System;
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayedDied;

    private bool isInvulnerable = false; // Flag for invulnerability

    public float invulnerabilityTime = 1f; // Time the player is invulnerable after taking damage

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (isInvulnerable) return;

    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    if (enemy)
    {
        TakeDamage(enemy.damage);
    }
}

private void OnTriggerEnter2D(Collider2D collision)
{
    if (isInvulnerable) return;

    Trap trap = collision.gameObject.GetComponent<Trap>();
    if (trap && trap.damage > 0)
    {
        TakeDamage(trap.damage);
    }
}


    void Heal(int amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthUI.UpdateHearts(currentHealth);
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage)
    {
        // If already invulnerable, do nothing
        if (isInvulnerable) return;

        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        // Start invulnerability and flash red effect
        StartCoroutine(FlashRed());
        StartCoroutine(InvulnerabilityCoroutine());

        if (currentHealth <= 0)
        {
            // Player dead 
            OnPlayedDied?.Invoke();
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true; // Enable invulnerability

        // Wait for the specified invulnerability duration
        yield return new WaitForSeconds(invulnerabilityTime);

        isInvulnerable = false; // Disable invulnerability
    }
}
