using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    public int columns;
    public int rows;

    private int childCount;
    private int extraRows = 0;

    private RectTransform rect;
    private GridLayoutGroup grid;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();
        childCount = transform.childCount;
    }

    private void Update()
    {
        //child added or removed
        if(transform.childCount != childCount)
        {
            childCount = transform.childCount;
            int targetRows = (childCount - 1) / columns;

            if(targetRows != extraRows)
            {
                //resize
                Resize(targetRows);
                extraRows = targetRows;
            }
        }
    }

    private void Resize(int numRows)
    {
        numRows++;
        float newHeight = grid.cellSize.y * numRows;
        newHeight += grid.padding.top + grid.padding.bottom;
        newHeight += grid.spacing.y * (numRows - 1);

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
    }
}
