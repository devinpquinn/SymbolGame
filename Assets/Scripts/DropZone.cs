using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    [HideInInspector] public int capacity;

    private void Start()
    {
        int numColumns = Mathf.FloorToInt(GetComponent<RectTransform>().sizeDelta.x / GetComponent<GridLayoutGroup>().cellSize.x);
        int numRows = Mathf.FloorToInt(GetComponent<RectTransform>().sizeDelta.y / GetComponent<GridLayoutGroup>().cellSize.y);

        capacity = numRows * numColumns;
    }
}
