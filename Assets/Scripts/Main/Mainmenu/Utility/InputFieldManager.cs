using UnityEngine;

public class InputFieldManager : MonoBehaviour
{
    private RectTransform containerRect;
    private RectTransform activeInputField;
    private Vector2 originalPosition;
    private float currentVelocity;

    public bool debugInEditor = false;
    void Start()
    {
        containerRect = GetComponent<RectTransform>();
        originalPosition = containerRect.anchoredPosition;
    }

    // Dipanggil oleh script Observer saat input field diklik
    public void SetActiveInputField(RectTransform field)
    {
        activeInputField = field;
    }

    void LateUpdate()
    {
        // Modifikasi kondisi IF agar bisa ditest di Editor
        bool isKeyboardOpen = TouchScreenKeyboard.visible;

        #if UNITY_EDITOR
                if (debugInEditor) isKeyboardOpen = true;
        #endif

        if (isKeyboardOpen && activeInputField != null)
        {
            ShiftUI();
        }
        else
        {
            ResetUI();
        }
    }

    void ShiftUI()
    {
        // 1. Dapatkan posisi dunia dari input field yang aktif
        Vector3[] corners = new Vector3[4];
        activeInputField.GetWorldCorners(corners);
        float fieldBottomY = corners[0].y; // Posisi bawah input field dalam pixel

        Canvas canvas = GetComponentInParent<Canvas>();
        float scaleFactor = canvas.scaleFactor;
        // 2. Dapatkan tinggi keyboard
        // TouchScreenKeyboard.area.height memberikan tinggi dalam pixel layar
        float keyboardHeight = TouchScreenKeyboard.area.height / scaleFactor;
        float fieldBottomYCanvas = fieldBottomY / scaleFactor;

        // 3. Cek apakah keyboard menutupi input field
        // Kita beri toleransi/margin tambahan 50 unit agar tidak mepet
        float margin = 30f;

        if (fieldBottomY < (keyboardHeight + margin))
        {
            float targetY = containerRect.anchoredPosition.y + (keyboardHeight - fieldBottomY) + margin;
            Vector2 targetPos = new Vector2(originalPosition.x, targetY);

            // Gerakan smooth agar tidak kaget
            //containerRect.anchoredPosition = Vector2.Lerp(containerRect.anchoredPosition, targetPos, Time.deltaTime * 10f);
            containerRect.anchoredPosition = targetPos;
        }
    }

    void ResetUI()
    {
        // Kembalikan ke posisi awal saat keyboard tertutup
        if (containerRect.anchoredPosition != originalPosition)
        {
            containerRect.anchoredPosition = Vector2.Lerp(containerRect.anchoredPosition, originalPosition, Time.deltaTime * 10f);
        }
        activeInputField = null;
    }

}