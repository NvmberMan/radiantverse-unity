using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class ForgotPasswordController : Controller
    {
        [SerializeField] private TMP_InputField inputEmail;
        [SerializeField] private Button submitButton;

        private void Start()
        {
            submitButton.onClick.AddListener(HandleForgotPassword);
        }

        public void HandleForgotPassword()
        {
            string email = inputEmail.text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Input Error", "Please enter your email address first.");
                return;
            }

            View loadingView = MenuManager.instance.GetController<UniversalController>().GetView("loading");
            loadingView.Show();

            AuthModel.SendPasswordReset(email,
                onSuccess: () => {
                    loadingView.Hide();
                    // Tampilkan pesan sukses kepada user
                    SuccessView success = MenuManager.instance.GetController<UniversalController>().GetView<SuccessView>();
                    success.SuccessSetup("Email Sent!", "Please check your inbox (and spam folder) for the reset link.", () =>
                    {
                        Direct("login");
                    });
                    success.Show();
                },
                onError: (errorMsg) => {
                    loadingView.Hide();
                    ShowError("Reset Failed", errorMsg);
                }
            );
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