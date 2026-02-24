using Firebase.Auth;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Main.Mainmenu.GameplayController;


namespace Main.Mainmenu
{
    public class LobbyController : Controller
    {
        [SerializeField] private Animator dailyRewardAnimator;

        private SettingView settingView;

        private void Start()
        {
            settingView = GetView<SettingView>();
            LoadVolumeValues();
            LoadSensitivity();
            LoadZoomSpeed();
            LoadJoystickMode();
        }

        #region Load Settings Data
        void LoadSensitivity()
        {
            settingView.cameraSensitivySlider.value = PlayerPrefs.GetFloat(settingView.SENS_KEY, 1f);
        }

        void LoadJoystickMode()
        {
            int savedMode = PlayerPrefs.GetInt(settingView.JOYSTICK_KEY, 0);
            JoystickMode joystickMode = (JoystickMode)savedMode;
            settingView.fixedCameraToggle.isOn = (joystickMode == JoystickMode.Fixed);
        }

        void LoadZoomSpeed()
        {
            settingView.zoomSpeedSlider.value = PlayerPrefs.GetFloat(settingView.ZOOM_MOBILE_KEY, 0.005f);
        }
        private void LoadVolumeValues()
        {
            settingView.musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 0.75f);
            settingView.sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 0.75f);
        }

        #endregion

        #region Reward System
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
        #endregion
    }
}