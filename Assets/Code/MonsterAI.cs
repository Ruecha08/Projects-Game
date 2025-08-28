using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    // A flag to check if the player is within the attack range
    private bool playerInRange = false;

    // The cooldown time between attacks, can be adjusted in the Inspector
    public float attackCooldown = 2.0f; 
    
    // The time when the monster can attack again
    private float nextAttackTime = 0f;

    // This function runs every frame
    private void Update()
    {
        // Check if the player is in range AND if enough time has passed since the last attack
        if (playerInRange && Time.time >= nextAttackTime)
        {
            // If both conditions are met, start the attack
            StartAttack();

            // Set the time for the next possible attack
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // This function is called when a Trigger Collider enters this object's Trigger Collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player using its Tag
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player has entered the attack range!");
        }
    }

    // This function is called when a Trigger Collider exits this object's Trigger Collider
    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object is the player using its Tag
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player has left the attack range.");
        }
    }

    // This is the function that contains the attack logic
    private void StartAttack()
    {
        Debug.Log("Monster is starting to attack the player.");
        // Place your actual attack logic here, such as:
        // - Playing an attack animation
        // - Dealing damage to the player
        // - Playing a sound effect
    }
}