using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;   // AudioSource ของมอนสเตอร์
    public AudioClip hurtSound;       // เสียงโดนโจมตี
    public AudioClip deathSound;      // เสียงตอนตาย

    // ฟังก์ชันเล่นเสียงบาดเจ็บ
    public void PlayHurtSound()
    {
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
    }

    // ฟังก์ชันเล่นเสียงตาย
    public void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }
}
