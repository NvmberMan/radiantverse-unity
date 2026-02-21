using Firebase.Auth;
using Firebase.Extensions;
using Google;
using Main.Gameplay;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Penting untuk Button dan Image


namespace Main.Mainmenu
{
    public class RegisterController : Controller
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField inputEmail;
        [SerializeField] private TMP_InputField inputPassword;
        [SerializeField] private TMP_InputField inputConfirmPassword;

        [Header("Password Visibility")]
        [SerializeField] private Button togglePassBtn;
        [SerializeField] private Image passEyeIcon;
        [Space]
        [SerializeField] private Button toggleConfirmPassBtn;
        [SerializeField] private Image confirmPassEyeIcon;
        [Space]
        [SerializeField] private Sprite eyeOpenSprite;
        [SerializeField] private Sprite eyeClosedSprite;

        [Header("Google Sign Up")]
        [SerializeField] Button googleRegisterBtn;
        [SerializeField] string webClientId = "230119757559-h7s33t3pl6j09a1760qlpe5622q6ce18.apps.googleusercontent.com";

        private bool isPassVisible = false;
        private bool isConfirmPassVisible = false;

        private void Start()
        {
            // Setup listener untuk tombol intip password
            if (togglePassBtn != null)
                togglePassBtn.onClick.AddListener(() => ToggleVisibility(ref isPassVisible, inputPassword, passEyeIcon));

            if (toggleConfirmPassBtn != null)
                toggleConfirmPassBtn.onClick.AddListener(() => ToggleVisibility(ref isConfirmPassVisible, inputConfirmPassword, confirmPassEyeIcon));

            if (googleRegisterBtn != null)
                googleRegisterBtn.onClick.AddListener(SignUpWithGoogle);

        }

        private void ToggleVisibility(ref bool isVisible, TMP_InputField input, Image icon)
        {
            isVisible = !isVisible;

            input.contentType = isVisible ?
                TMP_InputField.ContentType.Standard :
                TMP_InputField.ContentType.Password;

            input.ForceLabelUpdate();

            if (icon != null && eyeOpenSprite != null && eyeClosedSprite != null)
            {
                icon.sprite = isVisible ? eyeOpenSprite : eyeClosedSprite;
            }
        }

        public void Register()
        {
            inputEmail.text = inputEmail.text.Trim();

            if (!ValidatePasswords()) return;

            AuthManager.instance.IsRegistering = true;
            View loadingView = MenuManager.instance.GetController<UniversalController>().GetView("loading");
            loadingView.Show();

            AuthModel.RegisterUser(inputEmail.text, inputPassword.text,
                onSuccess: (user) => {
                    loadingView.Hide();
                    StartCoroutine(InitializeAllPlayerDataCoroutine(user));
                },
                onError: (error) => {
                    loadingView.Hide();
                    AuthManager.instance.IsRegistering = false;
                    ShowError("Registration Failed", error);
                });
        }

