using Firebase.Auth;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Mainmenu
{
    public class ChooseDefaultCharacterController : Controller
    {
        private List<string> defaultBoySkin = new()
        {
            "faces/Boy",
            "hairs/Type1",
            "pants/Sporty-short-blue",
            "shirts/Sporty-blue"
        };

        private List<string> defaultGirlSkin = new()
        {
            "faces/Girl",
            "hairs/Type1",
            "pants/Sporty-short-blue",
            "shirts/Sporty-blue"
        };

        public void SelectGender(bool isMan)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            if(isMan)
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