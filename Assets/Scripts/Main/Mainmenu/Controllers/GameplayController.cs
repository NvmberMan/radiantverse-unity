using Main.Gameplay;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class GameplayController : Controller
    {
        public enum CameraControlMode
        {
            Mobile,
            PC
        }

        public enum JoystickMode
        {
            Fixed,
            Dynamic
        }


        [Header("Camera Mode")]
        public CameraControlMode cameraMode = CameraControlMode.Mobile;

        [Header("Sensitivity (Shared)")]
        [Tooltip("Base sensitivity (slider controlled)")]
        public float baseSensitivity = 1f;

        [Header("PC Settings")]
        [Tooltip("Rotation speed multiplier for PC")]
        public float pcRotationSpeed = 300f;

        [Header("Mobile Settings")]
        [Tooltip("Rotation multiplier for Mobile")]
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

        [Header("UI")]
        public Slider sensitivitySlider;
        public Toggle joystickToggle;
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
            SetupJoystick();

            if (joystickToggle != null)
            {
                joystickToggle.isOn = (joystickMode == JoystickMode.Fixed);
            }
        }

        private void Start()
        {
            characterMovement = GameManager.Instance.playerTransform
                .GetComponent<CharacterMovement>();

            playerInput = GameManager.Instance.playerTransform
                .GetComponent<PlayerInput>();

            playerInputJoystick = GameManager.Instance.playerTransform
                .GetComponent<PlayerInputJoystick>();

            if (GameManager.Instance.orbitalFollow != null)
            {
                targetZoom = GameManager.Instance.orbitalFollow.RadialAxis.Value;
                currentZoom = targetZoom;
            }

            LoadSensitivity();
            SetupSlider();

            LoadZoomSpeed();
            SetupZoomSlider();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.isPaused)
            {
                if (!GameManager.Instance.isPaused)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
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
                    }
                    else
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }

                    if (!GameManager.Instance.isPaused && !GameManager.Instance.isCinematic)
                    {
                        HandlePCCamera();
                        HandleZoom();
                    }
                    break;
            }

            if (!GameManager.Instance.isPaused)
            {
                currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSmoothing);
                GameManager.Instance.orbitalFollow.RadialAxis.Value = currentZoom;
            }
        }



        void HandleZoom()
        {
            if (cameraMode == CameraControlMode.PC)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0)
                {
                    ApplyZoom(-scroll * zoomSpeedPC);
                }
            }
            else if (cameraMode == CameraControlMode.Mobile && Input.touchCount >= 2)
            {
                Touch? t0 = null;
                Touch? t1 = null;

                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);

                    if (IsTouchOnJoystick(t.position))
                        continue;

                    if (t0 == null) t0 = t;
                    else if (t1 == null) t1 = t;
                }

                if (t0 == null || t1 == null)
                    return;

                Touch touch0 = t0.Value;
                Touch touch1 = t1.Value;

                Vector2 prev0 = touch0.position - touch0.deltaPosition;
                Vector2 prev1 = touch1.position - touch1.deltaPosition;

                float prevMag = (prev0 - prev1).magnitude;
                float currMag = (touch0.position - touch1.position).magnitude;

                float diff = currMag - prevMag;
                ApplyZoom(-diff * zoomSpeedMobile);
            }


        }

        void ApplyZoom(float increment)
        {
            targetZoom += increment;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        void HandleMobileCamera()
        {
            Touch? cameraTouch = null;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);

                if (IsTouchOnJoystick(t.position))
                    continue;

                cameraTouch = t;
                break;
            }

            if (cameraTouch == null)
            {
                isCameraTouchActive = false;
                return;
            }

            Touch touch = cameraTouch.Value;

            if (touch.phase == TouchPhase.Began)
            {
                isCameraTouchActive = true;
                lastInputPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isCameraTouchActive)
            {
                Vector2 delta = touch.position - lastInputPos;
                lastInputPos = touch.position;

                RotateCameraMobile(NormalizeDelta(delta));
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isCameraTouchActive = false;
            }
        }
        void HandlePCCamera()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            RotateCameraPC(new Vector2(mouseX, mouseY));
        }

        void RotateCameraPC(Vector2 delta)
        {
            GameManager.Instance.orbitalFollow.HorizontalAxis.Value +=
                delta.x * pcRotationSpeed * baseSensitivity * Time.deltaTime;

            GameManager.Instance.orbitalFollow.VerticalAxis.Value -=
                delta.y * pcRotationSpeed * baseSensitivity * Time.deltaTime;

            ClampVertical();
        }

        void RotateCameraMobile(Vector2 normalizedDelta)
        {
            GameManager.Instance.orbitalFollow.HorizontalAxis.Value +=
                normalizedDelta.x * mobileRotationMultiplier * baseSensitivity;

            GameManager.Instance.orbitalFollow.VerticalAxis.Value -=
                normalizedDelta.y * mobileRotationMultiplier * baseSensitivity;

            ClampVertical();
        }

        void ClampVertical()
        {
            GameManager.Instance.orbitalFollow.VerticalAxis.Value = Mathf.Clamp(
                GameManager.Instance.orbitalFollow.VerticalAxis.Value,
                GameManager.Instance.orbitalFollow.VerticalAxis.Range.x,
                GameManager.Instance.orbitalFollow.VerticalAxis.Range.y
            );
        }

        Vector2 NormalizeDelta(Vector2 delta)
        {
            return new Vector2(
                delta.x / Screen.width,
                delta.y / Screen.height
            );
        }


        #region Setup
        void SetupSlider()
        {
            if (sensitivitySlider == null)
                return;

            sensitivitySlider.minValue = 0.2f;
            sensitivitySlider.maxValue = 2.5f;
            sensitivitySlider.value = baseSensitivity;

            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        void SetupJoystick()
        {
            if (fixedJoystick != null)
                fixedJoystick.gameObject.SetActive(false);

            if (dynamicJoystick != null)
                dynamicJoystick.gameObject.SetActive(false);

            switch (joystickMode)
            {
                case JoystickMode.Fixed:
                    activeJoystick = fixedJoystick;
                    activeJoystickArea = fixedJoystickArea;
                    if (fixedJoystick != null)
                        fixedJoystick.gameObject.SetActive(true);
                    break;

                case JoystickMode.Dynamic:
                    activeJoystick = dynamicJoystick;
                    activeJoystickArea = dynamicJoystickArea;
                    if (dynamicJoystick != null)
                        dynamicJoystick.gameObject.SetActive(true);
                    break;
            }

            OnChangeJoystickSystem?.Invoke();
        }


        void SetupZoomSlider()
        {
            if (zoomSpeedSlider == null)
                return;

            zoomSpeedSlider.minValue = 0.0001f;
            zoomSpeedSlider.maxValue = 0.01f;

            zoomSpeedSlider.value =
                cameraMode == CameraControlMode.PC
                    ? zoomSpeedPC
                    : zoomSpeedMobile;

            zoomSpeedSlider.onValueChanged.AddListener(OnZoomSpeedChanged);
        }
        #endregion

        #region Save Data
        void OnSensitivityChanged(float value)
        {
            baseSensitivity = value;
            PlayerPrefs.SetFloat(SENS_KEY, baseSensitivity);
            PlayerPrefs.Save();
        }

        public void OnJoystickToggleChanged(bool isOn)
        {
            joystickMode = isOn ? JoystickMode.Fixed : JoystickMode.Dynamic;

            SetupJoystick();
            SaveJoystickMode();
        }

        void OnZoomSpeedChanged(float value)
        {
            if (cameraMode == CameraControlMode.PC)
                zoomSpeedPC = value;
            else
                zoomSpeedMobile = value;

            SaveZoomSpeed();
        }


        void SaveJoystickMode()
        {
            PlayerPrefs.SetInt(JOYSTICK_KEY, (int)joystickMode);
            PlayerPrefs.Save();
        }

        void SaveZoomSpeed()
        {
            PlayerPrefs.SetFloat(ZOOM_PC_KEY, zoomSpeedPC);
            PlayerPrefs.SetFloat(ZOOM_MOBILE_KEY, zoomSpeedMobile);
            PlayerPrefs.Save();
        }

        #endregion

        #region Load Data
        void LoadSensitivity()
        {
            baseSensitivity = PlayerPrefs.GetFloat(SENS_KEY, 1f);
        }

        void LoadJoystickMode()
        {
            int savedMode = PlayerPrefs.GetInt(JOYSTICK_KEY, 0); // default Fixed
            joystickMode = (JoystickMode)savedMode;
        }

        void LoadZoomSpeed()
        {
            zoomSpeedPC = PlayerPrefs.GetFloat(ZOOM_PC_KEY, zoomSpeedPC);
            zoomSpeedMobile = PlayerPrefs.GetFloat(ZOOM_MOBILE_KEY, zoomSpeedMobile);
        }
        #endregion



        public void Jump()
        {
            characterMovement.Jump();
        }

        public void Pause()
        {
            MenuManager.instance.GetController<PauseController>()
                .Activate("base");

            GameManager.Instance.isPaused = true;
        }

        public void Resume()
        {
            MenuManager.instance.GetController<PauseController>()
                .Disactivate("base");

            GameManager.Instance.isPaused = false;
        }

        public void SetCameraModePC()
        {
            cameraMode = CameraControlMode.PC;
        }

        public void SetCameraModeMobile()
        {
            cameraMode = CameraControlMode.Mobile;
        }

        bool IsTouchOnJoystick(Vector2 screenPos)
        {
            if (activeJoystickArea == null)
                return false;

            return RectTransformUtility.RectangleContainsScreenPoint(
                activeJoystickArea,
                screenPos,
                null
            );
        }


        void OnDrawGizmos()
        {
            if (activeJoystickArea == null) return;

            Gizmos.color = Color.green;

            Vector3[] corners = new Vector3[4];
            activeJoystickArea.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
        }

    }
}
