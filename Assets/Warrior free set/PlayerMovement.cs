using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MagicPigGames;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public float dashSpeed = 5f; // 🎯 ปรับค่า dashSpeed เป็น 5
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public float attackCooldown = 2.0f;
    private float lastAttackTime;

    public float qAttackCooldown = 3.5f;
    private float lastQAttackTime;

    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private float climbInput;
    private bool isOnLadder = false;

    public float normalGravity = 2.5f;
    
    // เพิ่มตัวแปรสำหรับกระโดดสองครั้ง
    public int maxJumpCount = 2;
    private int jumpCount;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private float moveInput;

    private bool isDashing;
    private float dashTime;
    private float lastDashTime;

    private bool isInNoJumpZone = false;

    public int attackDamage = 20;
    public float attackRange = 1f;
    public LayerMask enemyLayer;

    // เพิ่มตัวแปรสำหรับจุดโจมตีของผู้เล่น
    public Transform attackPoint;

    [Header("Stun Settings")]
    public float stunDuration = 2f;
    public GameObject stunEffectPrefab;

    [Header("Mana Settings")]
    public float maxMana = 100f;
    public float dashManaCost = 20f;
    public float qAttackManaCost = 30f;
    public ProgressBar manaBar;
    private float currentMana;

    [Header("Healing & Mana Regen Settings")]
    public float healthRegenRate = 5f; // เลือดฟื้นฟู 5 ต่อวินาที
    public float manaRegenRate = 10f; // มานาฟื้นฟู 10 ต่อวินาที
    public PlayerStats playerHealth; // 🎯 เพิ่มตัวแปรสำหรับเชื่อมกับ Health สคริปต์

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastAttackTime = -attackCooldown;
        lastQAttackTime = -qAttackCooldown;

        currentMana = maxMana;
        jumpCount = 0;

        // ตรวจสอบการตั้งค่า
        if (groundCheck == null) Debug.LogError("groundCheck is not assigned.");
        if (attackPoint == null) Debug.LogError("AttackPoint not assigned.");
        if (manaBar == null) Debug.LogError("ManaBar not assigned.");
        if (playerHealth == null) Debug.LogError("PlayerHealth not assigned. Please add the Health.cs script to the player and assign it.");
    }

    void Update()
    {
        // ฟื้นฟูเลือดและมานา
        if (playerHealth != null)
        {
            playerHealth.Heal(healthRegenRate * Time.deltaTime);
        }

        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana); // ไม่ให้มานาเกินค่าสูงสุด
            UpdateManaBar();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && currentMana >= dashManaCost)
        {
            StartDash();
            currentMana -= dashManaCost;
            UpdateManaBar();
        }

        if (isDashing)
        {
            if (dashTime > 0)
            {
                float dashDirection = transform.localScale.x;
                rb.velocity = new Vector2(dashDirection * dashSpeed, 0);
                dashTime -= Time.deltaTime;
            }
            else
            {
                StopDash();
            }
            return;
        }

        // Climb
        if (isClimbing)
        {
            climbInput = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, climbInput * climbSpeed);
            animator.SetBool("isClimbing", true);
            animator.SetFloat("climbSpeed", Mathf.Abs(climbInput));
            rb.gravityScale = 0;
            return;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }

        // Move
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // เปลี่ยนการพลิกตัวจาก SpriteRenderer เป็น Transform
        if (moveInput > 0 && transform.localScale.x < 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        else if (moveInput < 0 && transform.localScale.x > 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }

        // Logic การกระโดดที่ถูกแก้ไข
        if (isGrounded)
        {
            jumpCount = 0;
            animator.SetBool("isJumping", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isInNoJumpZone)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount = 1;
                animator.SetBool("isJumping", true);
            }
            else if (jumpCount < maxJumpCount)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                animator.SetBool("isJumping", true);
            }
        }

        // Attack with Left Click
        if (Input.GetMouseButtonDown(0) && isGrounded && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        // Q Attack (สร้างดาเมจ + Stun)
        if (Input.GetKeyDown(KeyCode.Q) && isGrounded && Time.time >= lastQAttackTime + qAttackCooldown && currentMana >= qAttackManaCost)
        {
            animator.SetTrigger("QAttack");
            lastQAttackTime = Time.time;
            currentMana -= qAttackManaCost;
            UpdateManaBar();
        }

        // Animator states
        animator.SetBool("isplayerRun", moveInput != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y < -0.1f);
        animator.SetBool("isClimbing", false);
    }

    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        rb.gravityScale = 0;
        lastDashTime = Time.time;
        animator.SetBool("isDashing", true);
    }

    void StopDash()
    {
        isDashing = false;
        rb.gravityScale = normalGravity;
        rb.velocity = Vector2.zero;
        animator.SetBool("isDashing", false);
    }

    private void UpdateManaBar()
    {
        if (manaBar != null)
        {
            manaBar.SetProgress(currentMana / maxMana);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        else
        {
            Debug.LogWarning("groundCheck is not assigned in the Inspector.");
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    public void DealDamage()
    {
        ApplyDamage(false);
    }

    public void DealDamageQ()
    {
        ApplyDamage(true);
    }

    private void ApplyDamage(bool isQAttack)
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint not assigned.");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        HashSet<GameObject> hitObjects = new HashSet<GameObject>();

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (hitObjects.Contains(enemyCollider.gameObject)) continue;

            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                hitObjects.Add(enemyCollider.gameObject);
            }

            if (isQAttack)
            {
                EnemyController enemyController = enemyCollider.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.Stun(stunDuration);

                    if (stunEffectPrefab != null)
                    {
                        Vector3 effectPos = enemyCollider.transform.position + new Vector3(0, 1.5f, 0);
                        GameObject effect = Instantiate(stunEffectPrefab, effectPos, Quaternion.identity, enemyCollider.transform);
                        Destroy(effect, stunDuration);
                    }
                }
            }
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NoJumpZone")) isInNoJumpZone = true;
        else if (other.CompareTag("Ladder")) isOnLadder = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NoJumpZone")) isInNoJumpZone = false;
        else if (other.CompareTag("Ladder")) { isOnLadder = false; isClimbing = false; }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && isOnLadder)
        {
            if (Input.GetAxisRaw("Vertical") != 0) isClimbing = true;
        }
    }
}