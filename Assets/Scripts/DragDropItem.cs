using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDropItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isBeingDragged = false;
    private bool isReturning = false; // Flag to check if the item is returning to the original position
    private Vector2 originalPosition;
    private CanvasGroup canvasGroup;

    private Coroutine returnCoroutine;

    public float returnSpeed = 500f; // Speed at which the item moves back (units per second)

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>(); // To control transparency when dragging
        originalPosition = rectTransform.anchoredPosition;
    }

    // Called when the mouse pointer enters the item's area
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isReturning) return; // Prevent interaction if the item is returning

        // Change cursor to indicate the item can be picked up
        Cursor.SetCursor(Texture2D.whiteTexture, Vector2.zero, CursorMode.Auto);
    }

    // Called when the mouse pointer exits the item's area
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isReturning) return; // Prevent interaction if the item is returning

        // Revert cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Called when the mouse button is clicked down on the item
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isReturning) return; // Prevent interaction if the item is returning

        isBeingDragged = true;
        originalPosition = rectTransform.anchoredPosition;

        // Make the object slightly transparent when picked up (optional)
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
        }
    }

    // Called while the mouse is dragging the item
    public void OnDrag(PointerEventData eventData)
    {
        if (isReturning) return; // Prevent interaction if the item is returning

        if (isBeingDragged)
        {
            // Move the item with the mouse position
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Account for canvas scaling
        }
    }

    // Called when the mouse button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isReturning) return; // Prevent interaction if the item is returning

        isBeingDragged = false;

        // Start the smooth return process
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine); // Stop any previous return if the mouse is clicked again before it finishes
        }
        returnCoroutine = StartCoroutine(ReturnToOriginalPosition());

        // Restore the alpha value if you used it
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        // Revert cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Coroutine to smoothly return the item to its original position at a set speed
    private IEnumerator ReturnToOriginalPosition()
    {
        isReturning = true; // Set the flag to true when starting the return

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 direction = originalPosition - startPos; // Direction from current to original position
        float distance = direction.magnitude; // The distance to travel
        float timeToReturn = distance / returnSpeed; // Calculate time based on distance and speed

        float timeElapsed = 0f;

        // Keep moving the item towards its original position until it reaches the destination
        while (timeElapsed < timeToReturn)
        {
            float t = timeElapsed / timeToReturn; // Normalize time between 0 and 1
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t); // Interpolate position

            timeElapsed += Time.deltaTime; // Increment time
            yield return null; // Wait for the next frame
        }

        // Ensure that the position is exactly the original position when done
        rectTransform.anchoredPosition = originalPosition;

        isReturning = false; // Set the flag to false when the return is complete
    }
}
