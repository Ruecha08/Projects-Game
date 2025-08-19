using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 15f;
    
    // ตัวแปรสำหรับ Dash
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    // ตัวแปรสำหรับ Attack Cooldown
    public float attackCooldown = 1.0f; // กำหนดเวลา cooldown ของการคลิกซ้าย
    private float lastAttackTime;

    // ตัวแปรสำหรับ Q-Attack Cooldown
    public float qAttackCooldown = 3.5f; // กำหนดเวลา cooldown ของการโจมตีด้วยปุ่ม Q
    private float lastQAttackTime;
    
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // กำหนดเวลาเริ่มต้นให้โจมตีได้ทันที
        lastAttackTime = -attackCooldown;
        lastQAttackTime = -qAttackCooldown;
    }

    void Update()
    {
        // การตรวจจับการพุ่งตัว
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }

        // หากอยู่ในสถานะพุ่งตัว
        if (isDashing)
        {
            // นับเวลาถอยหลังการพุ่งตัว
            if (dashTime > 0)
            {
                // เคลื่อนที่ด้วยความเร็วพุ่งตัว
                rb.velocity = new Vector2(spriteRenderer.flipX ? -dashSpeed : dashSpeed, 0);
                dashTime -= Time.deltaTime;
            }
            else
            {
                // หยุดการพุ่งตัว
                StopDash();
            }
        }
        else // หากไม่อยู่ในสถานะพุ่งตัว
        {
            // ตรวจสอบการติดพื้น
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // การเคลื่อนที่ซ้าย-ขวา
            moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // สลับการหันซ้าย-ขวา
            if (moveInput > 0) spriteRenderer.flipX = false;
            else if (moveInput < 0) spriteRenderer.flipX = true;

            // กระโดด
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            // โจมตีด้วยการคลิกซ้าย พร้อม Cooldown
            if (Input.GetMouseButtonDown(0) && isGrounded && Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }

            // โจมตีด้วยปุ่ม Q พร้อม Cooldown
            if (Input.GetKeyDown(KeyCode.Q) && isGrounded && Time.time >= lastQAttackTime + qAttackCooldown)
            {
                animator.SetTrigger("QAttack");
                lastQAttackTime = Time.time;
            }
        }

        // ส่งค่าไป Animator ตามตาราง
        animator.SetBool("isplayerRun", moveInput != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y < -0.1f);
    }
    
    // ฟังก์ชันเริ่มการพุ่งตัว
    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        lastDashTime = Time.time;
        animator.SetBool("isDashing", true); // ตั้งค่า Parameter ใน Animator
    }
    
    // ฟังก์ชันหยุดการพุ่งตัว
    void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero; // ทำให้ความเร็วเป็นศูนย์เพื่อหยุดการพุ่งตัวทันที
        animator.SetBool("isDashing", false); // ตั้งค่า Parameter ใน Animator
    }

    void FixedUpdate()
    {
        // ตรวจสอบการติดพื้นซ้ำ (ป้องกัน delay)
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
}