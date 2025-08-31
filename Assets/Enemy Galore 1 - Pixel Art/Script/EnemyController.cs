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

    public float chaseRangeX = 8f;
    public float chaseRangeY = 4f;

    private Transform playerTransform;
    public float idleTime = 2f;
    private float idleTimer;
    private bool isPatrolling = false;

    // ---------------- Combat Variables ----------------
    public float attackRange = 1.5f;
    public float attackCooldown = 2.0f;
    public int attackDamage = 10;
    private float nextAttackTime = 0f;
    
    public Transform attackPoint;
    public LayerMask playerLayer;

    // ---------------- Components ----------------
    private Rigidbody2D rb;
    private Animator animator;

    // ---------------- Stun Variables ----------------
    private bool isStunned = false;
    private Coroutine stunCoroutine;
    public GameObject stunEffectPrefab;
    private GameObject activeStunEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        idleTimer = idleTime;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isStunned)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("Run", false);
            return;
        }
        
        if (playerTransform != null)
        {
            float distanceX = Mathf.Abs(transform.position.x - playerTransform.position.x);
            float distanceY = Mathf.Abs(transform.position.y - playerTransform.position.y);
            bool isInChaseRange = (distanceX <= chaseRangeX && distanceY <= chaseRangeY);

            if (isInChaseRange)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                
                if (distanceToPlayer <= attackRange)
                {
                    HandleAttack();
                    isPatrolling = false;
                }
                else
                {
                    ChasePlayer();
                    isPatrolling = false;
                }
            }
            else
            {
                if (!isPatrolling)
                {
                    rb.velocity = Vector2.zero;
                    animator.SetBool("Run", false);
                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0)
                    {
                        isPatrolling = true;
                        idleTimer = idleTime;
                    }
                }
                else
                {
                    Patrol();
                }
            }
        }
        else
        {
            Patrol();
        }
    }
    
    void HandleAttack()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("Run", false);
        FlipToTarget(playerTransform.position);

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Patrol()
    {
        if (Mathf.Abs(transform.position.x - initialPosition.x) >= patrolRange)
        {
            moveDirection *= -1;
            FlipToTarget(new Vector2(initialPosition.x + (patrolRange * moveDirection), transform.position.y));
        }

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        animator.SetBool("Run", true);
    }

    void ChasePlayer()
    {
        float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
        rb.velocity = new Vector2(directionToPlayer * moveSpeed, rb.velocity.y);
        animator.SetBool("Run", true);
        FlipToTarget(playerTransform.position);
    }
    
    public void AttackPlayer()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        
        foreach(Collider2D player in hitPlayers)
        {
            // 🎯 แก้ไข: เปลี่ยนไปหา PlayerStats แทน PlayerHealth
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if(playerStats != null)
            {
                playerStats.TakeDamage(attackDamage);
            }
        }
    }

    void FlipToTarget(Vector2 targetPosition)
    {
        if (targetPosition.x > transform.position.x && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (targetPosition.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    public void Stun(float duration)
    {
        if (!isStunned)
        {
            if (stunCoroutine != null) StopCoroutine(stunCoroutine); 
            
            stunCoroutine = StartCoroutine(StunCoroutine(duration));
        }
    }
    
    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        animator.SetBool("Stunned", true);

        if (stunEffectPrefab != null)
        {
            if (activeStunEffect != null) Destroy(activeStunEffect);
            Vector3 effectPos = transform.position + new Vector3(0, 1.5f, 0);
            activeStunEffect = Instantiate(stunEffectPrefab, effectPos, Quaternion.identity, transform);
        }
        
        yield return new WaitForSeconds(duration);

        isStunned = false;
        animator.SetBool("Stunned", false);
        if (activeStunEffect != null) Destroy(activeStunEffect);
        activeStunEffect = null;
    }

    public void Die()
    {
        animator.SetTrigger("Death");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector2(chaseRangeX * 2, chaseRangeY * 2));
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(initialPosition, new Vector2(patrolRange * 2, 1));
        
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}