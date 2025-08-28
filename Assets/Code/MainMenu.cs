using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // เพิ่ม namespace นี้เข้ามา

public class MainMenu : MonoBehaviour
{
    // เปลี่ยนประเภทตัวแปรเป็น TMP_InputField
    public TMP_InputField nameInputField;

    public void StartGame()
    {
        string playerName = nameInputField.text;
        
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Please enter a name!");
            return;
        }

        PlayerInfo.playerName = playerName;

        // เปลี่ยน "GameScene" เป็นชื่อของฉากเกมของคุณ
        // SceneManager.LoadScene("GameScene"); 
    }
}