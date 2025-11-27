using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public int ActiveIndex = 0;
    public List<Controller> ControllerList = new List<Controller>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DirectController(ActiveIndex);
    }

    public void DirectController(int index)
    {
        HideAllController();

        ControllerList[index].Show();
    }

    public void DirectController(string idTarget)
    {
        HideAllController();
        var target = ControllerList.Find(controller => controller.id == idTarget);

        if (target == null)
            Debug.LogWarning($"Controller dengan ID '{idTarget}' TIDAK ditemukan!");

        target.Show();

    }

    public void HideAllController()
    {
        for (int i = 0; i < ControllerList.Count; i++)
        {
            if (ControllerList[i] == null)
            {
                Debug.LogError($"ControllerList[{i}] is NULL!");
                continue;
            }

            ControllerList[i].Hide();
        }
    }

    public T GetController<T>() where T : Controller
    {
        return ControllerList.OfType<T>().FirstOrDefault();
    }

}
