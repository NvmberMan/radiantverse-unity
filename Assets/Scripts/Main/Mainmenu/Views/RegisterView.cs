using UnityEngine;

namespace Main.Mainmenu
{
    public class RegisterView : View
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