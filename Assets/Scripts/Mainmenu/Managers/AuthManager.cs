using Firebase;
using Firebase.Auth;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    public FirebaseAuth auth;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase initialized!");
            }
            else
            {
                Debug.LogError($"Firebase dependency error: {task.Result}");
            }
        });
    }
}
