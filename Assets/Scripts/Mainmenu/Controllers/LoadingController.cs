using UnityEngine;
using TMPro;

public class LoadingController : Controller
{
    [Header("Loading View Elements")]
    [SerializeField] private TMP_Text loadingTextView;
    [SerializeField] private TMP_Text progressTextView;
    // Anda bisa mengganti progressTextView dengan Slider jika ingin tampilan bar:
    // [SerializeField] private UnityEngine.UI.Slider progressBar; 

    private const string DefaultLoadingMessage = "Loading...";

    // --- Public Methods for Customization ---

    /// <summary>
    /// Menampilkan LoadingController dan mengatur teks loading awal.
    /// </summary>
    public override void Show()
    {
        base.Show();
        // Atur teks default saat pertama kali ditampilkan
        SetLoadingText(DefaultLoadingMessage);
        SetLoadingProgress(0); // Mulai dari 0%
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
        // Pastikan progress berada dalam batas 0-100
        int clampedProgress = Mathf.Clamp(progress, 0, 100);

        // Update Text
        if (progressTextView != null)
        {
            progressTextView.text = $"{clampedProgress}%";
        }
        // Update Slider (jika Anda menggunakannya)
        // if (progressBar != null)
        // {
        //     progressBar.value = clampedProgress / 100f;
        // }
    }
}