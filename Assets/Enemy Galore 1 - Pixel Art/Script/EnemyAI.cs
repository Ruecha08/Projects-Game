using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Movement and Patrol Variables
    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    private Vector2 initialPosition;
    private int moveDirection = 1;

    // Combat Variables
    public float chaseRange = 8f; // ระยะที่มอนสเตอร์จะเริ่มไล่ตามผู้เล่น
    public float attackRange = 1.5f; // ระยะโจมตี
    public float attackCooldown = 2.0f;
    public int attackDamage = 10;
    private float nextAttackTime = 0f;

    // Component References
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Animator animator;

    // State Variables
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        // ค้นหา GameObject ของผู้เล่นที่มี Tag เป็น "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= chaseRange)
            {
                isChasing = true;
                if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
            else
            {
                isChasing = false;
            }
        }
        
        // Control enemy movement
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Pass movement speed to the animator
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void Patrol()
    {
        // Set horizontal velocity for patrolling
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        // Check if the monster has reached the patrol range
        if (Mathf.Abs(transform.position.x - initialPosition.x) >= patrolRange)
        {
            moveDirection *= -1;
            Flip();
        }
    }

    void ChasePlayer()
    {
        // Determine direction to the player
        float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
        
        // Move towards the player
        rb.velocity = new Vector2(directionToPlayer * moveSpeed, rb.velocity.y);

        // Flip the monster to face the player
        if (directionToPlayer > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (directionToPlayer < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
    }
    
    void AttackPlayer()
    {
        Debug.Log("Enemy is attacking the player!");
        // ส่วนนี้คือ logic การโจมตี
        // เช่น เล่น animation โจมตี
        animator.SetTrigger("Attack");
        // ลดเลือดผู้เล่น
        // Health playerHealth = playerTransform.GetComponent<Health>();
        // if (playerHealth != null)
        // {
        //     playerHealth.TakeDamage(attackDamage);
        // }
    }

    void Flip()
    {
        // Flip the monster's visual scale
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
}