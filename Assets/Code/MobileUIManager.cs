using UnityEngine;

public class MobileUIManager : MonoBehaviour
{
    void Start()
    {
        // ถ้าไม่ใช่มือถือ → ปิดปุ่มมือถือ
        if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
        {
            gameObject.SetActive(false);
        }
    }
}
