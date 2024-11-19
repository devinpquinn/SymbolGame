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
        int dummyIndex = transform.GetSiblingIndex();
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);

        //create dummy
        dummy = Instantiate(gameObject, originalParent).transform;
        dummy.localScale = Vector3.one;
        dummy.transform.localEulerAngles = Vector3.zero;
        dummy.SetSiblingIndex(dummyIndex);

        //prune dummy
        Destroy(dummy.GetComponent<DragDropItem>());
        for (int i = 0; i < dummy.childCount; i++)
        {
            Destroy(dummy.GetChild(i).gameObject);
        }

        //tag dummy
        dummy.gameObject.name = gameObject.name + " Dummy";
        dummy.gameObject.tag = "Dummy";

        //dummy image display for debug
        //Image dummyImage = dummy.gameObject.GetComponent<Image>();
        //dummyImage.color = new Color(0, 0, 0, 0.1f);

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
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current){position = Input.mousePosition};

        GraphicRaycaster graphicRaycaster = GetComponentInParent<GraphicRaycaster>();

        if (graphicRaycaster != null)
        {
            var results = new System.Collections.Generic.List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            //setup storage variables
            bool hovering = false;
            Transform hoverParent = null;
            int hoverIndex = -1;
            bool overDummy = false;

            //iterate through hit results
            foreach (var result in results)
            {
                if (!IsOnLayer(result.gameObject, "DragDrop"))
                {
                    continue;
                }
                
                if (result.gameObject.CompareTag("Drop Zone"))
                {
                    //check if drop zone is full
                    DropZone dz = result.gameObject.GetComponent<DropZone>();
                    if(dz && ChildCountMinusDummy(dz.transform) == (dz.maxRows * dz.maxColumns))
                    {
                        continue;
                    }

                    hovering = true;
                    hoverParent = result.gameObject.transform;
                }
                else if (result.gameObject.CompareTag("Dummy"))
                {
                    overDummy = true;
                }
                else if (result.gameObject.CompareTag("Draggable") && result.gameObject != gameObject)
                {
                    hoverIndex = result.gameObject.transform.GetSiblingIndex();
                }
            }

            //determine hover state
            if (hovering != DragDropManager.instance.isHovering)
            {
                if (hovering)
                {
                    //start item hover
                    Straighten();

                    //set home
                    DragDropManager.instance.currentHome = hoverParent;

                }
                else
                {
                    //end item hover
                    Crooked();

                    //set home
                    DragDropManager.instance.currentHome = originalParent;

                    //set as last sibling
                    dummy.SetAsLastSibling();
                }

                if (dummy)
                {
                    dummy.SetParent(DragDropManager.instance.currentHome);
                }

                DragDropManager.instance.isHovering = hovering;
            }

            //determine if hovering over another item
            if(hovering)
            {
                if(!overDummy && hoverIndex < 0)
                {
                    dummy.SetAsLastSibling();
                }
                else if(hoverIndex >= 0)
                {
                    dummy.SetSiblingIndex(hoverIndex);
                }
                
                DragDropManager.instance.currentIndex = hoverIndex;

                LayoutRebuilder.ForceRebuildLayoutImmediate(DragDropManager.instance.currentHome.GetComponent<RectTransform>());
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
        DragDropManager.instance.currentIndex = -1;

        //setup dummy
        int dummyIndex = dummy.GetSiblingIndex();

        dummy.parent = canvas.transform;
        Vector2 originalPosition = dummy.GetComponent<RectTransform>().anchoredPosition;

        dummy.parent = myHome;
        dummy.SetSiblingIndex(dummyIndex);

        //get vector to dummy position
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

        //place
        transform.SetParent(myHome);
        transform.SetSiblingIndex(dummyIndex);

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

    public int ChildCountMinusDummy(Transform parent)
    {
        int childCount = 0;
        for(int i = 0; i < parent.childCount; i++)
        {
            if (!parent.GetChild(i).gameObject.name.Contains("Dummy"))
            {
                childCount++;
            }
        }
        return childCount;
    }
}
