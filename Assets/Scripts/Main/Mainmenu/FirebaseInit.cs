using UnityEngine;
using Firebase;
using Firebase.Extensions;

namespace Main.Mainmenu
{
    public class FirebaseInit : MonoBehaviour
    {
        FirebaseApp app;

        void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    app = FirebaseApp.DefaultInstance;
                    Debug.Log("Firebase is ready!");
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }
    }
}