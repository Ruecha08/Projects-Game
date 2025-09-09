using UnityEngine;
using UnityEngine.SceneManagement; // ✅ ใช้สำหรับโหลด Scene
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

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // ✅ ชื่อ Scene หน้า MainMenu

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        UpdateHealthBar();

        // ซ่อน UI ตอนเริ่ม
        if (healthBarUI != null)
            healthBarUI.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        // ตรวจสอบระยะผู้เล่น
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

        // ✅ เพิ่มคะแนนเมื่อฆ่าบอส
        if (ScoreManager.instance != null)
            ScoreManager.instance.AddScore(scoreValue);

        // ปิด collider + rigidbody
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // ซ่อนหลอดเลือด
        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        // ✅ โหลดหน้า MainMenu ทันที (หน่วงเวลาเล็กน้อยให้เล่นอนิเม Death ได้)
        Invoke(nameof(LoadMainMenu), 2f);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
