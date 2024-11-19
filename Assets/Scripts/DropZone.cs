using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    public Expand expand;

    public int maxColumns;
    public int maxRows;

    private int childCount;
    private int extraRows = 0;
    private int extraCols = 0;

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
            if(expand == Expand.Vertical)
            {
                int addRows = (transform.childCount + 1) / maxColumns;

                if (addRows != extraRows && addRows < maxRows && addRows >= 0)
                {
                    //resize
                    Resize();
                    extraRows = addRows;
                }
            }
            else if (expand == Expand.Horizontal)
            {
                if(transform.childCount < childCount && extraCols > 0)
                {
                    //shrink
                    Resize();
                    extraCols--;
                }
                else if(transform.childCount > childCount && transform.childCount < maxColumns)
                {
                    //grow
                    Resize();
                    extraCols++;
                } 
            }

            childCount = transform.childCount;
        }
    }

    private void Resize()
    {
        if(expand == Expand.Vertical)
        {
            int numItems = (transform.childCount - 1) / maxColumns;
            float newHeight = grid.cellSize.y * numItems;
            newHeight += grid.padding.top + grid.padding.bottom;
            newHeight += grid.spacing.y * (numItems - 1);

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
        } 
        else if(expand == Expand.Horizontal)
        {
            int numItems = transform.childCount + 1;

            float newWidth = grid.cellSize.x * numItems;
            newWidth += grid.padding.left + grid.padding.right;
            newWidth += grid.spacing.x * (numItems - 1);

            rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);
        }
    }
}

public enum Expand
{
    Fixed = 1,
    Vertical = 2,
    Horizontal = 3
}
