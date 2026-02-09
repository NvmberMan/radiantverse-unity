using Main.Gameplay;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnvironment : MonoBehaviour
{
    public static GlobalEnvironment instance;

    public List<Way> ways = new List<Way>();

    public List<AIData> aiData = new List<AIData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShuffleAIData()
    {
        if (aiData == null || aiData.Count == 0) return;

        for (int i = 0; i < aiData.Count; i++)
        {
            AIData temp = aiData[i];
            int randomIndex = Random.Range(i, aiData.Count);
            aiData[i] = aiData[randomIndex];
            aiData[randomIndex] = temp;
        }
    }

    public void RefreshTargetPoints()
    {
        //targetPoints = FindObjectsOfType<TargetPoint>()
        //    .OrderBy(t => t.targetIndex)
        //    .ToArray();
    }
}


[System.Serializable]
public class Way
{
    public TargetPoint[] targetPoints;
}
