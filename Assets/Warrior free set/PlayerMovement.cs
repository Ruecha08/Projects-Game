using UnityEngine;
using System.Collections;
using System.Collections.Generic; // **เพิ่มบรรทัดนี้เพื่อใช้งาน HashSet**
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    
    // ตัวแปรสำหรับ Dash
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    // ตัวแปรสำหรับ Attack Cooldown
    public float attackCooldown = 2.0f;
    private float lastAttackTime;

    // ตัวแปรสำหรับ Q-Attack Cooldown
    public float qAttackCooldown = 3.5f;
    private float lastQAttackTime;
    
    // ตัวแปรสำหรับปีน
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private float climbInput;
    private bool isOnLadder = false;
    
    // ตัวแปรสำหรับแรงโน้มถ่วงปกติ
    public float normalGravity = 2.5f; 
    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private float moveInput;
    
    // ตัวแปรสำหรับ Dash
    private bool isDashing;
    private float dashTime;
    private float lastDashTime;
    
    // ตัวแปรสำหรับพื้นที่ห้ามกระโดด
    private bool isInNoJumpZone = false;

    // เพิ่มตัวแปรเหล่านี้
    public int attackDamage = 20; // ความเสียหายจากการโจมตี
    public float attackRange = 1f; // ระยะการโจมตี
    public LayerMask enemyLayer; // Layer สำหรับศัตรู
    

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
        // ตรวจสอบการพุ่งตัว
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }

        // หากกำลัง Dash ให้หยุดการทำงานของโค้ดอื่น
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

        // โค้ดการปีน
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

        // โค้ดการเคลื่อนที่ปกติ
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isInNoJumpZone)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // โจมตีด้วยการคลิกซ้าย
        if (Input.GetMouseButtonDown(0) && isGrounded && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            DealDamage(); 
        }

        // โจมตีด้วยปุ่ม Q
        if (Input.GetKeyDown(KeyCode.Q) && isGrounded && Time.time >= lastQAttackTime + qAttackCooldown)
        {
            animator.SetTrigger("QAttack");
            lastQAttackTime = Time.time;
            DealDamage();
        }
    
        // ส่งค่าไป Animator
        animator.SetBool("isplayerRun", moveInput != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y < -0.1f);
        animator.SetBool("isClimbing", false);
    }

    // ฟังก์ชันเริ่มการพุ่งตัว
    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        lastDashTime = Time.time;
        animator.SetBool("isDashing", true);
    }
    
    // ฟังก์ชันหยุดการพุ่งตัว
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
    }
    
    // เพิ่มเมธอดนี้เข้ามา
    void DealDamage()
    {
        Vector2 attackPosition = transform.position;
        // ปรับตำแหน่งการโจมตีให้สอดคล้องกับทิศทางที่ผู้เล่นหันหน้าไป
        if (spriteRenderer.flipX)
        {
            attackPosition.x -= attackRange;
        }
        else
        {
            attackPosition.x += attackRange;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);
        
        // สร้าง HashSet เพื่อเก็บ GameObject ของศัตรูที่ถูกโจมตีไปแล้ว
        HashSet<GameObject> hitObjects = new HashSet<GameObject>();

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // ถ้า GameObject ของศัตรูนี้ถูกเพิ่มเข้าไปใน HashSet แล้ว ให้ข้ามไป
            if (hitObjects.Contains(enemyCollider.gameObject))
            {
                continue;
            }

            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                // เพิ่ม GameObject ของศัตรูลงใน HashSet เพื่อป้องกันการทำดาเมจซ้ำ
                hitObjects.Add(enemyCollider.gameObject);
            }
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NoJumpZone"))
        {
            isInNoJumpZone = true;
        }
        else if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NoJumpZone"))
        {
            isInNoJumpZone = false;
        }
        else if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && isOnLadder)
        {
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                isClimbing = true;
            }
        }
    }
}