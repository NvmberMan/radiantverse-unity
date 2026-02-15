using UnityEngine;
using TMPro; // Pastikan menggunakan TextMeshPro
using UnityEngine.EventSystems;

public class InputFieldObserver : MonoBehaviour, ISelectHandler
{
    private InputFieldManager manager;
    private RectTransform rectTransform;

    void Start()
    {
        // Mencari script manager di parent/container
        manager = GetComponentInParent<InputFieldManager>();
        rectTransform = GetComponent<RectTransform>();

        // Agar tidak muncul overlay input bawaan Android yang menutupi layar
        var inputField = GetComponent<TMP_InputField>();
        if (inputField != null) inputField.shouldHideMobileInput = true;
    }

    // Fungsi ini otomatis jalan saat user klik kotak input
    public void OnSelect(BaseEventData eventData)
    {
        if (manager != null)
        {
            manager.SetActiveInputField(rectTransform);
        }
    }
}