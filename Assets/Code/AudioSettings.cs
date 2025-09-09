using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public Slider volumeSlider; // ลาก Slider เข้า Inspector

    void Start()
    {
        // โหลดค่าเสียงเก่าที่บันทึกไว้ (0–100)
        volumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 100f);
        SetVolume(volumeSlider.value);

        // ฟังการเปลี่ยนค่า Slider โดยใช้เมาส์
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // ฟังก์ชันปรับเสียงทันที
    public void SetVolume(float volume)
    {
        // แปลงจาก 0–100 เป็น 0–1
        AudioListener.volume = volume / 100f;

        // บันทึกค่าไว้ทันที
        PlayerPrefs.SetFloat("GameVolume", volume);
    }
}
