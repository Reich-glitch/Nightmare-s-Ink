using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Enemy : MonoBehaviour
{
    private Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;
    public GameObject deathParticles;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;

    public int damage = 1;
    public int maxHealth = 1;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        ogColor = spriteRenderer.color;

        ScaleStats(GameController.currentLevelIndex); // Call scale stats
        currentHealth = maxHealth; // Set the enemy health after scaling
    }

    public void ScaleStats(int level)
    {
        int finalLevel = GameController.survivedLevelsCount + 1;
        maxHealth = 1 + (level * 1); // Health increases by 2 every level
        chaseSpeed = 2f + (level * 0.1f); // Speed increases gradually
        spriteRenderer.color = Color.Lerp(Color.white, Color.red, (float)level / 20f);

        lootTable[0].dropChance = Mathf.Clamp(lootTable[0].dropChance - (level * 1f), 10f, 100f); // Small Gem
        lootTable[1].dropChance = Mathf.Clamp(lootTable[1].dropChance + (level * 0.5f), 5f, 50f);  // Big Gem
        lootTable[2].dropChance = Mathf.Clamp(lootTable[2].dropChance - (level * 0.5f), 10f, 50f); // Heart

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer);

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        bool isPlayerAbove =  Physics2D.Raycast(transform.position, Vector2.up, 5f, 1 << player.gameObject.layer);

        if(isGrounded)
        {
            //chaseplayer
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
            //if ground
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2 (direction, 0), 2f, groundLayer);
            //if gap
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            //If platform above
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

            if(!groundInFront.collider && !gapAhead.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true;
            }
        }
    }
    void FixedUpdate()
    {
        if(isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;

            Vector2 jumpDirection = direction * jumpForce;

            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (deathParticles != null)
        {
            GameObject particles = Instantiate(deathParticles, transform.position, Quaternion.identity);
            Destroy(particles, 1f); // Destroy particles after 1 second
        }
        
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die(); // Call Die() only if enemy should be removed
        }
    }


    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = ogColor;
    }

    void Die()
    {
        bool droppedLoot = false;
        if (deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);

        foreach(LootItem lootItem in lootTable)
        {
            if(UnityEngine.Random.Range(0f, 100f) <= lootItem.dropChance && !droppedLoot)
            {
                InstantiateLoot(lootItem.itemPrefab);
                droppedLoot = true; // Stop the loop once an item is dropped
            }
        }
        Destroy(gameObject);

        if (GameController.survivedLevelsCount >= 5)
        {
            InstantiateLoot(lootTable[1].itemPrefab); // Big Gem
        }
    }

    void InstantiateLoot(GameObject loot)
    {
        if(loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);

            droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
