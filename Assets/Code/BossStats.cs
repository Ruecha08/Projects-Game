using UnityEngine;
using MagicPigGames; // ใช้ ProgressBar

public class BossStats : MonoBehaviour
{
    [Header("Boss Health Settings")]
    public float maxHealth = 500f;
    public float currentHealth;

    [Header("UI")]
    public ProgressBar healthBar;       // MagicPigGames ProgressBar
    public GameObject healthBarUI;      // UI ทั้งก้อน (Panel + ProgressBar)

    [Header("Player Detection")]
    public Transform player;            // ตัวผู้เล่น
    public float showHealthRange = 10f; // ระยะที่หลอดเลือดจะปรากฏ

    [Header("Score Settings")]
    public int scoreValue = 100;        // คะแนนที่ได้เมื่อฆ่าบอส

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        UpdateHealthBar();

        if (healthBarUI != null)
            healthBarUI.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        if (player != null && healthBarUI != null)
        {
            float distance = Vector2.Distance(player.position, transform.position);
            healthBarUI.SetActive(distance <= showHealthRange);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (animator != null)
            animator.SetTrigger("Hit");

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float progress = currentHealth / maxHealth;
            healthBar.SetProgress(progress);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Death");

        if (ScoreManager.instance != null)
            ScoreManager.instance.AddScore(scoreValue);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        // ✅ เปิด Portal เมื่อบอสตาย
        if (PortalActivator.instance != null)
            PortalActivator.instance.ActivatePortal();

        Destroy(gameObject, 2f);
    }
}
