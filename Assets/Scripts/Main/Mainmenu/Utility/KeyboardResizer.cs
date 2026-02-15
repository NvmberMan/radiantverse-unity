using UnityEngine;

public class KeyboardResizer : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 initialPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Cek jika keyboard aktif
        if (TouchScreenKeyboard.visible)
        {
            // Ambil tinggi keyboard (0-1) lalu konversi ke unit UI
            float keyboardHeight = GetKeyboardHeight();

            // Geser posisi Panel ke atas (bisa ditambah offset jika kurang tinggi)
            Vector2 newPos = new Vector2(initialPosition.x, keyboardHeight);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, newPos, Time.deltaTime * 10f);
        }
        else
        {
            // Kembalikan ke posisi awal jika keyboard tertutup
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, initialPosition, Time.deltaTime * 10f);
        }
    }

    private float GetKeyboardHeight()
    {
#if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject view = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
            Rect rect = new Rect();
            view.Call("getWindowVisibleDisplayFrame", rect);
            // Menghitung selisih tinggi layar asli dengan layar yang terlihat
            return Screen.height - rect.height;
        }
#else
        return TouchScreenKeyboard.area.height;
#endif
    }
}