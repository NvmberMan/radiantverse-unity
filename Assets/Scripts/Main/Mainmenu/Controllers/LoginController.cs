using UnityEngine;
using Firebase.Auth;
using TMPro;
using System.Collections;

namespace Main.Mainmenu
{
    public class LoginController : Controller
    {
        [Header("Controller Variables")]
        [SerializeField] TMP_InputField inputEmail;
        [SerializeField] TMP_InputField inputPassword;


        private void Start()
        {
            AuthManager.instance.OnUserLoggedIn += (user) =>
            {
                StartCoroutine(LoadAllPlayerDataCoroutine(user));
            };
        }
        public void Login()
        {
            Activate("loading");

            AuthModel.LoginUser(
                inputEmail.text,
                inputPassword.text,
                onSuccess: (user) =>
                {
                    Debug.Log($"Welcome, {user.Email}!");

                    Disactivate("loading");

                    StartCoroutine(LoadAllPlayerDataCoroutine(user));
                },
                onError: (errorMsg) =>
                {
                    Disactivate("loading");

                    ErrorView errorView = (ErrorView)GetView("error");
                    errorView.ErrorSetup("Failed to login!", errorMsg);

                    errorView.Show();
                }
            );
        }

        public IEnumerator LoadAllPlayerDataCoroutine(FirebaseUser user)
        {
            LoadingController loadingController = Controller.Get<LoadingController>();
            Direct("loading");

            const float MIN_DELAY_PER_ITEM = 0.2f;
            int loadedCount = 0;
            int totalItemsToLoad = 3;

            loadingController.SetLoadingText("Preparing to load player data...");

            bool userDataLoaded = false;
            bool statsLoaded = false;
            bool inventoryDataLoaded = false;


            IEnumerator UpdateProgressAndCheckLobby()
            {
                loadedCount++;
                int progress = (int)((float)loadedCount / totalItemsToLoad * 100);
                loadingController.SetLoadingProgress(progress);

                yield return new WaitForSeconds(MIN_DELAY_PER_ITEM);

                if (userDataLoaded && statsLoaded && inventoryDataLoaded)
                {
                    loadingController.SetLoadingText("Data loaded successfully!");
                    loadingController.SetLoadingProgress(100);

                    yield return new WaitForSeconds(0.5f);
                    Direct("lobby");
                }
            }


            // 1. Load UserData
            loadingController.SetLoadingText("Loading User Profile Data...");

            // Memastikan proses pemuatan selesai sebelum melanjutkan ke item berikutnya
            yield return StartCoroutine(WaitForUserData(user,
                () => { userDataLoaded = true; },
                (err) => { Debug.LogWarning(err); FirestoreModel.InitializeUserData(user); userDataLoaded = true; }
            ));

            // Update progress dan cek lobby setelah selesai
            yield return StartCoroutine(UpdateProgressAndCheckLobby());


            // 2. Load PlayerStats
            loadingController.SetLoadingText("Loading Player Statistics...");

            yield return StartCoroutine(WaitForPlayerStats(user,
                () => { statsLoaded = true; },
                (err) => { Debug.LogWarning(err); FirestoreModel.InitializePlayerStats(user); statsLoaded = true; }
            ));

            yield return StartCoroutine(UpdateProgressAndCheckLobby());


            // 3. Load InventoryData
            loadingController.SetLoadingText("Loading Player Inventory...");

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

    }
}