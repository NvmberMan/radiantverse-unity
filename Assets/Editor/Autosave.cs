using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class Autosave
{
    // Konfigurasi: Edit waktu di sini (dalam menit)
    private static float saveIntervalMinutes = 0.5f;
    private static double nextSaveTime;

    static Autosave()
    {
        // Menentukan waktu penyimpanan pertama kali saat Unity dibuka
        nextSaveTime = EditorApplication.timeSinceStartup + (saveIntervalMinutes * 60);

        // Mendaftarkan fungsi Update ke Editor
        EditorApplication.update += Update;

        Debug.Log($"<color=green>Autosave Aktif:</color> Menyimpan setiap {saveIntervalMinutes} menit.");
    }

    private static void Update()
    {
        // Cek apakah sudah waktunya menyimpan
        if (EditorApplication.timeSinceStartup > nextSaveTime)
        {
            SaveProject();
        }
    }

    private static void SaveProject()
    {
        // Jangan simpan saat sedang Play Mode
        if (EditorApplication.isPlaying) return;

        // Simpan Scene yang sedang terbuka
        EditorSceneManager.SaveOpenScenes();

        // Simpan aset lainnya (Prefabs, Materials, dll)
        AssetDatabase.SaveAssets();

        // Atur ulang timer untuk sesi berikutnya
        nextSaveTime = EditorApplication.timeSinceStartup + (saveIntervalMinutes * 60);

        Debug.Log($"<color=cyan>Autosave:</color> Proyek berhasil disimpan pada {System.DateTime.Now:HH:mm:ss}");
    }
}