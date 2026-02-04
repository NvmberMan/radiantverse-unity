using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu {
    public class LevelSelectorController : Controller
    {
        public List<World> worldList = new List<World>();

        private void Start()
        {
            foreach (World world in worldList)
            {
                foreach(Map map in world.worldMap)
                {
                    map.worldId = world.worldId;

                    map.clickButton.onClick.AddListener(() =>
                    {
                        ShowMapDetail(map);
                    });
                }
            }
        }
        public void ShowMapDetail(Map map)
        {
            World world = worldList.Find((t) => t.worldId == map.worldId);
            Map targetMap = world.worldMap.Find((t) => t.mapId == map.mapId);

            MapDetailView mapDetail = (MapDetailView)GetView("map detail");
            mapDetail.Show();
            mapDetail.Setup(targetMap);
        }
    }

    [System.Serializable]
    public class World
    {
        public string worldId;
        public string worldName;
        public GameObject worldObject;
        public List<Map> worldMap = new List<Map>();
    }

    [System.Serializable]
    public class Map
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