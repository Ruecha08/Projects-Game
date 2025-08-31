using UnityEngine;
using UnityEngine.UI; // For the Slider UI component

public class Health : MonoBehaviour
{
    public float maxHealth = 100f; // 🎯 Changed to float for more versatile damage
    public float currentHealth;

    // Optional references to UI and Animator
    public Slider healthBarSlider; // Drag the health bar UI Slider here
    private Animator animator; // Will be automatically found on the GameObject

    void Awake()
    {
        // Get the Animator component on this GameObject
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    public void TakeDamage(float damage) // 🎯 Changed to float
    {
        // Play Hit animation if not already at zero health
        if (currentHealth > 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
        
        currentHealth -= damage;
        // Use Mathf.Max to clamp health at 0
        currentHealth = Mathf.Max(currentHealth, 0); 
        
        UpdateHealthBar(); 
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // You can add a Heal method too if needed
    public void Heal(float amount)
    {
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
        Debug.Log(gameObject.name + " is dead!");
        Destroy(gameObject);
    }
}