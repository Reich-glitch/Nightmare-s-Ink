using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight = true;
    public ParticleSystem smokeFX;
    public ParticleSystem speedFX;

    BoxCollider2D playerCollider;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    float speedMultiplier = 1f;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Jumping")]
    public float jumpPower = 5f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    bool isGrounded;
    bool isOnPlatform;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;

    //Wall Jumping
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        smokeFX = GetComponentInChildren<ParticleSystem>();
        trailRenderer = GetComponent<TrailRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        SpeedItem.OnSpeedCollected += StartSpeedBoost;
    }

    void StartSpeedBoost(float multiplyer)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplyer));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplyer)
    {
        speedMultiplier = multiplyer;
        speedFX.Play();
        yield return new WaitForSeconds(2f);
        speedMultiplier = 1f;
        speedFX.Stop();
    }

    // Update is called once per frame
    void Update()
    {        
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
        if (isDashing)
        {
            return;
        }
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
        Flip();

        if(!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed * speedMultiplier, rb.linearVelocity.y);
            Flip();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    

    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        isDashing = false;
        trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if(context.performed && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.25f));
        }

    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)       
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                JumpFX();
            }
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
                JumpFX();
            }       
        }
        //Wall Jump
        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); //jump away from the wall
            wallJumpTimer = 0;
            JumpFX();

            //force flip
            if(transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); 
        }
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump"); 
        smokeFX.Play();
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void ProcessGravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if(!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
            speedFX.transform.localScale = ls;

            if(rb.linearVelocity.y == 0)
            {
                smokeFX.Play();
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheckPos.position, wallCheckSize);
    }
}

