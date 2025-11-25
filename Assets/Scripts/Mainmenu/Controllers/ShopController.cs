using UnityEngine;

public class ShopController : MonoBehaviour
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
