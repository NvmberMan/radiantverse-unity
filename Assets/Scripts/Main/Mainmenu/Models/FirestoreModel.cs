using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Mainmenu
{
    public static class FirestoreModel
    {
        private static FirebaseFirestore db => FirebaseFirestore.DefaultInstance;

        #region Save full document
        public static void SaveUserData(FirebaseUser user, UserData data)
        {
            SaveFullDocument("users", user.UserId, data);
        }

        public static void SavePlayerStats(FirebaseUser user, PlayerStats stats)
        {
            SaveFullDocument("playerStats", user.UserId, stats);
        }
        public static void SaveInventoryData(FirebaseUser user, InventoryData data)
        {
            SaveFullDocument("inventoryData", user.UserId, data);
        }

        private static void SaveFullDocument(string collection, string uid, object data)
        {
            db.Collection(collection).Document(uid)
            .SetAsync(data, SetOptions.Overwrite)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"{collection} successfully saved!");
                }
                else
                {
                    Debug.LogError($"Failed to save {collection}: {task.Exception}");
                }
            });
        }
        #endregion

        #region Save spesific document
        public static void SavePlayerStats(string uid, Dictionary<string, object> updates)
        {
            db.Collection("playerStats").Document(uid)
            .UpdateAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"PlayerStats updated!");
                }
                else
                {
                    Debug.LogError($"Update PlayerStats failed: {task.Exception}");
                }
            });
        }

        public static void SaveUserData(string uid, Dictionary<string, object> updates)
        {
            db.Collection("users").Document(uid)
            .UpdateAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"UserData updated!");
                }
                else
                {
                    Debug.LogError($"Update UserData failed: {task.Exception}");
                }
            });
        }

        public static void SaveInventoryData(string uid, Dictionary<string, object> updates)
        {
            db.Collection("inventoryData").Document(uid)
            .UpdateAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"InventoryData updated!");
                }
                else
                {
                    Debug.LogError($"Update InventoryData failed: {task.Exception}");
                }
            });
        }
        #endregion

        #region Getter document
        public static void GetUserData(FirebaseUser user, Action<UserData> onSuccess, Action<string> onError)
        {
            DocumentReference docRef = db.Collection("users").Document(user.UserId);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        UserData data = snapshot.ConvertTo<UserData>();
                        onSuccess?.Invoke(data);
                    }
                    else
                    {
                        onError?.Invoke("User data not found!");
                    }
                }
                else
                {
                    onError?.Invoke(task.Exception.Message);
                }
            });
        }

        public static void GetPlayerStats(FirebaseUser user, Action<PlayerStats> onSuccess, Action<string> onError)
        {
            DocumentReference docRef = db.Collection("playerStats").Document(user.UserId);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        PlayerStats data = snapshot.ConvertTo<PlayerStats>();
                        onSuccess?.Invoke(data);
                    }
                    else
                    {
                        onError?.Invoke("PlayerStats not found!");
                    }
                }
                else
                {
                    onError?.Invoke(task.Exception.Message);
                }
            });
        }

        public static void GetInventoryData(FirebaseUser user, Action<InventoryData> onSuccess, Action<string> onError)
        {
            DocumentReference docRef = db.Collection("inventoryData").Document(user.UserId);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        InventoryData data = snapshot.ConvertTo<InventoryData>();
                        onSuccess?.Invoke(data);
                    }
                    else
                    {
                        onError?.Invoke("InventoryData not found!");
                    }
                }
                else
                {
                    onError?.Invoke(task.Exception.Message);
                }
            });
        }


        #endregion

        #region Initialize Data
        public static void InitializeUserData(FirebaseUser user)
        {
            DocumentReference docRef = db.Collection("users").Document(user.UserId);

            UserData userData = new UserData
            {
                Uid = user.UserId,
                Email = user.Email,
                Username = "New Player",
                LastLogin = Timestamp.GetCurrentTimestamp()
            };

            docRef.SetAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    Debug.Log("User data created in Firestore!");
                else
                    Debug.LogError($"Failed to save data: {task.Exception}");
            });
        }

        public static void InitializePlayerStats(FirebaseUser user)
        {
            DocumentReference docRef = db.Collection("playerStats").Document(user.UserId);

            PlayerStats data = new PlayerStats
            {
                ArradiusDollar = 0,
                Experience = 0,
                Level = 0,
                Rank = "Rookie"
            };

            docRef.SetAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    Debug.Log("InventoryData created in Firestore!");
                else
                    Debug.LogError($"Failed to save data: {task.Exception}");
            });
        }

        //public static void InitializeInventoryData(FirebaseUser user)
        //{
        //    DocumentReference docRef = db.Collection("inventoryData").Document(user.UserId);

        //    InventoryData data = new InventoryData
        //    {
        //        UnlockedAchievements = new List<string>()
        //    };

        //    docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        //    {
        //        if (task.IsCompletedSuccessfully)
        //            Debug.Log("InventoryData created in Firestore!");
        //        else
        //            Debug.LogError($"Failed to save data: {task.Exception}");
        //    });
        //}

        public static void InitializeInventoryData(FirebaseUser user)
        {
            DocumentReference docRef = db.Collection("inventoryData").Document(user.UserId);

            InventoryData data = new InventoryData
            {
                UnlockedAccessories = new List<string> { "body/body_default", "head/head_default" }, // Skin awal
                SelectedSkins = new List<string> { "body/body_default", "head/head_default" },
                UnlockedAchievements = new List<string>()
            };

            docRef.SetAsync(data).ContinueWithOnMainThread(task => {
                if (task.IsCompletedSuccessfully) Debug.Log("Inventory Initialized!");
            });
        }
        #endregion

        public static void UpdateSelectedSkins(string uid, List<string> newSkins)
        {
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "SelectedSkins", newSkins }
            };
            SaveInventoryData(uid, updates);
        }
    }
}