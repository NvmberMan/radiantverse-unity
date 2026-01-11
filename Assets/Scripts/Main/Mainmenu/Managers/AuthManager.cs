using Firebase;
using Firebase.Auth;
using System;
using UnityEngine;

namespace Main.Mainmenu
{
    public class AuthManager : MonoBehaviour
    {
        public bool IsDebug;

        public static AuthManager instance;
        public bool HasInitiallyLoaded { get; private set; } = false;
        public FirebaseAuth auth;

        public FirebaseUser CurrentUser => auth?.CurrentUser;

        public event Action<FirebaseUser> OnUserLoggedIn;
        public event Action OnUserLoggedOut;


        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void ResetInitialLoad()
        {
            HasInitiallyLoaded = false;

            PlayerLocalData.Clear();
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

                    if(IsDebug)
                        Debug.Log("Firebase initialized!");

                    auth.StateChanged += OnAuthStateChanged;
                    OnAuthStateChanged(this, null);
                }
                else
                {
                    if (IsDebug)
                        Debug.LogError($"Firebase dependency error: {task.Result}");
                }
            });
        }

        private async void OnAuthStateChanged(object sender, EventArgs e)
        {
            if (auth.CurrentUser != null)
            {
                if (HasInitiallyLoaded) return;

                try
                {
                    var token = await auth.CurrentUser.TokenAsync(true);

                    if (!string.IsNullOrEmpty(token))
                    {
                        if (IsDebug)
                            Debug.Log($"User is still logged in: {auth.CurrentUser.Email}");

                        // ? Jangan panggil event langsung di thread Firebase
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            HasInitiallyLoaded = true;
                            OnUserLoggedIn?.Invoke(auth.CurrentUser);
                        });
                    }
                }
                catch (Exception ex)
                {
                    if (IsDebug)
                        Debug.LogError($"Auth validation failed: {ex.Message}");
                }
            }
            else
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    OnUserLoggedOut?.Invoke();
                });
            }
        }
    }
}