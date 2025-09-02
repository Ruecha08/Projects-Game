using UnityEngine;
using TMPro; // สำหรับ TextMeshPro
using UnityEngine.SceneManagement;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    // เรียกใช้ตอนกดปุ่ม "เริ่มเกม"
    public void SavePlayerName()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            PlayerPrefs.SetString("PlayerName", nameInputField.text); // บันทึกชื่อ
            PlayerPrefs.Save();

            // โหลดด่านถัดไป (เช่นชื่อว่า "Level1")
            SceneManager.LoadScene("Level1");
        }
    }
}
