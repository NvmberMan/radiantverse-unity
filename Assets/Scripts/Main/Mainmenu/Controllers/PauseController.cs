using Main.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Mainmenu
{
    public class PauseController : Controller
    {
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void Resume()
        {
            Disactivate("base");
            GameManager.Instance.isPaused = false;
        }
    }
}