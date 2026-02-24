using Firebase.Firestore;

namespace Main.Mainmenu
{
    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty] public string Uid { get; set; }
        [FirestoreProperty] public string Email { get; set; }
        [FirestoreProperty] public string Username { get; set; }
        [FirestoreProperty] public Timestamp LastLogin { get; set; }
    }
}