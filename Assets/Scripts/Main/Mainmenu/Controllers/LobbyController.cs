using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Main.Mainmenu
{
    public class LobbyController : Controller
    {
        [Header("Slider UI Elements")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider voiceSlider;
        [SerializeField] private Slider ambienceSlider;

        private void Start()
        {
            LoadSliderValues();

            musicSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.MUSIC_KEY, val);
            });

            sfxSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.SFX_KEY, val);
            });

            voiceSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.VOICE_KEY, val);
            });

            ambienceSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.AMBIENCE_KEY, val);
            });
        }

        private void LoadSliderValues()
        {
            musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 0.75f);
            sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 0.75f);
            voiceSlider.value = PlayerPrefs.GetFloat(AudioManager.VOICE_KEY, 0.75f);
            ambienceSlider.value = PlayerPrefs.GetFloat(AudioManager.AMBIENCE_KEY, 0.75f);
        }
    }
}