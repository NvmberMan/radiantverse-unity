using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class DebugManager : MonoBehaviour
{
    public TMP_Text debugText;

    [Header("Settings")]
    public float updateInterval = 0.5f; 

    private float deltaTime = 0.0f;
    private float timer;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = 60;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        timer += Time.unscaledDeltaTime;
        if (timer >= updateInterval)
        {
            DisplayStats();
            timer = 0f; 
        }
    }

    void DisplayStats()
    {
        if (debugText == null) return;

        float fps = 1.0f / deltaTime;

        float msec = deltaTime * 1000.0f;

        long totalMemory = Profiler.GetTotalReservedMemoryLong() / 1048576;

        debugText.text = string.Format("FPS: {0:0.} ({1:0.0} ms)\n", fps, msec) +
                         string.Format("MEM: {0} MB\n", totalMemory) +
                         string.Format("OS: {0}", SystemInfo.operatingSystem);
    }
}