using UnityEngine;
using TMPro; // ถ้าใช้ TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
<<<<<<< HEAD
    private int score = 0;

    public TextMeshProUGUI scoreText; // UI แสดงคะแนน
=======

    private int score = 0;

    [Header("UI")]
    public TextMeshProUGUI scoreText;          // สำหรับแสดงตอนเล่นเกม (ไม่แสดงคำว่า Score)
    public TextMeshProUGUI gameOverScoreText;  // สำหรับแสดงหน้า GameOver (แสดงคำว่า Score)
>>>>>>> parent of a889788 (Revert "ก")

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

    // เพิ่มคะแนน
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("AddScore called! Amount: " + amount + " | Total Score: " + score);
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
<<<<<<< HEAD
            scoreText.text = score.ToString(); // แสดงแค่ตัวเลข
=======
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
>>>>>>> parent of a889788 (Revert "ก")
    }
}
