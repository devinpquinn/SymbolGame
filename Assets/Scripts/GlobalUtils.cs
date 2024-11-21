using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalUtils
{
    public static int ChildCountMinusDummy(Transform parent)
    {
        int childCount = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (!parent.GetChild(i).gameObject.name.Contains("Dummy"))
            {
                childCount++;
            }
        }
        return childCount;
    }
}
