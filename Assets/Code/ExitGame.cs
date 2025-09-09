using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // ฟังก์ชันสำหรับปุ่มกด
    public void QuitGame()
    {
        #if UNITY_EDITOR
        // ใช้ปิดเกมใน Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // ใช้ออกเกมจริง
        Application.Quit();
        #endif
    }
}
