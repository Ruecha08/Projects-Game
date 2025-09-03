using UnityEngine;
using UnityEngine.SceneManagement;
<<<<<<< HEAD
using TMPro;

public class PortalArea : MonoBehaviour
{
    public string creditSceneName = "CreditScene"; 
    public TextMeshProUGUI hintText; 

    private bool isPlayerInside = false;

    void Start()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false); // ซ่อนข้อความตอนเริ่ม
=======
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
>>>>>>> parent of a889788 (Revert "ก")
    }

    void Update()
    {
<<<<<<< HEAD
=======
        // ถ้าผู้เล่นอยู่ในพื้นที่ และกดปุ่ม T
>>>>>>> parent of a889788 (Revert "ก")
        if (isPlayerInside && Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(creditSceneName);
        }
    }

<<<<<<< HEAD
    void OnTriggerEnter2D(Collider2D other)
=======
    private void OnTriggerEnter2D(Collider2D other)
>>>>>>> parent of a889788 (Revert "ก")
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
<<<<<<< HEAD
            if (hintText != null)
            {
                hintText.text = "Press T to return to Main Menu";
=======

            // โชว์ข้อความ "กด T เพื่อเข้าสู่ Credit Game "
            if (hintText != null)
            {
                hintText.text ="Press T to return to MainMenu";

>>>>>>> parent of a889788 (Revert "ก")
                hintText.gameObject.SetActive(true);
            }
        }
    }

<<<<<<< HEAD
    void OnTriggerExit2D(Collider2D other)
=======
    private void OnTriggerExit2D(Collider2D other)
>>>>>>> parent of a889788 (Revert "ก")
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
<<<<<<< HEAD
=======

            // ซ่อนข้อความเมื่อออกจากพื้นที่
>>>>>>> parent of a889788 (Revert "ก")
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }
}
