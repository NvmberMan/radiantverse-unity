using UnityEngine;
using UnityEngine.Playables; // Wajib untuk Timeline
using UnityEngine.Events;    // Wajib untuk Custom Event

public class CutsceneHandler : MonoBehaviour
{
    [Header("Timeline Reference")]
    public PlayableDirector director;

    [Header("Settings")]
    public bool playOnStart = true;

    [Header("Events")]
    public UnityEvent onCutsceneStart;
    public UnityEvent onCutsceneEnd;

    private void Start()
    {
        // Cari komponen Director jika belum diisi
        if (director == null)
            director = GetComponent<PlayableDirector>();

        // Langsung mainkan jika disetting auto-play
        if (playOnStart)
        {
            StartCutscene();
        }

        // Subscribe ke event "stopped" milik Timeline
        // Ini akan terpanggil otomatis saat durasi timeline habis
        director.stopped += OnTimelineStopped;
    }

    public void StartCutscene()
    {
        if (director != null)
        {
            // Matikan input player (Logic Dummy kita tadi)
            // GameManager.Instance.isGameActive = false; (Nanti di-uncomment kalau GameManager ada di scene ini)

            Debug.Log("🎬 Cutscene Dimulai...");
            onCutsceneStart.Invoke();
            director.Play();
        }
    }

    // Fungsi ini dipanggil otomatis oleh Unity saat Timeline selesai
    private void OnTimelineStopped(PlayableDirector obj)
    {
        Debug.Log("🎬 Cutscene Selesai!");

        // Hidupkan kembali input player
        // GameManager.Instance.isGameActive = true;

        onCutsceneEnd.Invoke();
    }

    // Penting: Hapus subscription saat script mati untuk mencegah memory leak
    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnTimelineStopped;
    }
}   