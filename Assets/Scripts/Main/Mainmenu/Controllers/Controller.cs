using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Main.Mainmenu
{
    public class Controller : MonoBehaviour
    {
        public string id = "New Controller";
        public List<View> viewList = new List<View>();

        public static T Get<T>() where T : Controller
        {
            if (MenuManager.instance == null)
            {
                Debug.LogError("MenuManager instance is not set!");
                return null;
            }
            return MenuManager.instance.GetController<T>();
        }

        public View GetView(string targetId)
        {
            return viewList.Find(x => x.id == targetId);
        }

        public T GetView<T> () where T : View
        {
            return viewList.OfType<T>().FirstOrDefault();
        }

        public virtual void Activate(string targetView)
        {
            var target = viewList.Find(view => view.id == targetView);

            if (target == null)
            {
                Debug.LogError($"View {targetView}'s not found");
                return;
            }

            target.Show();
        }

        public virtual void ActivateOneAndHidingAll(string targetView)
        {
            DisactivateAll();

            Activate(targetView);
        }

        public virtual void Disactivate(string targetView)
        {
            var target = viewList.Find(view => view.id == targetView);
            target.Hide();
        }

        public virtual void DisactivateAll()
        {
            foreach (var view in viewList)
            {
                view.Hide();
            }
        }

        public virtual void Direct(string targetId) { MenuManager.instance.DirectController(targetId); }
        public virtual void Direct(int index) { MenuManager.instance.DirectController(index); }
    }
}