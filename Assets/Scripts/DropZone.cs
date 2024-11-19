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

    private int startRows = 0;
    private int currentRows = 0;

    private int startCols = 0;

    private RectTransform rect;
    private GridLayoutGroup grid;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();
        childCount = transform.childCount;

        if(expand == Expand.Horizontal)
        {
            //todo: doesn't account for spacing
            float totalWidth = rect.sizeDelta.x;
            totalWidth -= (grid.padding.left + grid.padding.right);

            startCols = (int)(totalWidth / grid.cellSize.x);
        }
        else if(expand == Expand.Vertical)
        {
            startRows = (transform.childCount / maxColumns) + 1;
            currentRows = startRows;
        }
    }

    private void Update()
    {
        //child added or removed
        if(transform.childCount != childCount)
        {
            if(expand == Expand.Vertical)
            {
                int newRows = (transform.childCount / maxColumns) + 1;

                if(currentRows != newRows)
                {
                    if(newRows > currentRows && newRows <= maxRows)
                    {
                        //expand
                        Resize();
                    }
                    else if(newRows < currentRows && newRows >= startRows)
                    {
                        //collapse
                        Resize();
                    }

                    currentRows = newRows;
                }
            }
            else if (expand == Expand.Horizontal)
            {
                if(transform.childCount < childCount && transform.childCount + 2 > startCols)
                {
                    //collapse
                    Resize();
                }
                else if(transform.childCount > childCount && transform.childCount < maxColumns && transform.childCount >= startCols)
                {
                    //expand
                    Resize();
                } 
            }

            childCount = transform.childCount;
        }
    }

    private void Resize()
    {
        if(expand == Expand.Vertical)
        {
            int numY = (transform.childCount / maxColumns) + 1;

            float newHeight = grid.cellSize.y * numY;
            newHeight += grid.padding.top + grid.padding.bottom;
            newHeight += grid.spacing.y * (numY - 1);

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
        } 
        else if(expand == Expand.Horizontal)
        {
            int numX = transform.childCount + 1;

            float newWidth = grid.cellSize.x * numX;
            newWidth += grid.padding.left + grid.padding.right;
            newWidth += grid.spacing.x * (numX - 1);

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
