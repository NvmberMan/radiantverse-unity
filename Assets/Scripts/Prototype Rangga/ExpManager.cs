using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    public static ExpManager instance;

    public List<Exp> expList = new List<Exp>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}

[System.Serializable]
public class Exp
{
    public int Min = 0;
    public int Max = 0;
}

