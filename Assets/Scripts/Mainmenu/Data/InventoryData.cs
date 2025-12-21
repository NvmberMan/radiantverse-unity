using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class InventoryData
{
    [FirestoreProperty] public List<string> UnlockedAchievements { get; set; } = new();
    [FirestoreProperty] public List<string> OwnedCharacters { get; set; } = new();
}
