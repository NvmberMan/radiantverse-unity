using Firebase.Auth;
using TMPro;
using UnityEngine;

public class LobbyController : Controller
{
    [Header("Controller Variables")]
    [SerializeField] private TMP_Text arradiusDollarView;

    public override void Activate(string targetView)
    {
        base.Activate(targetView);
        // Langsung pakai data lokal (instant)
        if (PlayerLocalData.IsPlayerStatsLoaded)
        {
            arradiusDollarView.text = "Arradius Dollar: " + PlayerLocalData.playerStats.ArradiusDollar.ToString();
        }

        // Kalau mau ambil versi terbaru dari cloud
        RefreshPlayerStats();
    }

    private void RefreshPlayerStats()
    {
        var user = AuthManager.instance.CurrentUser;

        FirestoreModel.GetPlayerStats(user,
            onSuccess: (stats) =>
            {
                PlayerLocalData.playerStats = stats;   //update local
                arradiusDollarView.text = "Arradius Dollar: " + stats.ArradiusDollar.ToString();
            },
            onError: Debug.LogError
        );
    }

}
