using UnityEngine;
using UnityEngine.SceneManagement; // ต้องเพิ่มบรรทัดนี้

public class SceneChanger : MonoBehaviour
{
    // ฟังก์ชันสำหรับเปลี่ยน Scene ไปยัง Scene ที่มีชื่อตามที่กำหนด
    public void ChangeSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ฟังก์ชันสำหรับเปลี่ยน Scene ไปยัง Scene ที่มี Index ตามที่กำหนด
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}