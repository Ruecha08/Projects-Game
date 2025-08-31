using UnityEngine;
using UnityEngine.UI; // For the Slider UI component

public class Health : MonoBehaviour
{
    public float maxHealth = 100f; 
    public float currentHealth;

    public Slider healthBarSlider; 
    private Animator animator; 
    private bool isDead = false; // ✅ ป้องกันการตายซ้ำ

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
        if (isDead) return; // ✅ ถ้าตายแล้วไม่โดนซ้ำ

        if (currentHealth > 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit"); // เล่นอนิเมชันโดนตี
            }
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
        if (isDead) return; // ถ้าตายแล้วห้าม Heal
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

        if (animator != null)
        {
            animator.SetTrigger("Death"); // ✅ เล่น Animation Death
        }

        // ปิด collider และ rigidbody กันไม่ให้ขยับอีก
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // ✅ ทำลาย GameObject หลังอนิเม Death เล่นจบ (2 วินาที หรือปรับตามความยาว animation)
        Destroy(gameObject, 2f);
    }
}
