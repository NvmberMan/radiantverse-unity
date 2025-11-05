using Firebase;
using Firebase.Auth;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private FirebaseAuth auth;  // untuk autentikasi Firebase

    void Start()
    {
        // Inisialisasi Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase initialized!");

                LoginUser("numberman3250@gmail.com", "tohpati123");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    void LoginUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                return;
            }

            AuthResult result = task.Result; 
            FirebaseUser user = result.User;  
            Debug.Log($"Login successful: {user.Email}");
        });
    }

    public void Logout()
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log($"User {auth.CurrentUser.Email} logout.");
            auth.SignOut();
        }
    }
}
