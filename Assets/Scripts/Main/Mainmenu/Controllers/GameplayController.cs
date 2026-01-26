using Main.Gameplay;
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

        [Header("Camera Mode")]
        public CameraControlMode cameraMode = CameraControlMode.Mobile;

        [Header("Mobile Settings")]
        [Range(0, 1f)]
        public float cameraAreaStart = 0.5f;

        [Header("Sensitivity (Shared)")]
        [Tooltip("Base sensitivity (slider controlled)")]
        public float baseSensitivity = 1f;

        [Header("PC Settings")]
        [Tooltip("Rotation speed multiplier for PC")]
        public float pcRotationSpeed = 300f;

        [Header("Mobile Settings")]
        [Tooltip("Rotation multiplier for Mobile")]
        public float mobileRotationMultiplier = 180f;
        public Joystick joystick;

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

        private Vector2 lastInputPos;
        private bool isCameraTouchActive = false;
        private CharacterMovement characterMovement;
        private PlayerInput playerInput;
        private PlayerInputJoystick playerInputJoystick;

        private const string SENS_KEY = "CAMERA_SENSITIVITY";

        // ================= INIT =================
        private void Awake()
        {
//#if UNITY_STANDALONE || UNITY_EDITOR
//            cameraMode = CameraControlMode.PC;
//#else
//            cameraMode = CameraControlMode.Mobile;
//#endif
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

                    if (!GameManager.Instance.isPaused)
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
                float boundary = Screen.width * cameraAreaStart;

                Touch? t0 = null;
                Touch? t1 = null;

                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);

                    if (t.position.x >= boundary)
                    {
                        if (t0 == null) t0 = t;
                        else if (t1 == null) t1 = t;
                    }
                }

                if (t0 == null || t1 == null)
                    return;

                Touch touch0 = t0.Value;
                Touch touch1 = t1.Value;

                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                ApplyZoom(-difference * zoomSpeedMobile);
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
            float boundary = Screen.width * cameraAreaStart;

            // Cari touch di area kamera
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (t.position.x >= boundary)
                {
                    cameraTouch = t;
                    break;
                }
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

        void SetupSlider()
        {
            if (sensitivitySlider == null)
                return;

            sensitivitySlider.minValue = 0.2f;
            sensitivitySlider.maxValue = 2.5f;
            sensitivitySlider.value = baseSensitivity;

            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        void OnSensitivityChanged(float value)
        {
            baseSensitivity = value;
            PlayerPrefs.SetFloat(SENS_KEY, baseSensitivity);
            PlayerPrefs.Save();
        }

        void LoadSensitivity()
        {
            baseSensitivity = PlayerPrefs.GetFloat(SENS_KEY, 1f);
        }

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
    }
}
