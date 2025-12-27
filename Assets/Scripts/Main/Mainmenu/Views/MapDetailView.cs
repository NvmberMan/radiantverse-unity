using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class MapDetailView : View
    {
        [SerializeField] private TMP_Text mapNameText;
        [SerializeField] private TMP_Text mapDescriptionText;
        [SerializeField] private Button startButton;

        public void Setup(Map map)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = map.containerShowPosition;

            mapNameText.text = map.mapName;
            mapDescriptionText.text = map.mapDescription;

            startButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(map.mapSceneName);
            });
        }
    }
}