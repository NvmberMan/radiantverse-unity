using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UI;

namespace Main.Mainmenu {
    public class LevelSelectorController : Controller
    {
        public List<World> worldList = new List<World>();



        public override void Activate(string targetView)
        {
            base.Activate(targetView);

            List<string> mapUnlocked = PlayerLocalData.playerStats.MapUnlocked;

            foreach (World world in worldList)
            {
                foreach (Map map in world.worldMap)
                {
                    map.worldId = world.worldId;

                    bool unlocked = false;

                    foreach (string mapWorld in mapUnlocked)
                    {
                        string worldId = mapWorld.Split("__")[0];
                        string mapId = mapWorld.Split("__")[1];

                        if (worldId == world.worldId && mapId == map.mapId)
                        {
                            unlocked = true;
                            break;
                        }
                    }

                    map.unlockedObj.SetActive(unlocked);
                    map.lockedObj.SetActive(!unlocked);

                    map.clickButton.onClick.RemoveAllListeners();
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


            List<string> mapUnlocked = PlayerLocalData.playerStats.MapUnlocked;

            bool unlocked = false;
            foreach(string mapWorld in mapUnlocked)
            {
                string worldId = mapWorld.Split("__")[0];
                string mapId = mapWorld.Split("__")[1];

                Debug.Log($"{worldId}   {mapId}");

                if (worldId == world.worldId)
                {
                    if(mapId == targetMap.mapId)
                    {
                        unlocked = true;
                    }
                }
            }

            if (unlocked)
            {
                MapDetailView mapDetail = (MapDetailView)GetView("map detail");
                mapDetail.Show();
                mapDetail.Setup(targetMap);

                AudioManager.Instance.PlaySFX("button click");
            }
            else
            {
                ErrorView errorView = MenuManager.instance.GetController<UniversalController>().GetView<ErrorView>();
                errorView.ErrorSetup("Map is locked", "");
                errorView.Show();
            }

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
        public GameObject unlockedObj;
        public GameObject lockedObj;
    }
}