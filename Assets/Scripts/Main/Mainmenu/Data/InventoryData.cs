using Firebase.Firestore;
using System.Collections.Generic;

namespace Main.Mainmenu
{
    [FirestoreData]
    public class InventoryData
    {
        [FirestoreProperty] public List<string> UnlockedAccessories { get; set; } = new();
        [FirestoreProperty] public List<string> SelectedSkins { get; set; } = new();
        [FirestoreProperty] public List<string> UnlockedAchievements { get; set; } = new();
    }
}