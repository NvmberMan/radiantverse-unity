using System.Collections;
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
        [SerializeField] private Image mapPreview;

        public void Setup(Map map)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = map.containerShowPosition;

            mapNameText.text = map.mapName;
            mapDescriptionText.text = map.mapDescription;
            mapPreview.sprite = map.mapPreview;

            startButton.onClick.AddListener(() =>
            {
                StartCoroutine(InitializeMapPreview(map));
            });
        }

        IEnumerator InitializeMapPreview(Map map)
        {
            LoadingMapPreviewController loadingMapPreviewController = MenuManager.instance.GetController<LoadingMapPreviewController>();
            loadingMapPreviewController.Activate("base");
            loadingMapPreviewController.SetLoading(map.mapName, map.mapDescription, map.mapPreview, 20);

            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(map.mapSceneName);
        }
    }
}