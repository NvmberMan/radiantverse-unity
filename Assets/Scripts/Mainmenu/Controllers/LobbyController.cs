using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public GameObject panel;
    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
