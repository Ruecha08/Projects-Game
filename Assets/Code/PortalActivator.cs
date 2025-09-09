using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalActivator : MonoBehaviour
{
    public static PortalActivator instance;

    public GameObject portal;            // พอร์ทัลที่ตั้งไว้ใน Scene
    public string sceneToLoad = "CreditScene"; 
    public KeyCode interactKey = KeyCode.T;    

    [Header("UI")]
    public GameObject portalMessage;     // ✅ UI ข้อความ "กด T เพื่อเข้า"

    private bool canEnter = false;

    void Awake()
    {
        instance = this;
        if (portal != null) portal.SetActive(false);

        if (portalMessage != null) 
            portalMessage.SetActive(false); // ปิดข้อความตอนเริ่ม
    }

    public void ActivatePortal()
    {
        if (portal != null) portal.SetActive(true);
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(interactKey))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
            if (portalMessage != null)
                portalMessage.SetActive(true); // ✅ โชว์ข้อความเมื่อผู้เล่นเข้า
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
            if (portalMessage != null)
                portalMessage.SetActive(false); // ✅ ซ่อนข้อความเมื่อผู้เล่นออก
        }
    }
}
