using UnityEngine;
using Firebase.Auth;
using TMPro;
using System.Collections;

namespace Main.Mainmenu
{
    public class RegisterController : Controller
    {
        [Header("Controller Variables")]
        [SerializeField] TMP_InputField inputEmail;
        [SerializeField] TMP_InputField inputPassword;


        public void Register()
        {
            //LoadingPopupController popupController = Controller.Get<LoadingPopupController>();
            //popupController?.view.Show();
            Activate("loading");

            AuthModel.RegisterUser(inputEmail.text, inputPassword.text,
                onSuccess: (user) =>
                {
                    Debug.Log($"User registered: {user.Email}");
                    Disactivate("loading");

                    // Masuk ke loading screen dengan coroutine, sama seperti Login
                    StartCoroutine(InitializeAllPlayerDataCoroutine(user));
                },
                onError: (error) =>
                {
                    Debug.LogError($"Register error: {error}");
                    Disactivate("loading");
                });
        }

        private IEnumerator InitializeAllPlayerDataCoroutine(FirebaseUser user)
        {
            LoadingController loadingController = Controller.Get<LoadingController>();
            Direct("loading"); // pindah ke loading page

            const float MIN_DELAY_PER_ITEM = 0.2f;
            int loadedCount = 0;
            int totalItemsToLoad = 3;

            loadingController.SetLoadingText("Preparing new account data...");

            bool userDataCreated = false;
            bool statsCreated = false;
            bool inventoryCreated = false;

            IEnumerator UpdateProgressAndCheckLobby()
            {
                loadedCount++;
                int progress = (int)((float)loadedCount / totalItemsToLoad * 100);
                loadingController.SetLoadingProgress(progress);

                yield return new WaitForSeconds(MIN_DELAY_PER_ITEM);

                if (userDataCreated && statsCreated && inventoryCreated)
                {
                    loadingController.SetLoadingText("Account setup completed!");
                    loadingController.SetLoadingProgress(100);
                    yield return new WaitForSeconds(0.5f);
                    Direct("lobby");
                }
            }

            loadingController.SetLoadingText("Creating User Profile...");
            yield return StartCoroutine(GenerateUserData(user, () => { userDataCreated = true; }));
            yield return StartCoroutine(UpdateProgressAndCheckLobby());

            loadingController.SetLoadingText("Creating Player Statistics...");
            yield return StartCoroutine(GeneratePlayerStats(user, () => { statsCreated = true; }));
            yield return StartCoroutine(UpdateProgressAndCheckLobby());

            loadingController.SetLoadingText("Creating Inventory Data...");
            yield return StartCoroutine(GenerateInventoryData(user, () => { inventoryCreated = true; }));
            yield return StartCoroutine(UpdateProgressAndCheckLobby());
        }


        private IEnumerator GenerateUserData(FirebaseUser user, System.Action onComplete)
        {
            FirestoreModel.InitializeUserData(user);
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();
        }

        private IEnumerator GeneratePlayerStats(FirebaseUser user, System.Action onComplete)
        {
            FirestoreModel.InitializePlayerStats(user);
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();
        }

        private IEnumerator GenerateInventoryData(FirebaseUser user, System.Action onComplete)
        {
            FirestoreModel.InitializeInventoryData(user);
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();
        }
    }
}