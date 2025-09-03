using UnityEngine;
using TMPro; // <-- ใช้ TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText; // เปลี่ยนจาก Text เป็น TMP_Text

    private int totalScore = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // คะแนนจากเหรียญ
    public void AddCoinScore(int amount)
    {
        totalScore += amount;
        UpdateUI();
    }

    // คะแนนจากศัตรู
    public void AddEnemyScore(int amount)
    {
        totalScore += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if(scoreText != null)
            scoreText.text = totalScore.ToString();
    }
}
