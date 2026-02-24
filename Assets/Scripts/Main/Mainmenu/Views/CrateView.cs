using Firebase.Auth;
using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class CrateView : View
    {
        public Image previewImage;
        [HideInInspector] public Animator animator;

        [Header("Gacha Settings")]
        public int tapsRequired = 3;
        private int currentTapCount = 0;
        private CrateData currentCrateData;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Init(CrateData crateData)
        {
            currentCrateData = crateData;
            previewImage.sprite = crateData.iconNoBackground;
            currentTapCount = 0;
        }

        public void OnCrateTapped()
        {
            currentTapCount++;

            if (currentTapCount < tapsRequired)
            {
                if (animator != null) animator.SetTrigger("Shake");

                AudioManager.Instance.PlaySFX("shake");
            }
            else
            {
                ExecuteGacha();
                currentTapCount = 0; 
            }
        }

        private void ExecuteGacha()
        {
            if (currentCrateData == null) return;

            AccessoryData rolledItem = currentCrateData.RollGacha();
            Debug.Log($"Gacha Result: {rolledItem.displayName}");

            if (animator != null) animator.SetTrigger("Open");

            Hide();
            MenuManager.instance.GetController<UniversalController>().ShowGetItemPopup(rolledItem);
            GrantAccessory(rolledItem);
            UpdateDatabaseSync();
        }

        private void GrantAccessory(AccessoryData accessory)
        {
            var inventory = PlayerLocalData.inventoryData;
            if (!inventory.UnlockedAccessories.Contains(accessory.spineSkinName))
            {
                inventory.UnlockedAccessories.Add(accessory.spineSkinName);
            }
        }
        private void UpdateDatabaseSync()
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            var statsUpdates = new Dictionary<string, object> { { "ArradiusDollar", PlayerLocalData.playerStats.ArradiusDollar } };
            FirestoreModel.SavePlayerStats(uid, statsUpdates);

            var invUpdates = new Dictionary<string, object> { { "UnlockedAccessories", PlayerLocalData.inventoryData.UnlockedAccessories } };
            FirestoreModel.SaveInventoryData(uid, invUpdates);
        }

    }
}