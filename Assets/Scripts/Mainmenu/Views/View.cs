using UnityEngine;

public class View : MonoBehaviour
{
    public string id = "New View";

    [Header("View Variables")]
    [SerializeField] private GameObject panel;
    public virtual void Show() { panel.SetActive(true); }
    public virtual void Hide() { panel.SetActive(false); }
}
