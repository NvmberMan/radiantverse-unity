using Firebase.Auth;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Main.Mainmenu
{
    public class CreateUsernameController : Controller
    {
        [SerializeField] private TMP_InputField usernameField;
        public void ValidateUsername()
        {
            string inputName = usernameField.text.Trim();

            if (inputName.Length <= 2)
            {
                ShowError("Invalid Username", "Please choose at least 3 characters!");
                return;
            }

            FirestoreModel.CheckUsernameExists(inputName, (exists) =>
            {
                if (exists)
                {
                    ShowError("Username Taken", "This username is already in use. Try another one!");
                }
                else
                {
                    Debug.Log("Username tersedia! Lanjutkan proses simpan...");
                    UpdateUsername(inputName);
                }
            });
        }

        private void UpdateUsername(string newUsername)
        {
            AuthManager.instance.IsRegistering = false;

            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Username", newUsername }
            };

            FirestoreModel.SaveUserData(uid, updates);
            PlayerLocalData.userData.Username = newUsername;

            Direct("choose default character");
            Debug.Log("direct");
        }

        private void ShowError(string title, string message)
        {
            ErrorView errorView = MenuManager.instance.GetController<UniversalController>().GetView<ErrorView>();
            errorView.ErrorSetup(title, message);
            errorView.Show();
        }
    }
}