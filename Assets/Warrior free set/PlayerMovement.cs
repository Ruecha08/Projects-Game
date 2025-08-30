using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public float dashSpeed = 15f;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastAttackTime = -attackCooldown;
        lastQAttackTime = -qAttackCooldown;
    }

    void Update()
    {
        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }

        if (isDashing)
        {
            if (dashTime > 0)
            {
                rb.velocity = new Vector2(spriteRenderer.flipX ? -dashSpeed : dashSpeed, 0);
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

        // แก้ไข: เปลี่ยนการพลิกตัวจาก SpriteRenderer เป็น Transform
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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isInNoJumpZone)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Attack with Left Click
        if (Input.GetMouseButtonDown(0) && isGrounded && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        // Q Attack (สร้างดาเมจ + Stun)
        if (Input.GetKeyDown(KeyCode.Q) && isGrounded && Time.time >= lastQAttackTime + qAttackCooldown)
        {
            animator.SetTrigger("QAttack");
            lastQAttackTime = Time.time;
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
        lastDashTime = Time.time;
        animator.SetBool("isDashing", true);
    }

    void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("isDashing", false);
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // วาด Gizmos วงกลมโจมตีที่ตำแหน่ง AttackPoint
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    // 🎯 เรียกจาก Animation Event ปกติ (Left Click Attack)
    public void DealDamage()
    {
        ApplyDamage(false);
    }

    // 🎯 เรียกจาก Animation Event ของ Q Attack
    public void DealDamageQ()
    {
        ApplyDamage(true);
    }

    // ฟังก์ชันรวมการคำนวณดาเมจ + Stun
    private void ApplyDamage(bool isQAttack)
    {
        // ใช้ตำแหน่งของ attackPoint ในการหาศัตรู
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint not assigned. Please assign the AttackPoint Transform in the Inspector.");
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

            // ถ้าเป็น QAttack → สั่งให้ติด Stun
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