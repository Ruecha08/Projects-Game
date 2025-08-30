using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    // ---------------- Movement Variables ----------------
    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    private Vector2 initialPosition;
    private int moveDirection = 1;
    public float chaseRange = 8f;
    private Transform playerTransform;
    public float idleTime = 2f; // เพิ่มตัวแปรสำหรับระยะเวลา Idle
    private float idleTimer;
    private bool isPatrolling = false; // เพิ่มสถานะการลาดตระเวน

    // ---------------- Combat Variables ----------------
    public float attackRange = 1.5f;
    public float attackCooldown = 2.0f;
    public int attackDamage = 10;
    private float nextAttackTime = 0f;

    // ---------------- Components ----------------
    private Rigidbody2D rb;
    private Animator animator;

    // ---------------- Stun Variables ----------------
    public bool isStunned = false;
    private float stunTimer = 0f;
    public GameObject stunEffectPrefab;
    private GameObject activeStunEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        idleTimer = idleTime; // กำหนดค่าเริ่มต้น

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        // ---------------- จัดการ Stun ----------------
        if (isStunned)
        {
            HandleStun();
            return;
        }
        
        // ---------------- การจัดการพฤติกรรม ----------------
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer <= attackRange)
            {
                HandleAttack();
                isPatrolling = false; // หยุดการลาดตระเวน
            }
            else if (distanceToPlayer <= chaseRange)
            {
                ChasePlayer();
                isPatrolling = false; // หยุดการลาดตระเวน
            }
            else
            {
                // ถ้าอยู่นอกระยะทั้งหมด
                if (!isPatrolling)
                {
                    // เริ่มสถานะ Idle ก่อน
                    rb.velocity = Vector2.zero;
                    animator.SetBool("Run", false);
                    idleTimer -= Time.deltaTime;

                    if (idleTimer <= 0)
                    {
                        isPatrolling = true; // เมื่อครบเวลาให้เริ่มลาดตระเวน
                        idleTimer = idleTime; // รีเซ็ตเวลา Idle
                    }
                }
                else
                {
                    // ถ้ากำลังลาดตระเวนอยู่แล้ว
                    Patrol();
                }
            }
        }
        else
        {
            // ถ้าไม่มีผู้เล่น ให้ลาดตระเวน
            Patrol();
        }
    }
    
    // ---------------- Handle Stun ----------------
    void HandleStun()
    {
        stunTimer -= Time.deltaTime;
        rb.velocity = Vector2.zero;
        animator.SetBool("Run", false);
        if (stunTimer <= 0)
        {
            isStunned = false;
            animator.SetBool("Stunned", false);
            if (activeStunEffect != null) Destroy(activeStunEffect);
        }
    }

    // ---------------- Handle Attack ----------------
    void HandleAttack()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("Run", false);

        // หันหน้าเข้าหาผู้เล่น
        if (playerTransform.position.x > transform.position.x && transform.localScale.x < 0)
            Flip();
        else if (playerTransform.position.x < transform.position.x && transform.localScale.x > 0)
            Flip();

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // ---------------- Patrol ----------------
    void Patrol()
    {
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        animator.SetBool("Run", true);

        if (Mathf.Abs(transform.position.x - initialPosition.x) >= patrolRange)
        {
            moveDirection *= -1;
            Flip();
        }
    }

    // ---------------- Chase ----------------
    void ChasePlayer()
    {
        float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
        rb.velocity = new Vector2(directionToPlayer * moveSpeed, rb.velocity.y);
        animator.SetBool("Run", true);

        // Flip
        if (directionToPlayer > 0 && transform.localScale.x < 0)
            Flip();
        else if (directionToPlayer < 0 && transform.localScale.x > 0)
            Flip();
    }
    
    // ---------------- Deal Damage (ถูกเรียกด้วย Animation Event เท่านั้น) ----------------
    void DealDamage()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Player"));
        foreach(Collider2D player in hitPlayers)
        {
            Health playerHealth = player.GetComponent<Health>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    // ---------------- Flip ----------------
    void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    // ---------------- Stun ----------------
    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        animator.SetBool("Stunned", true);
        rb.velocity = Vector2.zero;

        if (stunEffectPrefab != null)
        {
            if (activeStunEffect != null) Destroy(activeStunEffect);
            Vector3 effectPos = transform.position + new Vector3(0, 1.5f, 0);
            activeStunEffect = Instantiate(stunEffectPrefab, effectPos, Quaternion.identity, transform);
        }
    }

    // ---------------- Die ----------------
    public void Die()
    {
        animator.SetTrigger("Death");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    // ---------------- Debug ระยะ ----------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}