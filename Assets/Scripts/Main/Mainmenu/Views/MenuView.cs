using UnityEngine;

namespace Main.Mainmenu
{
    public class MenuView : View
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MenuManager.instance.GetController<UniversalController>().GetView("exit").Show();
            }
        }
    }
}