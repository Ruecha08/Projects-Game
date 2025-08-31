using UnityEngine;
using MagicPigGames; 

public class PlayerStats : MonoBehaviour
{
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
        Destroy(gameObject);
    }
}