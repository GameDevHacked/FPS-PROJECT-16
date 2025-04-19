using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraTouchInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Touch Settings")]
    public Vector2 touchDelta;
    public bool pressed;
    public RectTransform touchPanel;

    private Vector2 pointerOld;
    private int pointerId;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(touchPanel, eventData.position))
        {
            pressed = true;
            pointerId = eventData.pointerId;
            pointerOld = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pointerId == eventData.pointerId)
        {
            pressed = false;
            touchDelta = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pressed && pointerId == eventData.pointerId)
        {
            touchDelta = eventData.position - pointerOld;
            pointerOld = eventData.position;
        }
    }
}