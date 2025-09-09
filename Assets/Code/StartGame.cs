using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นสำหรับโหลด Scene

public class StartGame : MonoBehaviour
{
    // ฟังก์ชันสำหรับปุ่มกด
    public void LoadGameScene()
    {
        // ใส่ชื่อ Scene ของเกมหลักที่ต้องการโหลด
        SceneManager.LoadScene("MainScene");
    }
}