        public void SignUpWithGoogle()
        {
            View loadingView = MenuManager.instance.GetController<UniversalController>().GetView("loading");
            loadingView.Show();

            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                loadingView.Hide();
                ErrorView errorView = MenuManager.instance.GetController<UniversalController>().GetView<ErrorView>();
                errorView.ErrorSetup("Failed to login!", "Please use mobile!");
                errorView.Show();

                return;
            }

            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                UseGameSignIn = false,
                RequestEmail = true,
            };

            GoogleSignIn.DefaultInstance.SignOut();

            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    loadingView.Hide();
                    Debug.LogError("Google Sign-In Faulted: " + task.Exception);

                    ErrorView errorView = MenuManager.instance.GetController<UniversalController>().GetView<ErrorView>();
                    errorView.ErrorSetup("Failed to login!", "");
                    errorView.Show();
                }
                else if (task.IsCanceled)
                {
                    loadingView.Hide();
                    Debug.Log("Google Sign-In Canceled");

                    ErrorView errorView = MenuManager.instance.GetController<UniversalController>().GetView<ErrorView>();
                    errorView.ErrorSetup("Google Sign-In Canceled", "");
                    errorView.Show();
                }
                else
                {
                    loadingView.Hide();
                    SignInWithFirebase(task.Result.IdToken);
                }
            });
        }


        private void SignInWithFirebase(string idToken)
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

            AuthManager.instance.auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Firebase Auth failed.");
                    return;
                }

                FirebaseUser newUser = task.Result.User;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

                // Kembali ke Main Thread Unity untuk menjalankan Coroutine
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    //StartCoroutine(LoadAllPlayerDataCoroutine(newUser));
                    StartCoroutine(CheckUserRegistrationStatus(newUser));
                });
            });
        }

        private IEnumerator CheckUserRegistrationStatus(FirebaseUser user)
        {
            bool checkDone = false;
            bool isRegistered = false;

            FirestoreModel.GetUserData(user,
                onSuccess: (data) => {
                    isRegistered = (data != null);
                    checkDone = true;
                },
                onError: (error) => {
                    isRegistered = false;
                    checkDone = true;
                }
            );

            while (!checkDone) yield return null;

            if (isRegistered)
            {
                Debug.Log("User lama: Langsung loading data.");
                LoginController loginController = MenuManager.instance.GetController<LoginController>();
                StartCoroutine(loginController.LoadAllPlayerDataCoroutine(user));
            }
            else
            {
                Debug.Log("User baru: Jalankan proses inisialisasi (Registrasi).");
                StartCoroutine(InitializeAllPlayerDataCoroutine(user));
            }
        }



        private bool ValidatePasswords()
        {
            string email = inputEmail.text;
            string pass = inputPassword.text;
            string confirmPass = inputConfirmPassword.text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                ShowError("Validation Error", "Email and Password cannot be empty!");
                return false;
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                ShowError("Invalid Email", "Please enter a valid email address.");
                return false;
            }

            if (pass != confirmPass)
            {
                ShowError("Password Mismatch", "Passwords do not match.");
                return false;
            }

            if (pass.Length < 6)
            {
                ShowError("Weak Password", "Password must be at least 6 characters long.");
                return false;
            }

            return true;
        }

        public IEnumerator InitializeAllPlayerDataCoroutine(FirebaseUser user)
        {
            LoadingController loadingController = Controller.Get<LoadingController>();
            Direct("loading");

            int loadedCount = 0;
            int totalItemsToLoad = 3;

            loadingController.SetLoadingText("Creating User Profile...");
            bool userDone = false;
            FirestoreModel.InitializeUserData(user, (data) => {
                PlayerLocalData.userData = data;
                userDone = true;
            });
            while (!userDone) yield return null;
            yield return StartCoroutine(UpdateProgress(loadingController, ++loadedCount, totalItemsToLoad));

            loadingController.SetLoadingText("Setting up stats...");
            bool statsDone = false;
            FirestoreModel.InitializePlayerStats(user, (stats) => {
                PlayerLocalData.playerStats = stats;
                PlayerSkillManager.Instance.currentPlayerSkill = stats.PlayerSkillRating;
                statsDone = true;
            });
            while (!statsDone) yield return null;
            yield return StartCoroutine(UpdateProgress(loadingController, ++loadedCount, totalItemsToLoad));

            loadingController.SetLoadingText("Preparing inventory...");
            bool invDone = false;
            FirestoreModel.InitializeInventoryData(user, (inv) => {
                PlayerLocalData.inventoryData = inv;
                invDone = true;
            });
            while (!invDone) yield return null;
            yield return StartCoroutine(UpdateProgress(loadingController, ++loadedCount, totalItemsToLoad));

            loadingController.SetLoadingText("Account Ready!");
            yield return new WaitForSeconds(0.8f);

            AuthManager.instance.IsRegistering = false;
            Direct("create username");
        }

        private IEnumerator UpdateProgress(LoadingController ctrl, int count, int total)
        {
            ctrl.SetLoadingProgress((int)((float)count / total * 100));
            yield return new WaitForSeconds(0.2f);
        }

        private void ShowError(string title, string message)
        {
            ErrorView errorView = MenuManager.instance.GetController<UniversalController>().GetView<ErrorView>();
            if (errorView != null)
            {
                errorView.ErrorSetup(title, message);
                errorView.Show();
            }
            else
            {
                Debug.LogError($"[RegisterController] ErrorView not found! {title}: {message}");
            }
        }
    }
}