using UnityEngine;
using TMPro; // Wajib: Pastikan Project sudah install TextMeshPro
using System.Collections.Generic;
using System.Linq; // Untuk rumus matematika P50/P90
using UnityEngine.SceneManagement;

public class RemoteTestManager : MonoBehaviour
{
    public static RemoteTestManager Instance;

    [Header("UI References (Drag & Drop disini)")]
    public GameObject reportPanel;      // Panel background hitam
    public TextMeshProUGUI reportText;  // Tempat tulisan laporan muncul

    [Header("Settings")]
    public bool autoFindPlayer = true;  // Cari player otomatis saat mulai?

    // --- DATA PENYIMPANAN ---
    private List<float> latencyHistory = new List<float>();
    private float totalFPS = 0f;
    private int frameCount = 0;
    private float minFPS = 999f;

    private int stuckCount = 0;
    private int deathCount = 0;
    private int pauseCount = 0;
    private bool hasReachedFinish = false;
    private bool isTestActive = true;
    private float timer = 0f;

    // Stuck Detection
    private Vector3 lastPlayerPos;
    private float timeSinceLastMove = 0f;
    private GameObject playerRef;

    void Awake()
    {
        // Singleton: Agar script ini bisa dipanggil dari mana saja
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Pastikan Panel mati dulu saat awal game
        if (reportPanel) reportPanel.SetActive(false);
    }

    void Start()
    {
        if (autoFindPlayer) FindPlayer();
    }

    void Update()
    {
        if (!isTestActive) return;

        // 1. Timer Global
        timer += Time.unscaledDeltaTime;

        // 2. Hitung FPS & Latency (Setiap Frame)
        float currentDelta = Time.unscaledDeltaTime;
        float currentFPS = 1.0f / currentDelta;

        // Simpan latency dalam milidetik (ms)
        latencyHistory.Add(currentDelta * 1000f);

        totalFPS += currentFPS;
        frameCount++;

        // Catat FPS terendah (Low 1%) setelah 3 detik pertama (skip loading)
        if (timer > 3f && currentFPS < minFPS) minFPS = currentFPS;

        // 3. Deteksi Stuck
        DetectStuck();

        if (Input.GetKeyDown(KeyCode.L))
        {
            ForceFinish();
        }
    }

    // --- FUNGSI UTAMA (LOGIC) ---

    void FindPlayer()
    {
        // Ganti "Player" sesuai Tag karakter kamu jika beda
        playerRef = GameObject.FindGameObjectWithTag("Player");
        if (playerRef) lastPlayerPos = playerRef.transform.position;
    }

    void DetectStuck()
    {
        if (playerRef == null) return;

        float dist = Vector3.Distance(playerRef.transform.position, lastPlayerPos);

        // Jika gerak kurang dari 1 cm dan game tidak sedang di-pause
        if (dist < 0.01f && Time.timeScale > 0)
        {
            timeSinceLastMove += Time.unscaledDeltaTime;
            if (timeSinceLastMove > 1.5f) // Diam > 1.5 detik dianggap stuck
            {
                stuckCount++;
                timeSinceLastMove = 0f; // Reset biar ga spam
            }
        }
        else
        {
            timeSinceLastMove = 0f;
            lastPlayerPos = playerRef.transform.position;
        }
    }

    // --- PUBLIC METHODS (Panggil dari Script Lain) ---

    public void LogPause() { pauseCount++; }

    public void LogDeath() { deathCount++; }

    public void LogFinish()
    {
        hasReachedFinish = true;
        FinishTest("Level Completed");
    }

    public void ForceFinish()
    {
        FinishTest("User Quit / Force Stop");
    }

    // --- GENERATE REPORT ---

    public void FinishTest(string reason)
    {
        if (!isTestActive) return;
        isTestActive = false;

        // Stop Waktu Game
        Time.timeScale = 0;

        // Hitung Rata-rata
        float avgFPS = totalFPS / frameCount;

        // Hitung Percentile Latency (P50, P90, P95)
        latencyHistory.Sort();
        int count = latencyHistory.Count;
        float p50 = count > 0 ? latencyHistory[(int)(count * 0.50f)] : 0;
        float p90 = count > 0 ? latencyHistory[(int)(count * 0.90f)] : 0;
        float p95 = count > 0 ? latencyHistory[(int)(count * 0.95f)] : 0;

        // Susun Laporan Teks
        string report = $"<b>REPORT HASIL TESTING</b>\n" +
                        $"----------------------------------------\n" +
                        $"Device: {SystemInfo.deviceModel}\n" +
                        $"Durasi: {timer:F0}s | Status: {reason}\n\n" +

                        $"<b>[MULTIMEDIA] Performance:</b>\n" +
                        $"• Avg FPS : <color={(avgFPS >= 30 ? "green" : "red")}>{avgFPS:F1}</color>\n" +
                        $"• Min FPS : {minFPS:F1}\n\n" +

                        $"<b>[MACHINE LEARNING] AI Latency:</b>\n" +
                        $"• P50 (Avg) : {p50:F2} ms\n" +
                        $"• P90 (High): {p90:F2} ms\n" +
                        $"• P95 (Max) : {p95:F2} ms\n\n" +

                        $"<b>[SOFTWARE ENG] Behavior Log:</b>\n" +
                        $"• Player Stuck Count : {stuckCount} kali\n" +
                        $"• Bot Death Count : {deathCount} kali\n" +
                        $"• Pause Menu  : {pauseCount} kali\n" +
                        $"• Player Finished: {(hasReachedFinish ? "<color=green>YES</color>" : "<color=red>NO</color>")}";

        // Tampilkan ke Layar
        if (reportText) reportText.text = report;
        if (reportPanel) reportPanel.SetActive(true);

        // Munculkan Mouse Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Triggered"); // Cek di Console editor
        Application.Quit();

        // Baris ini biar tombolnya jalan juga pas kita test di Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}