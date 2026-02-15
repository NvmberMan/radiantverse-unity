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

        [Header("Password Visibility (New)")]
        [SerializeField] private Button toggleOldPassBtn;
        [SerializeField] private Image oldPassEyeIcon;
        [Space]
        [SerializeField] private Button toggleNewPassBtn;
        [SerializeField] private Image newPassEyeIcon;
        [Space]
        [SerializeField] private Sprite eyeOpenSprite;
        [SerializeField] private Sprite eyeClosedSprite;

        private bool isOldPassVisible = false;
        private bool isNewPassVisible = false;

        private void Start()
        {
            submitButton.onClick.AddListener(Submit);

            // Setup listener untuk tombol intip password (Sama persis dengan Register)
            if (toggleOldPassBtn != null)
                toggleOldPassBtn.onClick.AddListener(() => ToggleVisibility(ref isOldPassVisible, oldPasswordField, oldPassEyeIcon));

            if (toggleNewPassBtn != null)
                toggleNewPassBtn.onClick.AddListener(() => ToggleVisibility(ref isNewPassVisible, newPasswordField, newPassEyeIcon));

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
                () => {
                    // Gunakan dispatcher agar UI tidak crash/error null
                    UnityMainThreadDispatcher.Instance.Enqueue(() => {
                        HideLoading();
                        submitButton.interactable = true;
                        ShowSuccess("Password berhasil diganti!", "");
                    });
                },
                (error) => {
                    UnityMainThreadDispatcher.Instance.Enqueue(() => {
                        HideLoading();
                        submitButton.interactable = true;
                        ShowError("Gagal", error);
                    });
                }
            );
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