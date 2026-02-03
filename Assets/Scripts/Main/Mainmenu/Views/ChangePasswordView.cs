using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class ChangePasswordView : View
    {
        public TMP_InputField oldPasswordField;
        public TMP_InputField newPasswordField;
        [SerializeField] private GameObject oldPasswordGroup;
        public Button submitButton;

        private void Start()
        {
            submitButton.onClick.AddListener(Submit);

        }

        private void OnEnable()
        {
            bool isEmailUser = AuthModel.HasPasswordProvider();

            oldPasswordGroup.SetActive(isEmailUser);

            oldPasswordField.text = "";
            newPasswordField.text = "";
        }

        public void Submit()
        {
            string oldPass = oldPasswordField.text;
            string newPass = newPasswordField.text;

            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass))
            {
                ShowError("Gagal", "Password tidak boleh kosong");
                return;
            }

            if (newPass.Length < 6)
            {
                ShowError("Gagal", "Password minimal 6 karakter");
                return;
            }

            ShowLoading();
            submitButton.interactable = false;

            AuthModel.ChangePassword(
                oldPass,
                newPass,
                () =>
                {
                    HideLoading();
                    submitButton.interactable = true;

                    ShowSuccess("Password berhasil diganti!", "");
                },
                (error) =>
                {
                    HideLoading();
                    submitButton.interactable = true;

                    ShowError("Gagal", error);
                });
        }


        private void ShowError(string title, string message)
        {
            ErrorView errorView =
                MenuManager.instance
                .GetController<UniversalController>()
                .GetView<ErrorView>();

            errorView.ErrorSetup(title, message);
            errorView.Show();
        }

        private void ShowSuccess(string title, string message)
        {
            SuccessView successView =
                MenuManager.instance
                .GetController<UniversalController>()
                .GetView<SuccessView>();

            successView.SuccessSetup(title, message, () =>
            {
                Hide();
                successView.Hide();
            });
            successView.Show();
        }

        private void ShowLoading()
        {
            View loadingView = MenuManager.instance.GetController<UniversalController>().GetView("loading");
            loadingView.Show();
        }

        private void HideLoading()
        {
            View loadingView =
                MenuManager.instance
                .GetController<UniversalController>()
                .GetView("loading");

            loadingView.Hide();
        }

    }
}