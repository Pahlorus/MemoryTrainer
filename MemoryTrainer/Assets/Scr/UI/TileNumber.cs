using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileNumber : TileBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool _active;
    private Vector2 oldPosition;
    private Vector2 defaultPosition;
    private RectTransform rectTr;

    void Start()
    {
        rectTr = GetComponent<RectTransform>();
    }

    public void SetActive(bool active)
    {
        _active = active;
        gameObject.SetActive(active);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_active) return;
        oldPosition = eventData.position;
        defaultPosition = rectTr.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_active) return;
        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - oldPosition;
        Vector3 newPosition = rectTr.position + new Vector3(diff.x, diff.y, transform.position.z);
        rectTr.position = newPosition;
        oldPosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_active) return;
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (var result in raycastResults)
            {
                if (result.gameObject.layer == 3)
                {
                    var tile = result.gameObject.GetComponent<Tile>();
                    tile.SetTile(Number);
                    SetActive(false);
                    //_uiManager.TileReturnPool(this);
                    return;
                }
            }
        }
        rectTr.position = defaultPosition;
    }
}
