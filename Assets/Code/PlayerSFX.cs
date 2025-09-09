using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;  // ตัวเล่นเสียง

    [Header("Attack SFX")]
    public AudioClip attackClip;     // เสียงโจมตีปกติ
    public AudioClip qAttackClip;    // เสียงโจมตี Q

    // เรียกจาก Animation Event หรือสคริปต์อื่น
    public void PlayAttack()
    {
        if (attackClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackClip);
        }
    }

    public void PlayQAttack()
    {
        if (qAttackClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(qAttackClip);
        }
    }
}
