using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Main.Gameplay // Sesuaikan namespace jika diperlukan
{
    public class PlayerSkillManager : MonoBehaviour
    {
        public static PlayerSkillManager Instance;

        [Header("Skill Settings")]
        [Tooltip("Berapa match terakhir yang diingat sistem?")]
        public int matchHistoryLimit = 5;

        [Tooltip("Batas maksimal nabrak/stuck sebelum nilai kebersihan main jadi 0")]
        public int maxTolerableStuck = 5;

        [Tooltip("Nilai awal untuk pemain yang baru pertama kali main (0.0 = Termudah, 1.0 = Tersulit)")]
        public float startingSkill = 0.3f;

        // Struktur data untuk menyimpan riwayat tiap balapan
        private struct MatchData
        {
            public bool isWin;
            public int stuckCount;
        }

        // Antrean (Queue) untuk menyimpan memori 5 balapan terakhir
        private Queue<MatchData> matchHistory = new Queue<MatchData>();

        [Header("Live Data (Hanya untuk dilihat di Inspector)")]
        [SerializeField] private float currentPlayerSkill;

        private void Awake()
        {
            // Setup Singleton & pastikan tidak hancur saat pindah scene/level
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Sangat penting agar memori tidak hilang!
            }
            else
            {
                Destroy(gameObject);
            }

            // Set nilai awal
            currentPlayerSkill = startingSkill;
        }

        // --- PANGGIL FUNGSI INI SETIAP KALI PEMAIN MENCAPAI GARIS FINISH ---
        public void RecordMatchResult(bool isWin, int stuckCount)
        {
            // 1. Catat data balapan yang baru saja selesai
            MatchData newMatch = new MatchData { isWin = isWin, stuckCount = stuckCount };
            matchHistory.Enqueue(newMatch);

            // 2. Jika memori sudah penuh (lebih dari 5), buang ingatan yang paling lama
            if (matchHistory.Count > matchHistoryLimit)
            {
                matchHistory.Dequeue();
            }

            // 3. Hitung ulang skill pemain berdasarkan histori terbaru
            CalculateSkill();
        }

        private void CalculateSkill()
        {
            if (matchHistory.Count == 0) return;

            // A. Hitung Win Ratio (Bobot 70%)
            // Berapa persen pemain menang dari 5 match terakhir?
            int winCount = matchHistory.Count(m => m.isWin);
            float w_Ratio = (float)winCount / matchHistory.Count;

            // B. Hitung Clean Run / Kebersihan Main (Bobot 30%)
            // Semakin jarang nabrak, nilainya semakin mendekati 1.0
            float totalCleanScore = 0f;
            foreach (var match in matchHistory)
            {
                // Jika nabrak >= maxTolerableStuck, nilainya 0. Jika tidak nabrak sama sekali, nilainya 1.
                float cleanScore = Mathf.Max(0f, 1f - ((float)match.stuckCount / maxTolerableStuck));
                totalCleanScore += cleanScore;
            }
            float c_Ratio = totalCleanScore / matchHistory.Count;

            // C. Gabungkan menjadi Final Skill
            currentPlayerSkill = (w_Ratio * 0.7f) + (c_Ratio * 0.3f);

            // Pastikan nilai tidak keluar dari rentang 0.0 sampai 1.0
            currentPlayerSkill = Mathf.Clamp(currentPlayerSkill, 0f, 1f);

            Debug.Log($"[ML SKILL UPDATE] Win Ratio: {w_Ratio:F2} | Clean Run: {c_Ratio:F2} | FINAL BOT DIFFICULTY: {currentPlayerSkill:F2}");
        }

        // Fungsi yang akan dipanggil oleh GameManager untuk mengatur kepintaran bot
        public float GetPlayerSkill()
        {
            if (matchHistory.Count == 0) return startingSkill;
            return currentPlayerSkill;
        }
    }
}