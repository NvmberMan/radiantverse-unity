using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class LoadingMapPreviewController : Controller
    {
        [Header("Loading View Elements")]
        [SerializeField] private TMP_Text mapTitleText;
        [SerializeField] private TMP_Text mapWorldText;
        [SerializeField] private Image mapPreviewImage;
        [SerializeField] private Slider loadingSlider;
        public override void Activate(string targetView)
        {
            base.Activate(targetView);
        }

        /// <summary>
        /// Mengatur nilai progress (persentase) yang ditampilkan.
        /// </summary>
        /// <param name="progress">Nilai progress dari 0 hingga 100.</param>
        public void SetLoading(string mapTitle, string mapWorld, Sprite mapPreview, int progress)
        {
            mapTitleText.text = mapTitle;
            mapWorldText.text = mapWorld;
            mapPreviewImage.sprite = mapPreview;
            loadingSlider.value = progress;

            int clampedProgress = Mathf.Clamp(progress, 0, 100);

            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }
        }

        public void SetLoadingProgress(int progress)
        {
            int clampedProgress = Mathf.Clamp(progress, 0, 100);

            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }
        }
    }
}