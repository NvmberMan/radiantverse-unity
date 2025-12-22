using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Wajib untuk sorting list

public class RacePositionSystem : MonoBehaviour
{
    [Header("References")]
    public Transform finishLine; // Objek garis finish
    public Transform playerTransform; // Objek player kita

    // Masukkan semua bot dan player ke sini di Inspector
    public List<Transform> allRacers;

    private void Update()
    {
        if (GameManager.Instance.isGameActive == false) return;

        // 1. Urutkan list berdasarkan jarak terdekat ke finish line
        // (Ascending: Jarak kecil = Posisi 1)
        allRacers.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a.position, finishLine.position);
            float distB = Vector3.Distance(b.position, finishLine.position);
            return distA.CompareTo(distB);
        });

        // 2. Cari di urutan ke berapa Player berada sekarang
        int playerRank = allRacers.IndexOf(playerTransform) + 1;

        // 3. Update UI lewat GameManager
        if (GameManager.Instance.rankUIText != null)
        {
            GameManager.Instance.rankUIText.text = $"Pos: {playerRank}/{allRacers.Count}";
        }
    }
}