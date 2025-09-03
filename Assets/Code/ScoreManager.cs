using UnityEngine;
using TMPro; // ถ้าใช้ TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // ✅ ชื่อตรงกับ Health.cs

    private int score = 0;
    public TextMeshProUGUI scoreText; // ถ้าใช้ TMP
    // public Text scoreText; // ถ้าใช้ UI Text ธรรมดา

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = " " + score; 
    }
}
