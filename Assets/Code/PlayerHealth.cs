using UnityEngine;
using MagicPigGames; 

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f; 
    public float currentHealth;
    
    public ProgressBar healthBar;
    
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>(); 
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    public void TakeDamage(float damage) 
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); 
        
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float progressValue = currentHealth / maxHealth;
            healthBar.SetProgress(progressValue);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}