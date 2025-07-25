using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float jumpVelocity;
    [SerializeField] private Vector3 footOffset;
    [SerializeField] private float footRadius = 0.1f;
    [SerializeField] private LayerMask groundLayerMask;

    private bool isOnGround;
    private bool canDoubleJump;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Vector3 attackOffset = new Vector3(0.5f, 0f, 0f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ตรวจว่าติดพื้นไหม
        CheckGrounded();

        // อัปเดตสถานะการกระโดด
        animator.SetBool("isJumping", !isOnGround);

        // อ่านการเคลื่อนไหวแนวนอน
        float moveInput = Input.GetAxisRaw("Horizontal");

        // อัปเดตทิศทางหันก่อนการโจมตี
        bool facingLeft = moveInput < 0;
        spriteRenderer.flipX = facingLeft;

        // ปรับตำแหน่ง attackPoint ให้ตรงทิศ
        if (attackPoint != null)
        {
            attackPoint.localPosition = facingLeft ? -attackOffset : attackOffset;
        }

        // Trigger อนิเมชันโจมตี
        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }

        // Trigger อนิเมชัน Hurt
        if (Input.GetKeyDown(KeyCode.I))
        {
            animator.SetTrigger("HurtTrigger");
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // ขยับตัวละคร
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // อนิเมชันการวิ่ง
        animator.SetBool("isplayerRun", moveInput != 0);

        // ตั้งค่า Double Jump
        if (isOnGround)
        {
            canDoubleJump = true;
        }

        // กระโดด
        if (Input.GetButtonDown("Jump"))
        {
            if (isOnGround)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        Vector3 checkPosition = transform.position + footOffset;
        Collider2D hitCollider = Physics2D.OverlapCircle(checkPosition, footRadius, groundLayerMask);
        isOnGround = hitCollider != null;
    }

    private void Attack()
    {
        animator.SetTrigger("AttackTrigger");

        if (attackPoint == null)
        {
            Debug.LogWarning("attackPoint not assigned!");
            return;
        }

        // ตรวจจับศัตรูในระยะ
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("ตีโดนศัตรู: " + enemy.name);
            // enemy.GetComponent<Enemy>()?.TakeDamage(damageAmount); // เพิ่มฟังก์ชันโจมตีศัตรูของคุณเอง
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isOnGround ? Color.green : Color.red;
        Gizmos.color = canDoubleJump && !isOnGround ? Color.blue : Gizmos.color;
        Gizmos.DrawWireSphere(transform.position + footOffset, footRadius);

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
