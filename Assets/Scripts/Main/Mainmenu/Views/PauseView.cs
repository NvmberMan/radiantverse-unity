using System;
using UnityEngine;
using UnityEngine.UI;
using static Main.Mainmenu.GameplayController;

namespace Main.Mainmenu
{
    public class PauseView : View
    {
        //[Header("Mobile Camera Exclusion")]
        //public RectTransform fixedJoystickArea;
        //public RectTransform dynamicJoystickArea;

        //[Header("Joystick Settings")]
        //public JoystickMode joystickMode = JoystickMode.Fixed;
        //public Joystick fixedJoystick;
        //public Joystick dynamicJoystick;
        public Toggle joystickToggle;

        //[HideInInspector] public Joystick activeJoystick;
        //private RectTransform activeJoystickArea;
        //public Action OnChangeJoystickSystem;


        //private Toggle joystickToggle;
        private GameplayController gameplayController;

        private const string JOYSTICK_KEY = "JOYSTICK_MODE";

        private void Start()
        {
        }
        private void OnEnable()
        {
            gameplayController = MenuManager.instance.GetController<GameplayController>();
            InitJoystickToggleUI();
        }

        private void InitJoystickToggleUI()
        {
            if (joystickToggle != null)
            {
                // Set nilai tanpa memicu trigger animasi "Switch" di awal
                joystickToggle.isOn = (gameplayController.joystickMode == JoystickMode.Fixed);

                Animator anim = joystickToggle.GetComponent<Animator>();
                if (anim != null)
                {
                    // Langsung set parameter "On" agar posisi handle benar sejak awal
                    anim.SetBool("On", joystickToggle.isOn);
                    Debug.Log($"aksldjfksjd: {joystickToggle.isOn}");
                }

                // Tambahkan listener untuk mendeteksi klik user
                joystickToggle.onValueChanged.RemoveAllListeners();
                joystickToggle.onValueChanged.AddListener(gameplayController.OnJoystickToggleChanged);
            }
        }

    }
}