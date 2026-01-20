using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Main.Mainmenu
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;

        public int ActiveIndex = 0;
        public List<Controller> ControllerList = new List<Controller>();

        private void Awake()
        {
            instance = this;

            if(ActiveIndex >= 0 && ActiveIndex < ControllerList.Count)
                DirectController(ActiveIndex);
        }

        public void DirectController(int index)
        {
            HideAllView();

            ControllerList[index].Activate("base");
        }

        public void DirectController(string idTarget)
        {
            HideAllView();
            var target = ControllerList.Find(controller => controller.id == idTarget);

            if (target == null)
                Debug.LogWarning($"Controller dengan ID '{idTarget}' TIDAK ditemukan!");

            target.Activate("base");

        }

        public void HideAllView()
        {
            for (int i = 0; i < ControllerList.Count; i++)
            {
                if (ControllerList[i] == null)
                {
                    Debug.LogError($"ControllerList[{i}] is NULL!");
                    continue;
                }

                if (ControllerList[i].id == "universal") continue;

                ControllerList[i].DisactivateAll();
            }
        }

        public T GetController<T>() where T : Controller
        {
            return ControllerList.OfType<T>().FirstOrDefault();
        }

        public Controller GetController(string targetController)
        {
            return ControllerList.Find((c) => c.id == targetController);
        }

    }
}