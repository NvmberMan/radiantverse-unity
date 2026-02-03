using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Main.Mainmenu
{
    public class RegisterController : Controller
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField inputEmail;
        [SerializeField] private TMP_InputField inputPassword;
        [SerializeField] private TMP_InputField inputConfirmPassword;

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

        private bool ValidatePasswords()
        {
            string email = inputEmail.text;
            string pass = inputPassword.text;
            string confirmPass = inputConfirmPassword.text;

            // Cek Kosong
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                ShowError("Validation Error", "Email and Password cannot be empty!");
                return false;
            }

            // Validasi Format Email (Regex Standard)
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                ShowError("Invalid Email", "Please enter a valid email address (e.g., name@example.com).");
                return false;
            }

            // Cek Kecocokan Password
            if (pass != confirmPass)
            {
                ShowError("Password Mismatch", "Passwords do not match.");
                return false;
            }

            // Cek Kekuatan Password (Minimal 6 karakter adalah syarat Firebase)
            if (pass.Length < 6)
            {
                ShowError("Weak Password", "Password must be at least 6 characters long.");
                return false;
            }

            return true;
        }

        private IEnumerator InitializeAllPlayerDataCoroutine(FirebaseUser user)
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