using UnityEngine;
using TMPro; // ถ้าใช้ TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int score = 0;

    [Header("UI")]
    public TextMeshProUGUI scoreText;          // สำหรับแสดงตอนเล่นเกม (ไม่แสดงคำว่า Score)
    public TextMeshProUGUI gameOverScoreText;  // สำหรับแสดงหน้า GameOver (แสดงคำว่า Score)

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("ScoreManager instance set!");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("ScoreManager instance destroyed duplicate!");
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("AddScore called! Amount: " + amount + " | Total Score: " + score);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
            Debug.Log("UpdateScoreUI called! Current Score: " + score);
        }
        else
        {
            Debug.LogWarning("scoreText is not assigned in ScoreManager!");
        }
    }

    // ฟังก์ชันเรียกตอน GameOver
    public void ShowGameOverScore()
    {
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score: " + score;
            Debug.Log("ShowGameOverScore called! Score: " + score);
        }
        else
        {
            Debug.LogWarning("gameOverScoreText is not assigned in ScoreManager!");
        }
    }

    // ให้เรียกได้จาก PlayerStats
    public int GetScore()
    {
        Debug.Log("GetScore called! Score: " + score);
        return score;
    }

    // รีเซ็ตคะแนนตอนเริ่มเกมใหม่ (ถ้าต้องการ)
    public void ResetScore()
    {
        score = 0;
        Debug.Log("ResetScore called! Score reset to 0");
        UpdateScoreUI();
    }
}
