using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 15f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
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

        // โจมตี (ปุ่ม J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Attack");
        }

        // ส่งค่าไป Animator ตามตาราง
        animator.SetBool("isplayerRun", moveInput != 0);   // Idle <-> Run
        animator.SetBool("isGrounded", isGrounded);        // Grounded check
        animator.SetFloat("yVelocity", rb.velocity.y);     // Jump / Fall
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y < -0.1f); // Fall
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
