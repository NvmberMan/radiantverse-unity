using UnityEngine;
using Firebase;
using Firebase.Extensions;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class Map: MonoBehaviour
    {
        [HideInInspector] public string worldId;

        public string mapId;
        public string mapName;
        public string mapDescription;
        public string mapSceneName;

        [Space(10)]
        public Vector2 containerShowPosition;
        public Sprite mapPreview;

        [Space(10)]
        public Button clickButton;
    }
}