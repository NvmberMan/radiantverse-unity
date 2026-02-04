using Firebase.Firestore;


namespace Main.Mainmenu
{
    [FirestoreData]
    public class PlayerStats
    {
        [FirestoreProperty] public int ArradiusDollar { get; set; }
        [FirestoreProperty] public int Experience { get; set; }
        [FirestoreProperty] public int Level { get; set; }
        [FirestoreProperty] public int DailyStreak { get; set; }
        [FirestoreProperty] public string Rank { get; set; }
    }
}