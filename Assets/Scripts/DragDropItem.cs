using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDropItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isBeingDragged = false;
    private bool isReturning = false;
    private Vector2 originalPosition;

    private Coroutine returnCoroutine;

    private float returnSpeed = 50f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isReturning)
        {
            return;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isReturning)
        {
            return;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isReturning)
        {
            return;
        }

        isBeingDragged = true;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isReturning)
        {
            return;
        }

        if (isBeingDragged)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isReturning)
        {
            return;
        } 

        isBeingDragged = false;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        returnCoroutine = StartCoroutine(ReturnToOriginalPosition());
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        isReturning = true;

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 direction = originalPosition - startPos;
        float distance = direction.magnitude;
        float timeToReturn = distance / (returnSpeed * 100);

        float timeElapsed = 0f;

        while (timeElapsed < timeToReturn)
        {
            float t = timeElapsed / timeToReturn;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;

        isReturning = false;
    }
}
