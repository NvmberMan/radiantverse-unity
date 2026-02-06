using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.LightTransport;

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
                    //Debug.Log($"{collection} successfully saved!");
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
                    //Debug.Log($"PlayerStats updated!");
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
                    //Debug.Log($"UserData updated!");
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
        public static void InitializeUserData(FirebaseUser user, Action<UserData> onSuccess = null)
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
                {
                    Debug.Log("User data initialized in Firestore!");

                    if(onSuccess != null)
                        onSuccess?.Invoke(userData);
                }
            });
        }

        public static void InitializePlayerStats(FirebaseUser user, Action<PlayerStats> onSuccess = null)
        {
            DocumentReference docRef = db.Collection("playerStats").Document(user.UserId);

            PlayerStats data = new PlayerStats
            {
                ArradiusDollar = 0,
                Experience = 0,
                Level = 1,
                MapUnlocked = new List<string> {
                    "world-001__map-001"
                },
                DailyStreak = 0,
                Rank = "Rookie",
            };

            docRef.SetAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("InventoryData created in Firestore!");

                    if(onSuccess != null)
                        onSuccess?.Invoke(data);
                }
                else
                    Debug.LogError($"Failed to save data: {task.Exception}");
            });
        }

        public static void InitializeInventoryData(FirebaseUser user, Action<InventoryData> onSuccess = null)
        {
            DocumentReference docRef = db.Collection("inventoryData").Document(user.UserId);

            InventoryData data = new InventoryData
            {
                Gender = 0,
                UnlockedAccessories = new List<string> {
                    "faces/Boy",
                    "hairs/Ivy league",
                    "pants/Sporty-short-blue",
                    "shirts/Sporty-blue",
                    "shoes/Sporty-aqua",
                    "socks/long-white-sock",
                    "faces/Girl",
                    "hairs/ponytail with bangs",
                    "pants/Sporty-short-pink",
                    "shirts/Sporty pink",
                    "shoes/Pink sport"
                },
                SelectedSkins = new List<string> {
                    "faces/Boy",
                    "hairs/Ivy league",
                    "pants/Sporty-short-blue",
                    "shirts/Sporty-blue",
                    "shoes/Sporty-aqua",
                    "socks/long-white-sock"
                },
                UnlockedAchievements = new List<string>()
            };

            docRef.SetAsync(data).ContinueWithOnMainThread(task => {
                if (task.IsCompletedSuccessfully) 
                {
                    Debug.Log("Inventory Initialized!");

                    if (onSuccess != null)
                            onSuccess?.Invoke(data);
                }
            });
        }



        public static void CheckUsernameExists(string username, Action<bool> onResult)
        {
            db.Collection("users").WhereEqualTo("Username", username).GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    QuerySnapshot snapshot = task.Result;

                    bool exists = snapshot.Count > 0;

                    onResult?.Invoke(exists);
                }
                else
                {
                    Debug.LogError("Gagal mengecek username: " + task.Exception);
                    onResult?.Invoke(false);
                }
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

        public static void UnlockAchievement(string achievementId)
        {
            if (PlayerLocalData.inventoryData.UnlockedAchievements.Contains(achievementId)) return;

            PlayerLocalData.inventoryData.UnlockedAchievements.Add(achievementId);

            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("inventoryData").Document(uid);

            docRef.UpdateAsync("UnlockedAchievements", FieldValue.ArrayUnion(achievementId))
                .ContinueWithOnMainThread(task => {
                    if (task.IsCompletedSuccessfully) Debug.Log($"Cloud Updated: {achievementId}");
                });
        }

        public static void UnlockMap(string mapId)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            DocumentReference docRef = db.Collection("playerStats").Document(uid);

            docRef.UpdateAsync("MapUnlocked", FieldValue.ArrayUnion(mapId))
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (!PlayerLocalData.playerStats.MapUnlocked.Contains(mapId))
                    {
                        PlayerLocalData.playerStats.MapUnlocked.Add(mapId);
                    }
                }
                else
                {
                    Debug.LogError($"Gagal membuka map: {task.Exception}");
                }
            });
        }

        public static void RecordMapWin(string mapId)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            if (!PlayerLocalData.playerStats.MapsWon.Contains(mapId))
            {
                PlayerLocalData.playerStats.MapsWon.Add(mapId);
                db.Collection("playerStats").Document(uid)
                  .UpdateAsync("MapsWon", FieldValue.ArrayUnion(mapId));
            }
        }

        public static void SetGender(int gender)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            PlayerLocalData.inventoryData.Gender = gender;
            db.Collection("inventoryData").Document(uid)
                .UpdateAsync("Gender", gender).ContinueWithOnMainThread(task => {
                    if (task.IsFaulted)
                    {
                        //Debug.LogError("Gagal update: " + task.Exception);
                    }
                    else
                    {
                        //Debug.Log("Update berhasil!");
                    }
                });
        }

        public static void AddExperience(int expGained)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            var stats = PlayerLocalData.playerStats;

            stats.Experience += expGained;

            while (stats.Level < ExpManager.instance.expList.Count)
            {
                int maxExpForCurrentLevel = ExpManager.instance.expList[stats.Level-1].Max;

                if (stats.Experience >= maxExpForCurrentLevel)
                {
                    stats.Experience -= maxExpForCurrentLevel;
                    stats.Level++;
                    Debug.Log($"Level Up! Sekarang Level: {stats.Level}");
                }
                else
                {
                    break; 
                }
            }

            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Experience", stats.Experience },
                { "Level", stats.Level }
            };

            db.Collection("playerStats").Document(uid)
            .UpdateAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    Debug.Log("Experience & Level synced to Cloud!");
                else
                    Debug.LogError($"Sync EXP failed: {task.Exception}");
            });
        }

        public static void AddArradiusDollar(int amount)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            PlayerLocalData.playerStats.ArradiusDollar += amount;

            DocumentReference docRef = db.Collection("playerStats").Document(uid);

            docRef.UpdateAsync("ArradiusDollar", FieldValue.Increment(amount))
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"Berhasil menambah {amount} ArradiusDollar ke Cloud!");
                }
                else
                {
                    Debug.LogError($"Gagal menambah ArradiusDollar: {task.Exception}");
                }
            });
        }

        public static void IncrementPlayerStat(string fieldName, int amount)
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            if (string.IsNullOrEmpty(uid)) return;

            // 1. Update Local Data (menggunakan Reflection agar fleksibel)
            if (PlayerLocalData.playerStats != null)
            {
                // Mencari property berdasarkan nama string (fieldName)
                PropertyInfo prop = typeof(PlayerStats).GetProperty(fieldName);

                if (prop != null && prop.PropertyType == typeof(int))
                {
                    int currentValue = (int)prop.GetValue(PlayerLocalData.playerStats);
                    prop.SetValue(PlayerLocalData.playerStats, currentValue + amount);
                    // Debug.Log($"Local Data Updated: {fieldName} is now {prop.GetValue(PlayerLocalData.playerStats)}");
                }
                else
                {
                    Debug.LogError($"Property '{fieldName}' tidak ditemukan di PlayerStats atau bukan tipe int!");
                    return; // Keluar jika property tidak valid agar tidak error di Cloud
                }
            }

            // 2. Update Cloud Data
            db.Collection("playerStats").Document(uid)
                .UpdateAsync(fieldName, FieldValue.Increment(amount))
                .ContinueWithOnMainThread(task => {
                    if (task.IsCompletedSuccessfully)
                    {
                        // Debug.Log($"Cloud Data Incremented: {fieldName} +{amount}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to increment {fieldName} in Cloud: {task.Exception}");
                    }
                });
        }
    }


}