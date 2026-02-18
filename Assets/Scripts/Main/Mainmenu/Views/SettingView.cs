using UnityEngine;
using UnityEngine.UI;
using static Main.Mainmenu.GameplayController;

namespace Main.Mainmenu
{
    public class SettingView : View
    {
        [Header("Slider UI Elements")]
        public Slider musicSlider;
        public Slider sfxSlider;
        public Slider cameraSensitivySlider;
        public Slider zoomSpeedSlider;
        public Toggle fixedCameraToggle;
        public Button exitButton;

        [HideInInspector] public string SENS_KEY = "CAMERA_SENSITIVITY";
        [HideInInspector] public string JOYSTICK_KEY = "JOYSTICK_MODE";
        [HideInInspector] public string ZOOM_PC_KEY = "ZOOM_SPEED_PC";
        [HideInInspector] public string ZOOM_MOBILE_KEY = "ZOOM_SPEED_MOBILE";

        private void Start()
        {
            musicSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.MUSIC_KEY, val);
            });

            sfxSlider.onValueChanged.AddListener((val) => {
                AudioManager.Instance.SetVolume(AudioManager.SFX_KEY, val);
            });

            cameraSensitivySlider.onValueChanged.AddListener(OnSensitivityChanged);

            fixedCameraToggle.onValueChanged.AddListener(OnJoystickToggleChanged);

            zoomSpeedSlider.onValueChanged.AddListener(OnZoomSpeedChanged);

            exitButton.onClick.AddListener(() =>
            {
                MenuManager.instance.GetController<UniversalController>().GetView<ExitPopupView>().Show();
            });
        }

        private void OnEnable()
        {
            fixedCameraToggle.gameObject.GetComponent<Animator>().SetBool("On", fixedCameraToggle.isOn);
        }

        void OnSensitivityChanged(float value)
        {
            PlayerPrefs.SetFloat(SENS_KEY, value);
            PlayerPrefs.Save();
        }

        public void OnJoystickToggleChanged(bool isOn)
        {
            JoystickMode joystickMode = isOn ? JoystickMode.Fixed : JoystickMode.Dynamic;

            fixedCameraToggle.GetComponent<Animator>().SetTrigger("Switch");
            fixedCameraToggle.GetComponent<Animator>().SetBool("On", isOn);

            PlayerPrefs.SetInt(JOYSTICK_KEY, (int)joystickMode);
            PlayerPrefs.Save();
        }

        void OnZoomSpeedChanged(float value)
        {
            PlayerPrefs.SetFloat(ZOOM_PC_KEY, value);
            PlayerPrefs.SetFloat(ZOOM_MOBILE_KEY, value);
            PlayerPrefs.Save();
        }
    }
}