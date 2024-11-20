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
        if(expand == Expand.Fixed)
        {
            return;
        }

        //child added or removed
        int minusDummy = ChildrenMinusDummy();
        if(minusDummy != childCount)
        {
            if(expand == Expand.Vertical)
            {
                int newRows = (minusDummy / maxColumns) + 1;

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
                if(minusDummy < childCount && minusDummy + 2 > startCols)
                {
                    //collapse
                    Resize();
                }
                else if(minusDummy > childCount && minusDummy < maxColumns && minusDummy >= startCols)
                {
                    //expand
                    Resize();
                } 
            }

            childCount = minusDummy;
        }
    }

    private void Resize()
    {
        int minusDummy = ChildrenMinusDummy();

        if(expand == Expand.Vertical)
        {
            int numY = (minusDummy / maxColumns) + 1;

            float newHeight = grid.cellSize.y * numY;
            newHeight += grid.padding.top + grid.padding.bottom;
            newHeight += grid.spacing.y * (numY - 1);

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
        } 
        else if(expand == Expand.Horizontal)
        {
            int numX = minusDummy + 1;

            float newWidth = grid.cellSize.x * numX;
            newWidth += grid.padding.left + grid.padding.right;
            newWidth += grid.spacing.x * (numX - 1);

            rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);
        }
    }

    private int ChildrenMinusDummy()
    {
        int children = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.CompareTag("Dummy"))
            {
                children++;
            }
        }
        return children;
    }
}

public enum Expand
{
    Fixed = 1,
    Vertical = 2,
    Horizontal = 3
}
