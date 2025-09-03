using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f; 
    public float currentHealth;

    public Slider healthBarSlider; 
    private Animator animator; 
    private bool isDead = false;

    [Header("Score Settings")]
    public int scoreValue = 10; // ✅ คะแนนของศัตรู

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (currentHealth > 0 && animator != null)
        {
            animator.SetTrigger("Hit");
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
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
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " is dead!");

        // ✅ เพิ่มคะแนนเมื่อศัตรูตาย
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
            Debug.Log("Added score: " + scoreValue);
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Destroy(gameObject, 2f);
    }
}
