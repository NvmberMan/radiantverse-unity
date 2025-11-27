using UnityEngine;

public class Controller : MonoBehaviour
{
    public string id = "New Controller";
    public GameObject panel;
    public static T Get<T>() where T : Controller
    {
        if (MenuManager.instance == null)
        {
            Debug.LogError("MenuManager instance is not set!");
            return null;
        }
        return MenuManager.instance.GetController<T>();
    }

    public virtual void Show() { panel.SetActive(true); }

    public virtual void Hide() { panel.SetActive(false); }

                
    public virtual void Direct(string targetId) { MenuManager.instance.DirectController(targetId);  }
    public virtual void Direct(int index) { MenuManager.instance.DirectController(index); }
}
