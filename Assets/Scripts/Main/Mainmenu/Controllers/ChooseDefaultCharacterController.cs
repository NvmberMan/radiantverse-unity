using Firebase.Auth;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Mainmenu
{
    public class ChooseDefaultCharacterController : Controller
    {
        [HideInInspector] public List<string> defaultBoySkin = new()
        {
            "faces/Boy",
            "hairs/Ivy league",
            "pants/Sporty-short-blue",
            "shirts/Sporty-blue",
            "shoes/Sporty-aqua",
            "socks/long-white-sock"
        };

        [HideInInspector] public List<string> defaultGirlSkin = new()
        {
            "faces/Girl",
            "hairs/ponytail with bangs",
            "pants/Sporty-short-pink",
            "shirts/Sporty pink",
            "shoes/Pink sport",
            "socks/long-white-sock"
        };

        public void SelectGender(bool isMan)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            FirestoreModel.SetGender(isMan ? 0 : 1);

            if (isMan)
            {
                PlayerLocalData.inventoryData.SelectedSkins = defaultBoySkin;
                FirestoreModel.UpdateSelectedSkins(uid, PlayerLocalData.inventoryData.SelectedSkins);
            }
            else
            {
                PlayerLocalData.inventoryData.SelectedSkins = defaultGirlSkin;
                FirestoreModel.UpdateSelectedSkins(uid, PlayerLocalData.inventoryData.SelectedSkins);
            }

            Direct("lobby");
        }
    }
}