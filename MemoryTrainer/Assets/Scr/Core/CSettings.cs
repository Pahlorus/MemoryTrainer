using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSettings
{
    public int xCount = 5;
    public int yCount = 5;
    public float timeDelay = 5;

    public static CSettings Instance;

    public CSettings()
    {
        Instance = this;
    }

    public int GetTileCount()
    {
        return xCount * yCount;
    }
}
