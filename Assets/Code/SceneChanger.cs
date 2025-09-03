using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // ฟังก์ชันสำหรับเปลี่ยนฉากโดยใช้ชื่อฉาก (Scene Name)
    public void ChangeSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ฟังก์ชันสำหรับเปลี่ยนฉากโดยใช้หมายเลขลำดับ (Scene Index)
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // ฟังก์ชันสำหรับโหลดฉากต่อไปใน Build Settings
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}