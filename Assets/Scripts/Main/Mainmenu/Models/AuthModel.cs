using Firebase.Auth;
using Firebase.Extensions;
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

        public static async void ChangePassword(
            string oldPassword,
            string newPassword,
            Action onSuccess,
            Action<string> onError)
        {
            FirebaseUser user = AuthManager.instance.CurrentUser;

            if (user == null)
            {
                onError?.Invoke("User belum login.");
                return;
            }

            try
            {
                Credential credential =
                    EmailAuthProvider.GetCredential(user.Email, oldPassword);

                await user.ReauthenticateAsync(credential);

                await user.UpdatePasswordAsync(newPassword);

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }

        public static void SendPasswordReset(string email, Action onSuccess, Action<string> onError)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task => {
                if (task.IsCanceled || task.IsFaulted)
                {
                    onError?.Invoke("Failed to send reset email. Make sure the email is registered.");
                }
                else
                {
                    onSuccess?.Invoke();
                }
            });
        }

    }
}