using System.Collections.Generic;
using UnityEngine;

public class GlobalEnvironment : MonoBehaviour
{
    public static GlobalEnvironment instance;

    public List<Way> ways = new List<Way>();
     

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
