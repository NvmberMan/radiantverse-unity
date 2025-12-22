using Firebase.Auth;
using System;
using UnityEngine;


namespace Main.Mainmenu
{
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

        public static async void RegisterUser(string email, string password, Action<FirebaseUser> onSuccess = null, Action<string> onError = null)
        {
            try
            {
                var result = await AuthManager.instance.auth.CreateUserWithEmailAndPasswordAsync(email, password);
                FirebaseUser user = result.User;

                Debug.Log($"Account created successfully: {user.Email}");
                onSuccess?.Invoke(user);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Registration failed: {ex.Message}");
                onError?.Invoke(ex.Message);
            }
        }


        public static void LogoutUser(Action onSuccess = null, Action<string> onError = null)
        {
            var auth = AuthManager.instance.auth;

            try
            {
                if (auth.CurrentUser != null)
                {
                    Debug.Log($"User {auth.CurrentUser.Email} logged out.");
                    auth.SignOut();
                    onSuccess?.Invoke();
                }
                else
                {
                    Debug.Log("No user currently logged in.");
                    onError?.Invoke("No user currently logged in.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Logout failed: {ex.Message}");
                onError?.Invoke(ex.Message);
            }
        }

    }
}