using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;

    private int childCount;
    private int rowsCount;

    public int columns;
    public int rows;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        childCount = transform.childCount;

        rowsCount = Mathf.CeilToInt((float)transform.childCount / columns);
        if(rowsCount < 1)
        {
            rowsCount = 1;
        }
    }

    void Update()
    {
        if(transform.childCount != childCount)
        {
            int numRows = Mathf.CeilToInt((float)transform.childCount / columns);
            if(numRows != rowsCount)
            {
                UpdateImageSize(numRows);
                rowsCount = numRows;
            }
            
            childCount = transform.childCount;
        }
    }

    void UpdateImageSize(int numRows)
    {
        Debug.Log(numRows);

        // Calculate the new height based on the number of rows
        //float newHeight = calcRows * gridLayoutGroup.cellSize.y + (rows - 1) * gridLayoutGroup.spacing.y;

        // Update the Image RectTransform height
        //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
    }
}
