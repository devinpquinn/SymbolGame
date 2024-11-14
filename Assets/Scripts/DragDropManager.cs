using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    public DragDropItem current;

    public bool isDragging = false;
    public bool isHovering = false;
    public bool isReturning = false;

    private void Awake()
    {
        instance = this;
    }
}
