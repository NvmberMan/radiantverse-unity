using UnityEngine;

namespace Main.Mainmenu
{
    public class LoginView : View
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