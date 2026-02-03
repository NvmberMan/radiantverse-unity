using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InputNavigator : MonoBehaviour
{
    private EventSystem system;

    [Header("Sequence of Navigation")]
    [SerializeField] private Selectable[] selectables;

    void Start()
    {
        system = EventSystem.current;
    }

    void Update()
    {
        // 1. Logika Pindah Fokus dengan TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;

            // Cek apakah sedang Shift + Tab (mundur) atau Tab saja (maju)
            bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (system.currentSelectedGameObject != null)
            {
                Selectable current = system.currentSelectedGameObject.GetComponent<Selectable>();
                if (current != null)
                {
                    next = isShiftDown ? current.FindSelectableOnUp() : current.FindSelectableOnDown();
                }
            }

            // Jika navigasi otomatis tidak ketemu, gunakan list manual kita
            if (next == null && selectables.Length > 0)
            {
                next = selectables[0];
            }

            if (next != null)
            {
                next.Select();

                // Jika itu InputField, otomatis aktifkan kursor ketik
                TMP_InputField inputField = next.GetComponent<TMP_InputField>();
                if (inputField != null) inputField.ActivateInputField();
            }
        }

        // 2. Logika Submit dengan ENTER
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Cek jika fokus ada di elemen terakhir (biasanya tombol Login/Register)
            // Atau jika user sedang di InputField terakhir, langsung trigger button
            GameObject currentObj = system.currentSelectedGameObject;
            if (currentObj != null)
            {
                Button btn = currentObj.GetComponent<Button>();
                if (btn != null && btn.interactable)
                {
                    btn.onClick.Invoke();
                }
                else
                {
                    // Opsional: Jika masih di input field terakhir, langsung klik tombol utama
                    // TriggerButtonUtama();
                }
            }
        }
    }
}