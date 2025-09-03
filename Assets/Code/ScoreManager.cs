using UnityEngine;
using TMPro; // ถ้าใช้ TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private int score = 0;

    public TextMeshProUGUI scoreText; // UI แสดงคะแนน

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // เพิ่มคะแนน
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // ✅ ฟังก์ชัน GetScore() สำหรับเรียกคะแนนปัจจุบัน
    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString(); // แสดงแค่ตัวเลข
    }
}
