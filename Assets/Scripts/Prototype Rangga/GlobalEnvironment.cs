using System.Linq;
using UnityEngine;

public class GlobalEnvironment : MonoBehaviour
{
    public static GlobalEnvironment instance;

     public TargetPoint[] targetPoints;

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
        targetPoints = FindObjectsOfType<TargetPoint>()
            .OrderBy(t => t.targetIndex)
            .ToArray();
    }
}
