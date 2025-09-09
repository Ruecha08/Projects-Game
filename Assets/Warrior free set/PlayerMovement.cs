using UnityEngine;
using System.Collections.Generic;
using MagicPigGames;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float normalGravity = 2.5f;

    [Header("Dash Settings")]
    public float dashSpeed = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private float dashTime;
    private float lastDashTime;

    [Header("Attack Settings")]
    public int attackDamage = 20;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public Transform attackPoint;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public float qAttackCooldown = 3.5f;
    private float lastQAttackTime;

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
    public float healthRegenRate = 5f;
    public float manaRegenRate = 10f;
    public PlayerStats playerHealth;

    [Header("Ground & Jump")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public int maxJumpCount = 2;
    private int jumpCount;
    private bool isGrounded;

    [Header("Climb Settings")]
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private bool isOnLadder = false;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;
    private float moveInput;
    private bool isInNoJumpZone = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        lastAttackTime = -attackCooldown;
        lastQAttackTime = -qAttackCooldown;
        currentMana = maxMana;
        jumpCount = 0;

        // ตรวจสอบการตั้งค่า
        if (groundCheck == null) Debug.LogError("groundCheck is not assigned.");
        if (attackPoint == null) Debug.LogError("AttackPoint not assigned.");
        if (manaBar == null) Debug.LogError("ManaBar not assigned.");
        if (playerHealth == null) Debug.LogError("PlayerHealth not assigned.");
    }

    void Update()
    {
        // ฟื้นฟูเลือดและมานา
        if (playerHealth != null)
            playerHealth.Heal(healthRegenRate * Time.deltaTime);

        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
            UpdateManaBar();
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !isDashing)
            jumpCount = 0;

        // Dash input (คีย์บอร์ด)
        if (!isDashing && Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && currentMana >= dashManaCost)
        {
            StartDash();
            currentMana -= dashManaCost;
            UpdateManaBar();
        }

        if (isDashing)
        {
            float dashDirection = Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(dashDirection * dashSpeed, 0);
            dashTime -= Time.deltaTime;
            if (dashTime <= 0) StopDash();
            return; // ป้องกัน move/flip override velocity
        }

        // Climb
        if (isClimbing)
        {
            float climbInput = Input.GetAxisRaw("Vertical");
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

        // Move (คีย์บอร์ด/มือถือ)
        float keyboardInput = Input.GetAxisRaw("Horizontal"); 
        float finalInput = keyboardInput != 0 ? keyboardInput : moveInput; // ถ้ามีคีย์บอร์ด ใช้คีย์บอร์ดก่อน
        rb.velocity = new Vector2(finalInput * moveSpeed, rb.velocity.y);

        // Flip character
        if (finalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(finalInput) * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Jump (คีย์บอร์ด Space)
        if (isGrounded) animator.SetBool("isJumping", false);

        if (Input.GetKeyDown(KeyCode.Space) && !isInNoJumpZone)
        {
            if (isGrounded || jumpCount < maxJumpCount)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                animator.SetBool("isJumping", true);
            }
        }

        // Attack (คลิกซ้าย)
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        // Q Attack (กด Q)
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= lastQAttackTime + qAttackCooldown && currentMana >= qAttackManaCost)
        {
            animator.SetTrigger("QAttack");
            lastQAttackTime = Time.time;
            currentMana -= qAttackManaCost;
            UpdateManaBar();
        }

        // Animator
        animator.SetBool("isplayerRun", finalInput != 0);
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
        animator.SetBool("isDashing", false);
    }

    private void UpdateManaBar()
    {
        if (manaBar != null)
            manaBar.SetProgress(currentMana / maxMana);
    }

    public void DealDamage() => ApplyDamage(false);
    public void DealDamageQ() => ApplyDamage(true);

    private void ApplyDamage(bool isQAttack)
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        HashSet<GameObject> hitObjects = new HashSet<GameObject>();

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (hitObjects.Contains(enemyCollider.gameObject)) continue;

            // มอนสเตอร์ทั่วไป
            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                hitObjects.Add(enemyCollider.gameObject);
            }

            // บอส
            BossStats boss = enemyCollider.GetComponent<BossStats>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
                hitObjects.Add(enemyCollider.gameObject);
            }

            // Q Attack Stun
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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NoJumpZone")) isInNoJumpZone = true;
        if (other.CompareTag("Ladder")) isOnLadder = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NoJumpZone")) isInNoJumpZone = false;
        if (other.CompareTag("Ladder")) { isOnLadder = false; isClimbing = false; }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && isOnLadder)
        {
            if (Input.GetAxisRaw("Vertical") != 0) isClimbing = true;
        }
    }

    // -------------------------------
    // 📱 ฟังก์ชันสำหรับปุ่มมือถือ
    // -------------------------------

    // เดิน
    public void MoveRightButtonDown() => moveInput = 1f;
    public void MoveLeftButtonDown() => moveInput = -1f;
    public void MoveButtonUp() => moveInput = 0f;

    // กระโดด
    public void JumpButton()
    {
        if (isGrounded || jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            animator.SetBool("isJumping", true);
        }
    }

    // Dash
    public void DashButton()
    {
        if (!isDashing && Time.time >= lastDashTime + dashCooldown && currentMana >= dashManaCost)
        {
            StartDash();
            currentMana -= dashManaCost;
            UpdateManaBar();
        }
    }

    // โจมตี
    public void AttackButton()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // Q Attack
    public void QAttackButton()
    {
        if (Time.time >= lastQAttackTime + qAttackCooldown && currentMana >= qAttackManaCost)
        {
            animator.SetTrigger("QAttack");
            lastQAttackTime = Time.time;
            currentMana -= qAttackManaCost;
            UpdateManaBar();
        }
    }
}
