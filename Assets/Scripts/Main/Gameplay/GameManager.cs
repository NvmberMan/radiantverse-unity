using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Untuk UI
using System.Collections.Generic; // Untuk List

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game States")]
    public bool isGameActive = true;
    public int totalRacers = 8; // Default 8 pelari
    public int currentRank = 1; // Urutan finish saat ini

    [Header("UI References")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TextMeshProUGUI rankUIText; // UI di pojok layar (Contoh: "Pos: 1/8")
    public TextMeshProUGUI finalRankText; // Teks di panel kalah (Contoh: "You finished #3")

    // Mencegah satu orang finish berkali-kali
    private List<GameObject> finishedRacers = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Pastikan waktu berjalan
        Time.timeScale = 1;
        currentRank = 1;
        finishedRacers.Clear();
    }

    public void OnFinishLineCrossed(GameObject racer)
    {
        // 1. Cek apakah racer ini sudah pernah finish sebelumnya?
        if (finishedRacers.Contains(racer)) return;

        // 2. Masukkan ke daftar finish
        finishedRacers.Add(racer);
        Debug.Log($"🏁 {racer.name} Finish di posisi #{currentRank}");

        // 3. Cek Siapa yang Finish
        if (racer.CompareTag("Player"))
        {
            // --- PLAYER FINISH ---
            HandlePlayerFinish(currentRank);
        }
        else if (racer.CompareTag("NPC"))
        {
            // --- BOT FINISH ---
            // Game TIDAK berhenti, cuma ranking bertambah
            // Opsional: Matikan AI bot biar dia diam setelah finish
            // racer.GetComponent<AI_Script>().enabled = false; 
            currentRank++;
        }
    }

    private void HandlePlayerFinish(int rank)
    {
        isGameActive = false;
        Time.timeScale = 0; // Game berhenti HANYA jika player finish

        if (rank == 1)
        {
            // JUARA 1 = MENANG
            Debug.Log("✨ PLAYER JUARA 1!");

            // Reward: Unlock Level, Gold, XP
            PlayerPrefs.SetInt("Level2_Unlocked", 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 100);
            PlayerPrefs.SetInt("XP", PlayerPrefs.GetInt("XP") + 500);
            PlayerPrefs.Save();

            if (winPanel != null) winPanel.SetActive(true);
        }
        else
        {
            // JUARA 2 DST = KALAH (Tapi tetap dapat XP)
            Debug.Log($"💀 PLAYER KALAH (Posisi {rank})");

            // Reward: XP Only (Penghibur)
            PlayerPrefs.SetInt("XP", PlayerPrefs.GetInt("XP") + 100);
            PlayerPrefs.Save();

            if (finalRankText != null)
                finalRankText.text = $"You Finished #{rank}";

            if (losePanel != null) losePanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Debug.Log("🔄 Restarting Game...");

        // Penting: Kembalikan waktu ke normal sebelum reload
        Time.timeScale = 1;

        // Reload Scene saat ini
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}