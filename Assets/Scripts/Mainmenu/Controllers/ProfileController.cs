using UnityEngine;
using Firebase.Auth;
using TMPro;

public class ProfileController : Controller
{
    //[Header("Controller Variables")]
    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void Logout()
    {
        AuthModel.LogoutUser(
           onSuccess: () =>
           {
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
