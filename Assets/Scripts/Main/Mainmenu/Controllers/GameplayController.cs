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
        [Range(0.3f, 0.7f)]
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

        [Header("UI")]
        public Slider sensitivitySlider;

        private Vector2 lastInputPos;
        private CharacterMovement characterMovement;
        private PlayerInput playerInput;
        private PlayerInputJoystick playerInputJoystick;

        private const string SENS_KEY = "CAMERA_SENSITIVITY";

        // ================= INIT =================
        private void Awake()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            cameraMode = CameraControlMode.PC;
#else
            cameraMode = CameraControlMode.Mobile;
#endif
        }

        private void Start()
        {
            characterMovement = GameManager.Instance.playerTransform
                .GetComponent<CharacterMovement>();

            playerInput = GameManager.Instance.playerTransform
                .GetComponent<PlayerInput>();

            playerInputJoystick = GameManager.Instance.playerTransform
                .GetComponent<PlayerInputJoystick>();

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

                    if(!GameManager.Instance.isPaused)
                        HandleMobileCamera();

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
                        HandlePCCamera();
                    break;
            }
        }

        void HandleMobileCamera()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.mousePosition.x < Screen.width * cameraAreaStart)
                    return;

                lastInputPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition.x < Screen.width * cameraAreaStart)
                    return;

                Vector2 delta = (Vector2)Input.mousePosition - lastInputPos;
                lastInputPos = Input.mousePosition;

                RotateCameraMobile(NormalizeDelta(delta));
            }
#else
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.position.x < Screen.width * cameraAreaStart)
                    return;

                if (touch.phase == TouchPhase.Began)
                {
                    lastInputPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.position - lastInputPos;
                    lastInputPos = touch.position;

                    RotateCameraMobile(NormalizeDelta(delta));
                }
            }
#endif
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
