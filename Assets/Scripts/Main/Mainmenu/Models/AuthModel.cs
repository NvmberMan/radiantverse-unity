using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using UnityEngine;
using System.Threading.Tasks;


namespace Main.Mainmenu
{
    public static class AuthModel
    {
        private const int TIMEOUT_MILLISECONDS = 10000;

        public static async void LoginUser(
            string email,
            string password,
            Action<FirebaseUser> onSuccess = null,
            Action<string> onError = null)
        {
            try
            {
                // Membuat task Firebase
                var loginTask = AuthManager.instance.auth.SignInWithEmailAndPasswordAsync(email, password);

                // Membalapkan task Firebase dengan Task.Delay (Timeout)
                var completedTask = await Task.WhenAny(loginTask, Task.Delay(TIMEOUT_MILLISECONDS));

                if (completedTask == loginTask)
                {
                    // Jika login selesai tepat waktu
                    AuthResult result = await loginTask;
                    FirebaseUser user = result.User;
                    Debug.Log($"Login successful: {user.Email}");
                    onSuccess?.Invoke(user);
                }
                else
                {
                    // Jika Task.Delay yang selesai duluan (Timeout)
                    onError?.Invoke("Connection timeout. Please check your internet.");
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke(GetAuthErrorMessage(ex));
            }
        }

        public static async void RegisterUser(
            string email,
            string password,
            Action<FirebaseUser> onSuccess = null,
            Action<string> onError = null)
        {
            try
            {
                var registerTask = AuthManager.instance.auth.CreateUserWithEmailAndPasswordAsync(email, password);

                var completedTask = await Task.WhenAny(registerTask, Task.Delay(TIMEOUT_MILLISECONDS));

                if (completedTask == registerTask)
                {
                    AuthResult result = await registerTask;
                    FirebaseUser user = result.User;
                    Debug.Log($"Account created successfully: {user.Email}");
                    onSuccess?.Invoke(user);
                }
                else
                {
                    onError?.Invoke("Request timed out. Please try again.");
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke(GetAuthErrorMessage(ex));
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

        public static bool HasPasswordProvider()
        {
            FirebaseUser user = AuthManager.instance.CurrentUser;
            if (user == null) return false;

            foreach (var profile in user.ProviderData)
            {
                if (profile.ProviderId == "password") return true;
            }
            return false;
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
                onError?.Invoke("User is not logged in.");
                return;
            }

            try
            {
                if (HasPasswordProvider())
                {
                    if (string.IsNullOrEmpty(oldPassword))
                    {
                        onError?.Invoke("Old password is required.");
                        return;
                    }
                    Credential credential = EmailAuthProvider.GetCredential(user.Email, oldPassword);
                    await user.ReauthenticateAsync(credential);
                }

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

        private static string GetFriendlyErrorMessage(AuthError errorCode)
        {
            return errorCode.ToString() switch
            {
                "MissingPassword" => "Please enter your password.",
                "MissingEmail" => "Please enter your email address.",
                "Failure" => "Invalid email or password. Please try again.",
                _ => $"Authentication error: {errorCode}"
            };
        }

        private static string GetAuthErrorMessage(Exception ex)
        {
            Exception baseException = ex.GetBaseException();
            FirebaseException firebaseEx = baseException as FirebaseException;

            if (firebaseEx != null)
            {
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                return GetFriendlyErrorMessage(errorCode);
            }

            return "An unexpected error occurred.";
        }


    }
}