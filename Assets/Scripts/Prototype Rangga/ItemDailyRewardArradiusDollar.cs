using Firebase.Auth;
using Main.Mainmenu;
using System.Collections.Generic;
using UnityEngine;

public class ItemDailyRewardArradiusDollar : ItemDailyReward
{
    public ParticleAttractor ArradiusDollarAttractor;
    public int Amount = 20;
    public override void ClaimReward()
    {
        base.ClaimReward();

        PlayerLocalData.playerStats.ArradiusDollar += Amount;

        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirestoreModel.SavePlayerStats(uid, new Dictionary<string, object>
        {
            { "ArradiusDollar", PlayerLocalData.playerStats.ArradiusDollar }
        });

        LobbyView lobbyView = MenuManager.instance.GetController<LobbyController>().GetView<LobbyView>();

        Instantiate(ArradiusDollarAttractor, transform.parent.parent.transform).target = lobbyView.arradiusDollarCircleTargetAtractor.transform;

        lobbyView.RefreshPlayerStats();

        AudioManager.Instance.PlaySFX("arradius dollar");
    }
}
