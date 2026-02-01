using Firebase.Auth;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Main.Mainmenu
{
    public class LobbyController : Controller
    {
        [SerializeField] private Animator dailyRewardAnimator;
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

        public void CloseDailyRewad(float duration)
        {
            StartCoroutine(CloseDailyRewadIenumerator(duration));
        }

        public IEnumerator CloseDailyRewadIenumerator(float duration)
        {
            dailyRewardAnimator.SetTrigger("Close");
            yield return new WaitForSeconds(duration);
            Disactivate("daily reward");
        }
    }
}