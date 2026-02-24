using Firebase.Firestore;
using System.Collections.Generic;


namespace Main.Mainmenu
{
    [FirestoreData]
    public class PlayerStats
    {
        [FirestoreProperty] public int ArradiusDollar { get; set; }
        [FirestoreProperty] public int Experience { get; set; }
        [FirestoreProperty] public int Level { get; set; }
        [FirestoreProperty] public float PlayerSkillRating { get; set; }
        [FirestoreProperty] public int DailyStreak { get; set; }
        [FirestoreProperty] public int LastClaimedDay { get; set; }
        [FirestoreProperty] public List<string> MapUnlocked { get; set; } = new();
        [FirestoreProperty] public string Rank { get; set; }

        [FirestoreProperty] public Timestamp LastDailyClaim { get; set; }


        // Untuk 'Shopaholic'
        [FirestoreProperty] public int TotalItemsPurchased { get; set; }

        // Untuk 'Rookie Champion', 'Pro Sprinter', 'Radiant Legend'
        // Bisa berupa List string berisi ID Map yang sudah dimenangkan (bukan sekadar unlock)
        [FirestoreProperty] public List<string> MapsWon { get; set; } = new();

        // Untuk 'Boing Boing!'
        [FirestoreProperty] public int TotalJumps { get; set; }

        // Untuk 'Whale Spending'
        // Meskipun bisa dicek saat belanja, jika ingin menyimpan record belanja terbesar:
        [FirestoreProperty] public int HighestSingleTransaction { get; set; }

        // Untuk 'The Unstoppable'
        [FirestoreProperty] public int NeverOvertakenWinStreak { get; set; }
    }
}