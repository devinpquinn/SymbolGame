using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    public DragDropItem current;

    private void Awake()
    {
        instance = this;
    }
}
