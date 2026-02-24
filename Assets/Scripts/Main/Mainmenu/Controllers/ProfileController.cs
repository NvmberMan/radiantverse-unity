using UnityEngine;
using Firebase.Auth;
using TMPro;

namespace Main.Mainmenu
{
    public class ProfileController : Controller
    {
        //[Header("Controller Variables")]
        public void Logout()
        {
            AuthModel.LogoutUser(
               onSuccess: () =>
               {
                   AuthManager.instance.ResetInitialLoad();
                   Debug.Log("Logout success, moving to Login Page...");
                   MenuManager.instance.DirectController("login");
               },
               onError: (error) =>
               {
                   Debug.LogError("Logout error: " + error);
               }
           );
        }
    }
}
