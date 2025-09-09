using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel; // ลาก Panel ที่สร้างไว้ตรงนี้

    // เปิด Panel
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    // ปิด Panel
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
