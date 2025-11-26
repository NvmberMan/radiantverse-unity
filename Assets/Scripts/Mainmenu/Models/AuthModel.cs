using Firebase.Auth;
using System;
using UnityEngine;

public static class AuthModel
{
    public static async void LoginUser(string email, string password, Action<FirebaseUser> onSuccess = null, Action<string> onError = null)
    {
        try
        {
            var result = await AuthManager.instance.auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;

            Debug.Log($"Login successful: {user.Email}");
            onSuccess?.Invoke(user);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
            onError?.Invoke(ex.Message);
        }
    }

    public static void LogoutUser()
    {
        var auth = AuthManager.instance.auth;

        if (auth.CurrentUser != null)
        {
            Debug.Log($"User {auth.CurrentUser.Email} logged out.");
            auth.SignOut();
        }
        else
        {
            Debug.Log("No user currently logged in.");
        }
    }
}
