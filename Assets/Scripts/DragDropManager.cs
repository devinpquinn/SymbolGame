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

    public Transform currentHome;
    public int currentIndex = -1;

    private void Awake()
    {
        instance = this;
    }
}
