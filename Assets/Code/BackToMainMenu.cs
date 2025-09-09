using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    // ฟังก์ชันสำหรับปุ่มกด
    public void GoToMainMenu()
    {
        // เปลี่ยนชื่อ "MainMenuScene" เป็นชื่อ Scene ของเมนูหลักจริง
        SceneManager.LoadScene("MainMenu");
    }
}
