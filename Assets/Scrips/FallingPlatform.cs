using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class FallingPlatform : MonoBehaviour
{
    public float fallWait = 2f;       // Time before falling
    public float destroyWait = 1f;    // Time before disappearing
    public float respawnWait = 3f;    // Time before reappearing

    private bool isFalling;
    private Rigidbody2D rb;
    private Vector3 originalPosition; // Store original position
    private SpriteRenderer sr;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        originalPosition = transform.position; // Save original position
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rb.bodyType = RigidbodyType2D.Dynamic;  // Make it fall
        yield return new WaitForSeconds(destroyWait);
        
        sr.enabled = false;   // Hide platform
        col.enabled = false;  // Disable collision

        yield return new WaitForSeconds(respawnWait);

        Respawn();  // Call Respawn function
    }

    private void Respawn()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;    // Temporarily set to Dynamic to reset velocity
        rb.linearVelocity = Vector2.zero;              // Reset Velocity
        transform.position = originalPosition;    // Set back to original position
        rb.bodyType = RigidbodyType2D.Static;     // Set back to Static
        sr.enabled = true;                       // Show platform
        col.enabled = true;                      // Enable collision
        isFalling = false;                       // Reset falling state
    }
}
