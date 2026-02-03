using UnityEngine;
using Firebase.Auth;
using TMPro;
using System.Collections;
using Google;
using System.Threading.Tasks;
using UnityEngine.UI; // Penting untuk Toggle dan Button

namespace Main.Mainmenu
{
    public class LoginController : Controller
    {
        [Header("Login Inputs")]
        [SerializeField] TMP_InputField inputEmail;
        [SerializeField] TMP_InputField inputPassword;

        [Header("Remember Me System")]
        [SerializeField] Toggle rememberMeToggle;

        [Header("Password Visibility")]
        [SerializeField] Button togglePasswordBtn;
        [SerializeField] Image eyeIcon;
        [SerializeField] Sprite eyeOpenSprite;
        [SerializeField] Sprite eyeClosedSprite;

        [Header("Social Login")]
        [SerializeField] Button googleLoginBtn;
        [SerializeField] string webClientId = "PASTE_YOUR_WEB_CLIENT_ID_HERE.apps.googleusercontent.com";

        private bool isPasswordVisible = false;

        private void Start()
        {
            // 1. Load data email yang tersimpan jika Remember Me aktif
            LoadSavedCredentials();

            // 2. Cek apakah user sudah login sebelumnya (Auto-redirect)
            if (AuthManager.instance.CurrentUser != null && PlayerLocalData.userData != null)
            {
                Debug.Log("User session found, directing to lobby...");
                Direct("lobby");
                return;
            }

            // 3. Setup event listener untuk AuthManager
            AuthManager.instance.OnUserLoggedIn += (user) =>
            {
                StartCoroutine(LoadAllPlayerDataCoroutine(user));
            };

            // 4. Setup listener untuk tombol intip password
            if (togglePasswordBtn != null)
                togglePasswordBtn.onClick.AddListener(TogglePasswordVisibility);

            //if (googleLoginBtn != null)
            //    googleLoginBtn.onClick.AddListener(SignInWithGoogle);
        }

        public void Login()
        {
            View loadingView = MenuManager.instance.GetController<UniversalController>().GetView("loading");
            loadingView.Show();

            // Simpan status "Remember Me" saat tombol login ditekan
            HandleRememberMeSelection();

            AuthModel.LoginUser(
                inputEmail.text,
                inputPassword.text,
                onSuccess: (user) =>
                {
                    loadingView.Hide();
                    StartCoroutine(LoadAllPlayerDataCoroutine(user));
                },
                onError: (errorMsg) =>
                {
                    loadingView.Hide();
                    ErrorView errorView = MenuManager.instance.GetController<UniversalController>().GetView<ErrorView>();
                    errorView.ErrorSetup("Failed to login!", errorMsg);
                    errorView.Show();
                }
            );
        }

        //public void SignInWithGoogle()
        //{
        //    // Tampilkan loading view
        //    View loadingView = MenuManager.instance.GetController<UniversalController>().GetView("loading");
        //    loadingView.Show();

        //    GoogleSignIn.Configuration = new GoogleSignInConfiguration
        //    {
        //        WebClientId = webClientId,
        //        RequestIdToken = true
        //    };

        //    GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task => {
        //        if (task.IsFaulted)
        //        {
        //            loadingView.Hide();
        //            Debug.LogError("Google Sign-In Faulted: " + task.Exception);
        //        }
        //        else if (task.IsCanceled)
        //        {
        //            loadingView.Hide();
        //            Debug.Log("Google Sign-In Canceled");
        //        }
        //        else
        //        {
        //            // Berhasil dapat ID Token dari Google, sekarang kirim ke Firebase
        //            SignInWithFirebase(task.Result.IdToken);
        //        }
        //    });
        //}

        //private void SignInWithFirebase(string idToken)
        //{
        //    Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        //    AuthManager.instance.Auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
        //        if (task.IsFaulted || task.IsCanceled)
        //        {
        //            Debug.LogError("Firebase Auth failed.");
        //            return;
        //        }

        //        FirebaseUser newUser = task.Result.User;
        //        Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

        //        // Kembali ke Main Thread Unity untuk menjalankan Coroutine
        //        UnityMainThreadDispatcher.Instance().Enqueue(() => {
        //            StartCoroutine(LoadAllPlayerDataCoroutine(newUser));
        //        });
        //    });
        //}

        #region UI Logic Features

        private void TogglePasswordVisibility()
        {
            isPasswordVisible = !isPasswordVisible;

            // Ubah mode input field
            inputPassword.contentType = isPasswordVisible ?
                TMP_InputField.ContentType.Standard :
                TMP_InputField.ContentType.Password;

            // Refresh tampilan secara visual
            inputPassword.ForceLabelUpdate();

            // Ganti icon mata jika referensi sprite tersedia
            if (eyeIcon != null)
            {
                eyeIcon.sprite = isPasswordVisible ? eyeOpenSprite : eyeClosedSprite;
            }
        }

