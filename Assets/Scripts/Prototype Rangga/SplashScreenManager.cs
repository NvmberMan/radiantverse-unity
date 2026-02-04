using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    public void GoToMainmenu()
    {
        SceneManager.LoadScene("Mainmenu");
    }
}
