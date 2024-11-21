using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    public int capacity;

    private RectTransform rect;
    private GridLayoutGroup grid;

    private Coroutine resizeOp = null;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();
    }

    public void CheckResize()
    {
        //check child count and determine if resize is needed
        if(grid.constraint == GridLayoutGroup.Constraint.Flexible)
        {
            return;
        }

        if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            //flex vertical
            int desiredRows = 1 + (transform.childCount / grid.constraintCount);
            if(desiredRows > (capacity / grid.constraintCount))
            {
                return;
            }

            float width = rect.sizeDelta.x;
            float height = desiredRows * grid.cellSize.y;
            height += grid.padding.top + grid.padding.bottom;

            Resize(new Vector2(width, height));
        }
        else
        {
            //flex horizontal
            int desiredColumns = 1 + transform.childCount;
            if(desiredColumns > capacity)
            {
                return;
            }

            float width = desiredColumns * grid.cellSize.x;
            width += grid.padding.left + grid.padding.right;
            float height = rect.sizeDelta.y;

            Resize(new Vector2(width, height));
        }
    }

    private void Resize(Vector2 to)
    {
        if(rect.sizeDelta == to)
        {
            return;
        }

        if(resizeOp != null)
        {
            StopCoroutine(resizeOp);
        }

        StartCoroutine(DoResize(to));
    }

    IEnumerator DoResize(Vector2 to)
    {
        float timeElapsed = 0f;
        float duration = 0.05f;

        Vector2 from = rect.sizeDelta;

        while (timeElapsed < duration)
        {
            rect.sizeDelta = Vector2.Lerp(from, to, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final size is exactly the target size
        rect.sizeDelta = to;
    }
}
