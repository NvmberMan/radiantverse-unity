using UnityEngine;

public class Controller : MonoBehaviour
{
    public string id = "New Controller";
    public GameObject panel;

    public virtual void Show() { panel.SetActive(true); }

    public virtual void Hide() { panel.SetActive(false); }
}
