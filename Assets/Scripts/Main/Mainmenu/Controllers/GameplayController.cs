using Main.Gameplay;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class GameplayController : Controller
    {
        public enum CameraControlMode { Mobile, PC }
        public enum JoystickMode { Fixed, Dynamic }

        [Header("Camera Mode")]
        public CameraControlMode cameraMode = CameraControlMode.Mobile;

        [Header("Sensitivity (Shared)")]
        public float baseSensitivity = 1f;

        [Header("PC Settings")]
        public float pcRotationSpeed = 300f;

        [Header("Mobile Settings")]
        public float mobileRotationMultiplier = 180f;

        [Header("Joystick Settings")]
        public JoystickMode joystickMode = JoystickMode.Fixed;
        public Joystick fixedJoystick;
        public Joystick dynamicJoystick;

        [Header("Mobile Camera Exclusion")]
        public RectTransform fixedJoystickArea;
        public RectTransform dynamicJoystickArea;

        [Header("Zoom Settings")]
        public float zoomSpeedPC = 5f;
        public float zoomSpeedMobile = 0.01f;
        public float minZoom = 2f;
        public float maxZoom = 15f;
        public float zoomSmoothing = 10f;
        private float targetZoom;
        private float currentZoom;

        [Header("UI Elements")]
        public Slider sensitivitySlider;
        public Toggle joystickToggle; // Toggle ini akan punya animasi
        public Slider zoomSpeedSlider;

        private Vector2 lastInputPos;
        private bool isCameraTouchActive = false;
        private CharacterMovement characterMovement;
        private PlayerInput playerInput;
        private PlayerInputJoystick playerInputJoystick;

        [HideInInspector] public Joystick activeJoystick;
        private RectTransform activeJoystickArea;
        public Action OnChangeJoystickSystem;

        private const string SENS_KEY = "CAMERA_SENSITIVITY";
        private const string JOYSTICK_KEY = "JOYSTICK_MODE";
        private const string ZOOM_PC_KEY = "ZOOM_SPEED_PC";
        private const string ZOOM_MOBILE_KEY = "ZOOM_SPEED_MOBILE";

        private void Awake()
        {
            LoadJoystickMode();
        }

        private void Start()
        {
            characterMovement = GameManager.Instance.playerTransform.GetComponent<CharacterMovement>();
            playerInput = GameManager.Instance.playerTransform.GetComponent<PlayerInput>();
            playerInputJoystick = GameManager.Instance.playerTransform.GetComponent<PlayerInputJoystick>();

            if (GameManager.Instance.orbitalFollow != null)
            {
                targetZoom = GameManager.Instance.orbitalFollow.RadialAxis.Value;
                currentZoom = targetZoom;
            }

            // Inisialisasi Data & UI
            LoadSensitivity();
            SetupSlider();
            LoadZoomSpeed();
            SetupZoomSlider();

            // Inisialisasi Joystick & Tampilan Toggle
            SetupJoystick();
            //InitJoystickToggleUI();
        }

        //private void InitJoystickToggleUI()
        //{
        //    if (joystickToggle != null)
        //    {
        //        // Set nilai tanpa memicu trigger animasi "Switch" di awal
        //        joystickToggle.isOn = (joystickMode == JoystickMode.Fixed);

        //        Animator anim = joystickToggle.GetComponent<Animator>();
        //        if (anim != null)
        //        {
        //            // Langsung set parameter "On" agar posisi handle benar sejak awal
        //            anim.SetBool("On", joystickToggle.isOn);
        //            Debug.Log($"aksldjfksjd: {joystickToggle.isOn}");
        //        }

        //        // Tambahkan listener untuk mendeteksi klik user
        //        joystickToggle.onValueChanged.RemoveAllListeners();
        //        joystickToggle.onValueChanged.AddListener(OnJoystickToggleChanged);
        //    }
        //}

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!GameManager.Instance.isPaused) Pause();
                else Resume();
            }

            switch (cameraMode)
            {
                case CameraControlMode.Mobile:
                    playerInput.enabled = false;
                    playerInputJoystick.enabled = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    if (!GameManager.Instance.isPaused)
                    {
                        HandleMobileCamera();
                        HandleZoom();
                    }
                    break;

                case CameraControlMode.PC:
                    playerInput.enabled = true;
                    playerInputJoystick.enabled = false;

                    if (!GameManager.Instance.isPaused)
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        if (!GameManager.Instance.isCinematic)
                        {
                            HandlePCCamera();
                            HandleZoom();
                        }
                    }
                    else
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    break;
            }

            if (!GameManager.Instance.isPaused && GameManager.Instance.orbitalFollow != null)
            {
                currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSmoothing);
                GameManager.Instance.orbitalFollow.RadialAxis.Value = currentZoom;
            }
        }

        #region Camera & Zoom Logic
        void HandleZoom()
        {
            if (cameraMode == CameraControlMode.PC)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0) ApplyZoom(-scroll * zoomSpeedPC);
            }
            else if (cameraMode == CameraControlMode.Mobile && Input.touchCount >= 2)
            {
                Touch? t0 = null; Touch? t1 = null;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    if (IsTouchOnJoystick(t.position)) continue;
                    if (t0 == null) t0 = t; else if (t1 == null) t1 = t;
                }

                if (t0 != null && t1 != null)
                {
                    Vector2 prev0 = t0.Value.position - t0.Value.deltaPosition;
                    Vector2 prev1 = t1.Value.position - t1.Value.deltaPosition;
                    float prevMag = (prev0 - prev1).magnitude;
                    float currMag = (t0.Value.position - t1.Value.position).magnitude;
                    ApplyZoom(-(currMag - prevMag) * zoomSpeedMobile);
                }
            }
        }

        void ApplyZoom(float increment)
        {
            targetZoom = Mathf.Clamp(targetZoom + increment, minZoom, maxZoom);
        }

        void HandleMobileCamera()
        {
            Touch? cameraTouch = null;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (IsTouchOnJoystick(t.position)) continue;
                cameraTouch = t; break;
            }

            if (cameraTouch == null) { isCameraTouchActive = false; return; }

            Touch touch = cameraTouch.Value;
            if (touch.phase == TouchPhase.Began) { isCameraTouchActive = true; lastInputPos = touch.position; }
            else if (touch.phase == TouchPhase.Moved && isCameraTouchActive)
            {
                Vector2 delta = touch.position - lastInputPos;
                lastInputPos = touch.position;
                RotateCameraMobile(new Vector2(delta.x / Screen.width, delta.y / Screen.height));
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) isCameraTouchActive = false;
        }

        void HandlePCCamera()
        {
            RotateCameraPC(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
        }

        void RotateCameraPC(Vector2 delta)
        {
            GameManager.Instance.orbitalFollow.HorizontalAxis.Value += delta.x * pcRotationSpeed * baseSensitivity * Time.deltaTime;
            GameManager.Instance.orbitalFollow.VerticalAxis.Value -= delta.y * pcRotationSpeed * baseSensitivity * Time.deltaTime;
            ClampVertical();
        }

        void RotateCameraMobile(Vector2 normDelta)
        {
            GameManager.Instance.orbitalFollow.HorizontalAxis.Value += normDelta.x * mobileRotationMultiplier * baseSensitivity;
            GameManager.Instance.orbitalFollow.VerticalAxis.Value -= normDelta.y * mobileRotationMultiplier * baseSensitivity;
            ClampVertical();
        }

        void ClampVertical()
        {
            var axis = GameManager.Instance.orbitalFollow.VerticalAxis;
            axis.Value = Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);
        }
        #endregion

        #region Setup & Save/Load
        void SetupSlider()
        {
            if (sensitivitySlider == null) return;
            sensitivitySlider.minValue = 0.2f; sensitivitySlider.maxValue = 2.5f;
            sensitivitySlider.value = baseSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        void SetupJoystick()
        {
            if (fixedJoystick != null) fixedJoystick.gameObject.SetActive(false);
            if (dynamicJoystick != null) dynamicJoystick.gameObject.SetActive(false);

            if (joystickMode == JoystickMode.Fixed)
            {
                activeJoystick = fixedJoystick;
                activeJoystickArea = fixedJoystickArea;
                if (fixedJoystick != null) fixedJoystick.gameObject.SetActive(true);
            }
            else
            {
                activeJoystick = dynamicJoystick;
                activeJoystickArea = dynamicJoystickArea;
                if (dynamicJoystick != null) dynamicJoystick.gameObject.SetActive(true);
            }
            OnChangeJoystickSystem?.Invoke();
        }

        void SetupZoomSlider()
        {
            if (zoomSpeedSlider == null) return;
            zoomSpeedSlider.minValue = 0.0001f; zoomSpeedSlider.maxValue = 0.01f;
            zoomSpeedSlider.value = (cameraMode == CameraControlMode.PC) ? zoomSpeedPC : zoomSpeedMobile;
            zoomSpeedSlider.onValueChanged.AddListener(OnZoomSpeedChanged);
        }

        public void OnJoystickToggleChanged(bool isOn)
        {
            // Update Logika
            joystickMode = isOn ? JoystickMode.Fixed : JoystickMode.Dynamic;
            SetupJoystick();
            PlayerPrefs.SetInt(JOYSTICK_KEY, (int)joystickMode);
            PlayerPrefs.Save();

            // Update Animasi (Sama persis dengan SettingView)
            if (joystickToggle != null)
            {
                Animator anim = joystickToggle.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetTrigger("Switch");
                    anim.SetBool("On", isOn);
                }
            }
        }

        void OnSensitivityChanged(float value) { baseSensitivity = value; PlayerPrefs.SetFloat(SENS_KEY, value); PlayerPrefs.Save(); }
        void OnZoomSpeedChanged(float value) { if (cameraMode == CameraControlMode.PC) zoomSpeedPC = value; else zoomSpeedMobile = value; SaveZoomSpeed(); }
        void SaveZoomSpeed() { PlayerPrefs.SetFloat(ZOOM_PC_KEY, zoomSpeedPC); PlayerPrefs.SetFloat(ZOOM_MOBILE_KEY, zoomSpeedMobile); PlayerPrefs.Save(); }
        void LoadSensitivity() => baseSensitivity = PlayerPrefs.GetFloat(SENS_KEY, 1f);
        void LoadJoystickMode() => joystickMode = (JoystickMode)PlayerPrefs.GetInt(JOYSTICK_KEY, 0);
        void LoadZoomSpeed() { zoomSpeedPC = PlayerPrefs.GetFloat(ZOOM_PC_KEY, 5f); zoomSpeedMobile = PlayerPrefs.GetFloat(ZOOM_MOBILE_KEY, 0.01f); }
        #endregion

        #region Public Methods
        public void Jump() => characterMovement.Jump();
        public void Pause() { MenuManager.instance.GetController<PauseController>().Activate("base"); GameManager.Instance.isPaused = true; if (RemoteTestManager.Instance != null)  RemoteTestManager.Instance.LogPause(); }
        public void Resume() { MenuManager.instance.GetController<PauseController>().Disactivate("base"); GameManager.Instance.isPaused = false; }
        public void SetCameraModePC() => cameraMode = CameraControlMode.PC;
        public void SetCameraModeMobile() => cameraMode = CameraControlMode.Mobile;

        bool IsTouchOnJoystick(Vector2 screenPos)
        {
            if (activeJoystickArea == null) return false;
            return RectTransformUtility.RectangleContainsScreenPoint(activeJoystickArea, screenPos, null);
        }
        #endregion
    }
}