using UnityEngine;
using UnityEngine.UI; // ต้องเพิ่ม namespace นี้เข้ามา

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // เพิ่มตัวแปรสำหรับอ้างอิงถึงแถบเลือด
    public Slider healthBar; 

    void Start()
    {
        currentHealth = maxHealth;
        // ตั้งค่า MaxValue และค่าเริ่มต้นของ Slider
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " damage. Current Health: " + currentHealth);
        
        // อัปเดตค่าของ Slider ให้ตรงกับค่าเลือดปัจจุบัน
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}