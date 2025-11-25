using Firebase.Auth;
using System;
using UnityEngine;

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
        LoginUser("numberman3250@gmail.com", "tohpati123");
    }

    public async void LoginUser(string email, string password)
    {
        try
        {
            var result = await AuthManager.instance.auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;

            Debug.Log($"Login successful: {user.Email}");

            // Panggil setelah sukses login
            MenuManager.instance.DirectController("lobby");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
        }
    }


    public void Logout()
    {
        if (AuthManager.instance.auth.CurrentUser != null)
        {
            Debug.Log($"User {AuthManager.instance.auth.CurrentUser.Email} logout.");
            AuthManager.instance.auth.SignOut();
        }
    }
}
