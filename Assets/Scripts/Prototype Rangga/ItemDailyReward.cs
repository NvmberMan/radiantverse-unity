using Firebase.Auth;
using Main.Mainmenu;
using System.Collections.Generic;
using UnityEngine;

public class ItemDailyReward : MonoBehaviour
{
    public GameObject GraphicReady, GraphicUnReady, GraphicClaimed;

    public void OnReady()
    {
        GraphicReady.SetActive(true);
        GraphicUnReady.SetActive(false);
        GraphicClaimed.SetActive(false);
    }

    public void OnUnReady()
    {
        GraphicReady.SetActive(false);
        GraphicUnReady.SetActive(true);
        GraphicClaimed.SetActive(false);
    }

    public void OnClaim()
    {
        GraphicReady.SetActive(false);
        GraphicUnReady.SetActive(false);
        GraphicClaimed.SetActive(true);
    }

    public virtual void ClaimReward()
    {
        OnClaim();

        FirestoreModel.ClaimDailyReward();
    }
}
