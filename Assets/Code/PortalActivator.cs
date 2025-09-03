using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalActivator : MonoBehaviour
{
    public static PortalActivator instance;

    public GameObject portal;    // พอร์ทัลที่ตั้งไว้ใน Scene
    public string sceneToLoad = "CreditScene"; // ตั้งชื่อ Scene เครดิต
    public KeyCode interactKey = KeyCode.T;    // ปุ่มที่ใช้กดเข้าไป

    private bool canEnter = false;

    void Awake()
    {
        instance = this;
        if (portal != null) portal.SetActive(false);
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
            canEnter = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            canEnter = false;
    }
}
