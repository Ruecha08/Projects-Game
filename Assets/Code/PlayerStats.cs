using UnityEngine;
using UnityEngine.SceneManagement; // เพิ่มบรรทัดนี้เข้ามา
using MagicPigGames;

public class PlayerStats : MonoBehaviour
{
    // เพิ่มตัวแปรสำหรับเก็บชื่อฉากที่จะโหลด
    public string sceneToLoadOnDeath;

    public float maxHealth = 100f;
    public float currentHealth;
    public ProgressBar healthBar;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
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
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetProgress(currentHealth / maxHealth);
        }
    }

    private void Die()
    {
        // โค้ดนี้จะตรวจสอบว่ามีชื่อฉากที่กำหนดไว้หรือไม่
        if (!string.IsNullOrEmpty(sceneToLoadOnDeath))
        {
            // ถ้ามี ให้โหลดฉากตามชื่อที่กำหนดใน Inspector
            SceneManager.LoadScene(sceneToLoadOnDeath);
        }
        else
        {
            // ถ้าไม่มี ให้โหลดฉากปัจจุบันซ้ำ (เหมือนเดิม)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}