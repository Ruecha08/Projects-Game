using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthBarSlider;
    private Animator animator;
    private bool isDead = false;

    [Header("Score Settings")]
    public int scoreValue = 10; // คะแนนที่ได้จากการฆ่าศัตรู

    private EnemyAudio enemyAudio; 
    private EnemyController enemyController; // ✅ อ้างอิงไปที่ EnemyController

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAudio = GetComponent<EnemyAudio>();
        enemyController = GetComponent<EnemyController>(); // ✅ หา EnemyController
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

        // ✅ เล่นเสียงบาดเจ็บ
        if (enemyAudio != null)
        {
            enemyAudio.PlayHurtSound();
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

    void UpdateHealthBar()
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

        // ✅ บวกคะแนน
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
            Debug.Log("Added score: " + scoreValue);
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // ✅ เล่นเสียงตาย
        if (enemyAudio != null)
        {
            enemyAudio.PlayDeathSound();
        }

        // ✅ เรียก EnemyController ให้หยุดการเคลื่อนไหว
        if (enemyController != null)
        {
            enemyController.Die();
        }

        // ลบ object หลังจาก 2 วินาที
        Destroy(gameObject, 2f);
    }
}
