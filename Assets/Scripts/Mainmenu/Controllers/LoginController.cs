using UnityEngine;
using Firebase.Auth;

public class LoginController : Controller
{
    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void Login()
    {
        AuthModel.LoginUser(
            "numberman3250@gmail.com",
            "tohpati123",
            onSuccess: (user) =>
            {
                Debug.Log($"Welcome, {user.Email}!");

                // Setelah login sukses, arahkan ke Lobby/Menu
                MenuManager.instance.DirectController("lobby");
            },
            onError: (errorMsg) =>
            {
                Debug.LogError($"Login Error: {errorMsg}");
            }
        );
    }

    public void Logout()
    {
        AuthModel.LogoutUser();
    }
}
