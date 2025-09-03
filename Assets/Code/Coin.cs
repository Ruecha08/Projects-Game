using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // ค่าคะแนนต่อเหรียญ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // ถ้าชนผู้เล่น
        {
            ScoreManager.instance.AddScore(coinValue); // เพิ่มคะแนน
            Destroy(gameObject); // ลบเหรียญออกไป
        }
    }
}
