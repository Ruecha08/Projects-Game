using UnityEngine;
using TMPro; // ใช้ถ้าคุณใช้ TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    public TextMeshProUGUI scoreText; // UI ที่จะแสดงคะแนน

    private void Awake()
    {
        // ทำให้เป็น Singleton (เรียกใช้ได้จาก Coin)
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}
