using UnityEngine;
using MagicPigGames; // สำหรับ ProgressBar

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 100f; 
    public float currentHealth;
    public ProgressBar healthBar;

    private Animator animator;
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (animator != null)
            animator.SetTrigger("Hurt");

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

    public void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.SetProgress(currentHealth / maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Death");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        // ✅ อ่านคะแนนจาก ScoreManager (ต้องมี GetScore() ใน ScoreManager)
        int finalScore = 0;
        if (ScoreManager.instance != null)
            finalScore = ScoreManager.instance.GetScore();

        Debug.Log("Player Died! Final Score: " + finalScore);

        // ❌ ไม่เรียก GameOverManager
    }
}
