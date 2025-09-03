using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // ✅ ต้องใช้ TextMeshPro

public class PortalArea : MonoBehaviour
{
    public string creditSceneName = "CreditScene"; // ชื่อ Scene เครดิต
    private bool isPlayerInside = false; // เช็คว่าผู้เล่นอยู่ใน Portal หรือยัง

    [Header("UI Settings")]
    public TextMeshProUGUI hintText; // UI ข้อความแจ้งเตือน

    void Start()
    {
        // ซ่อนข้อความตั้งแต่เริ่มเกม
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    void Update()
    {
        // ถ้าผู้เล่นอยู่ในพื้นที่ และกดปุ่ม T
        if (isPlayerInside && Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(creditSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;

            // โชว์ข้อความ "กด T เพื่อเข้าสู่ Credit Game "
            if (hintText != null)
            {
                hintText.text ="Press T to return to MainMenu";

                hintText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;

            // ซ่อนข้อความเมื่อออกจากพื้นที่
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }
}
