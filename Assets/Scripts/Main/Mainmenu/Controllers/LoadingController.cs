using UnityEngine;
using TMPro;

namespace Main.Mainmenu
{
    public class LoadingController : Controller
    {
        [Header("Loading View Elements")]
        [SerializeField] private TMP_Text loadingTextView;
        [SerializeField] private TMP_Text progressTextView;

        private const string DefaultLoadingMessage = "Loading...";

        public override void Activate(string targetView)
        {
            base.Activate(targetView);

            SetLoadingText(DefaultLoadingMessage);
            SetLoadingProgress(0);
        }

        /// <summary>
        /// Mengatur teks yang ditampilkan di layar loading.
        /// </summary>
        /// <param name="message">Pesan loading yang akan ditampilkan.</param>
        public void SetLoadingText(string message)
        {
            if (loadingTextView != null)
            {
                loadingTextView.text = message;
            }
            else
            {
                Debug.LogWarning("Loading text view is not assigned in the Inspector!");
            }
        }

        /// <summary>
        /// Mengatur nilai progress (persentase) yang ditampilkan.
        /// </summary>
        /// <param name="progress">Nilai progress dari 0 hingga 100.</param>
        public void SetLoadingProgress(int progress)
        {
            int clampedProgress = Mathf.Clamp(progress, 0, 100);

            if (progressTextView != null)
            {
                progressTextView.text = $"{clampedProgress}%";
            }
        }
    }
}