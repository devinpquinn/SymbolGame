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

    private Vector3 crookedRot = Vector3.zero;

    private Transform dummy = null;

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

        //create dummy
        dummy = Instantiate(gameObject, originalParent).transform;
        dummy.localScale = Vector3.one;
        dummy.transform.localEulerAngles = Vector3.zero;

        //prune dummy
        Destroy(dummy.GetComponent<DragDropItem>());
        for (int i = 0; i < dummy.childCount; i++)
        {
            Destroy(dummy.GetChild(i).gameObject);
        }

        //tag dummy
        dummy.gameObject.name = gameObject.name + " Dummy";
        dummy.gameObject.tag = "Dummy";

        //set home
        DragDropManager.instance.currentHome = originalParent;

        //size
        transform.localScale = Vector2.one * 1.2f;

        //rotation
        Crooked();

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
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        GraphicRaycaster graphicRaycaster = GetComponentInParent<GraphicRaycaster>();

        if (graphicRaycaster != null)
        {
            var results = new System.Collections.Generic.List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                bool hovering = false;
                if (IsOnLayer(result.gameObject, "DragDrop") && result.gameObject.CompareTag("Drop Zone"))
                {
                    hovering = true;
                }

                if(hovering != DragDropManager.instance.isHovering)
                {
                    if (hovering)
                    {
                        //start item hover
                        Straighten();

                        //set home
                        DragDropManager.instance.currentHome = result.gameObject.transform;

                    }
                    else
                    {
                        //end item hover
                        Crooked();

                        //set home
                        DragDropManager.instance.currentHome = originalParent;
                    }

                    dummy.SetParent(DragDropManager.instance.currentHome);

                    DragDropManager.instance.isHovering = hovering;
                }

                if (hovering)
                {
                    return;
                }
            }
        }
    }

    private void Straighten()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    private void Crooked()
    {
        if(crookedRot == Vector3.zero)
        {
            float randomRot = Random.Range(4f, 8f) * (Random.Range(0, 2) * 2 - 1);
            crookedRot = new Vector3(0, 0, randomRot);

        }

        transform.localEulerAngles = crookedRot;
    }

    private bool IsOnLayer(GameObject obj, string layerName)
    {
        int layerMask = LayerMask.NameToLayer(layerName);
        return obj.layer == layerMask;
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

        //clear crooked rot
        crookedRot = Vector3.zero;

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

        //find home
        Transform myHome = DragDropManager.instance.currentHome;
        DragDropManager.instance.currentHome = null;

        //get dummy
        LayoutRebuilder.ForceRebuildLayoutImmediate(myHome.GetComponent<RectTransform>());
        dummy.transform.parent = canvas.transform;

        Vector2 originalPosition = dummy.GetComponent<RectTransform>().anchoredPosition;

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

        Destroy(dummy.gameObject);

        transform.SetParent(myHome);

        //size
        transform.localScale = Vector2.one;

        //rotation
        Straighten();

        //shadow
        shadow.anchoredPosition = new Vector2(5, -5);

        //tag
        gameObject.tag = "Draggable";

        DragDropManager.instance.isReturning = false;
    }
}
