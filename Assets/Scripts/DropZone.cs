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
                childCount = transform.childCount;

                int addRows = (childCount - 1) / maxColumns;

                if (addRows != extraRows && addRows < maxRows)
                {
                    //resize
                    Resize(addRows);
                    extraRows = addRows;
                }
            }
            else if (expand == Expand.Horizontal)
            {
                if(transform.childCount < childCount)
                {
                    //shrink
                    Resize(extraRows - 1);
                    extraRows--;
                }
                else
                {
                    //grow
                    Resize(extraRows + 1);
                    extraRows++;
                } 
            }
        }
    }

    private void Resize(int numItems)
    {
        numItems++;

        if(expand == Expand.Vertical)
        {
            float newHeight = grid.cellSize.y * numItems;
            newHeight += grid.padding.top + grid.padding.bottom;
            newHeight += grid.spacing.y * (numItems - 1);

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
        } 
        else if(expand == Expand.Horizontal)
        {
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
