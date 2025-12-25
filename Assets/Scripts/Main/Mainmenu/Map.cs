using UnityEngine;
using Firebase;
using Firebase.Extensions;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class Map: MonoBehaviour
    {
        public string worldId;

        public string mapId;
        public string mapName;
        public string mapDescription;
        public string mapSceneName;

        [Space(10)]
        public Vector2 containerShowPosition;
        public Sprite mapPreview;
        public Sprite achievementPreview;

        [Space(10)]
        public Button clickButton;
    }
}