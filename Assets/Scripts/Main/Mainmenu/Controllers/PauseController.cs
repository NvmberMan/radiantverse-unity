using Main.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class PauseController : Controller
    {
        [Header("Slider UI Elements")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            LoadSliderValues();

            musicSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.MUSIC_KEY, val);
            });

            sfxSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.SFX_KEY, val);
            });
        }

        private void LoadSliderValues()
        {
            musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 0.75f);
            sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 0.75f);
        }
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