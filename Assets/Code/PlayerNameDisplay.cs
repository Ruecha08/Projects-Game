using UnityEngine;
using TMPro;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    private void Start()
    {
        // ดึงชื่อที่บันทึกจาก PlayerPrefs
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        nameText.text = playerName;
    }

    private void LateUpdate()
    {
        // บังคับชื่อไม่พลิกตาม Player
        Vector3 scale = nameText.transform.localScale;
        scale.x = 1; // ล็อกไม่ให้กลับด้าน
        nameText.transform.localScale = scale;
    }
}