        private void HandleRememberMeSelection()
        {
            if (rememberMeToggle.isOn)
            {
                PlayerPrefs.SetString("SavedEmail", inputEmail.text);
                PlayerPrefs.SetInt("RememberMeActive", 1);
            }
            else
            {
                PlayerPrefs.DeleteKey("SavedEmail");
                PlayerPrefs.SetInt("RememberMeActive", 0);
            }
            PlayerPrefs.Save();
        }

        private void LoadSavedCredentials()
        {
            if (PlayerPrefs.GetInt("RememberMeActive", 0) == 1)
            {
                rememberMeToggle.isOn = true;
                inputEmail.text = PlayerPrefs.GetString("SavedEmail", "");
            }
            else
            {
                rememberMeToggle.isOn = false;
            }
        }

        #endregion

        #region Data Loading Coroutines

        public IEnumerator LoadAllPlayerDataCoroutine(FirebaseUser user)
        {
            LoadingController loadingController = Controller.Get<LoadingController>();
            Direct("loading");

            const float MIN_DELAY_PER_ITEM = 0.2f;
            int loadedCount = 0;
            int totalItemsToLoad = 3;

            loadingController.SetLoadingText("Preparing to load player data...");
            loadingController.SetLoadingProgress(10);

            bool userDataLoaded = false;
            bool statsLoaded = false;
            bool inventoryDataLoaded = false;

            // Fungsi lokal untuk update progress bar
            IEnumerator UpdateProgressAndCheckLobby()
            {
                loadedCount++;
                int progress = (int)((float)loadedCount / totalItemsToLoad * 100);
                loadingController.SetLoadingProgress(progress);

                yield return new WaitForSeconds(MIN_DELAY_PER_ITEM);

                if (userDataLoaded && statsLoaded && inventoryDataLoaded)
                {
                    loadingController.SetLoadingText("Ready to go!");
                    loadingController.SetLoadingProgress(100);
                    yield return new WaitForSeconds(0.5f);
                    Direct("lobby");
                }
            }

            // 1. Load UserData
            loadingController.SetLoadingText("Preparing your experience...");
            yield return StartCoroutine(WaitForUserData(user,
                () => { userDataLoaded = true; },
                (err) => { Debug.LogWarning(err); FirestoreModel.InitializeUserData(user); userDataLoaded = true; }
            ));
            yield return StartCoroutine(UpdateProgressAndCheckLobby());

            // 2. Load PlayerStats
            loadingController.SetLoadingText("Loading your data...");
            yield return StartCoroutine(WaitForPlayerStats(user,
                () => { statsLoaded = true; },
                (err) => { Debug.LogWarning(err); FirestoreModel.InitializePlayerStats(user); statsLoaded = true; }
            ));
            yield return StartCoroutine(UpdateProgressAndCheckLobby());

            // 3. Load InventoryData
            loadingController.SetLoadingText("Setting things up...");
            yield return StartCoroutine(WaitForInventoryData(user,
                () => { inventoryDataLoaded = true; },
                (err) => { Debug.LogWarning(err); FirestoreModel.InitializeInventoryData(user); inventoryDataLoaded = true; }
            ));
            yield return StartCoroutine(UpdateProgressAndCheckLobby());
        }

        private IEnumerator WaitForUserData(FirebaseUser user, System.Action onSuccess, System.Action<string> onError)
        {
            bool done = false;
            FirestoreModel.GetUserData(user,
                data => { PlayerLocalData.userData = data; onSuccess(); done = true; },
                err => { onError(err); done = true; }
            );
            while (!done) { yield return null; }
        }

        private IEnumerator WaitForPlayerStats(FirebaseUser user, System.Action onSuccess, System.Action<string> onError)
        {
            bool done = false;
            FirestoreModel.GetPlayerStats(user,
                data => { PlayerLocalData.playerStats = data; onSuccess(); done = true; },
                err => { onError(err); done = true; }
            );
            while (!done) { yield return null; }
        }

        private IEnumerator WaitForInventoryData(FirebaseUser user, System.Action onSuccess, System.Action<string> onError)
        {
            bool done = false;
            FirestoreModel.GetInventoryData(user,
                data => { PlayerLocalData.inventoryData = data; onSuccess(); done = true; },
                err => { onError(err); done = true; }
            );
            while (!done) { yield return null; }
        }

        #endregion
    }
}