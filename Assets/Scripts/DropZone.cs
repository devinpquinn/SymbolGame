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

        if(grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            //flex vertical
        }
        else
        {
            //flex horizontal
        }
    }

    private void Resize(Vector2 to)
    {
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
