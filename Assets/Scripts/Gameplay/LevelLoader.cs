using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Fungsi ini bisa dipanggil lewat Inspector (Unity Event)
    public void LoadLevelByName(string sceneName)
    {
        Debug.Log("🚀 Memuat Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}