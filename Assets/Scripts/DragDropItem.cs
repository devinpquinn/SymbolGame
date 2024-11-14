using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDropItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private Transform originalParent;
    private Coroutine returnCoroutine;
    private float returnSpeed = 50f;

    private RectTransform shadow;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        shadow = transform.Find("Shadow").GetComponent<RectTransform>();
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

        //size
        transform.localScale = Vector2.one * 1.2f;

        //rotation
        float randomRot = Random.Range(4f, 8f) * (Random.Range(0, 2) * 2 - 1);
        transform.localEulerAngles = new Vector3(0, 0, randomRot);

        //shadow
        shadow.anchoredPosition = new Vector2(10, -10);

        //tag
        gameObject.tag = "Held";

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

        //raycast
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent.GetComponent<RectTransform>());
        dummy.transform.parent = canvas.transform;

        Vector2 originalPosition = dummy.GetComponent<RectTransform>().anchoredPosition;

        Destroy(dummy);

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

        transform.SetParent(originalParent);

        //size
        transform.localScale = Vector2.one;

        //rotation
        transform.localEulerAngles = Vector3.zero;

        //shadow
        shadow.anchoredPosition = new Vector2(5, -5);

        //tag
        gameObject.tag = "Draggable";

        DragDropManager.instance.isReturning = false;
    }
}
