using UnityEngine;
using UnityEngine.SceneManagement;
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
    }

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(creditSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            if (hintText != null)
            {
                hintText.text = "Press T to return to Main Menu";
                hintText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }
}
