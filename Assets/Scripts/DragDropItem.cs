using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDropItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private Transform originalParent;
    private Coroutine returnCoroutine;
    private float returnSpeed = 50f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragDropManager.instance.isReturning)
        {
            return;
        }

        //cursor
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragDropManager.instance.isReturning)
        {
            return;
        }

        //cursor
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (DragDropManager.instance.isReturning || DragDropManager.instance.current != null)
        {
            return;
        }

        //pick up
        DragDropManager.instance.current = this;

        //parent
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);

        DragDropManager.instance.isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragDropManager.instance.isReturning)
        {
            return;
        }

        if (DragDropManager.instance.isDragging)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (DragDropManager.instance.isReturning)
        {
            return;
        }

        //release
        if (DragDropManager.instance.current == this)
        {
            DragDropManager.instance.current = null;
        }

        DragDropManager.instance.isDragging = false;

        //raycast to detect layout group

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        returnCoroutine = StartCoroutine(ReturnToOriginalPosition());
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        DragDropManager.instance.isReturning = true;

        Vector2 startPos = rectTransform.anchoredPosition;

        //instantiate dummy child of original parent
        GameObject dummy = Instantiate(gameObject, originalParent);

        Vector2 originalPosition = dummy.GetComponent<RectTransform>().anchoredPosition;

        Vector2 direction = originalPosition - startPos;
        float distance = direction.magnitude;
        float timeToReturn = distance / (returnSpeed * 100);

        Destroy(dummy);

        float timeElapsed = 0f;

        while (timeElapsed < timeToReturn)
        {
            float t = timeElapsed / timeToReturn;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.SetParent(originalParent);

        DragDropManager.instance.isReturning = false;
    }
}
