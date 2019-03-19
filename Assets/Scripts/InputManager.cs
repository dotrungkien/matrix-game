using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    private bool draggingItem = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;


    void Start()
    {
        GameManager.GetInstance().SpawnNewTile();
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DragOrPickUp();
        }
        else
        {
            if (draggingItem)
                DropItem();
        }
    }

    Vector2 CurrentTouchPosition
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;
        if (draggingItem)
        {
            var tile = draggedObject.GetComponent<Tile>();
            if (!tile) return;
            if (tile.movable) draggedObject.transform.position = inputPosition + touchOffset;
        }
        else
        {
            var layerMask = 1 << 0;
            RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.5f, layerMask);
            if (touches.Length > 0)
            {
                var hit = touches[0];
                if (hit.transform != null && hit.transform.tag == "Tile")
                {
                    draggingItem = true;
                    draggedObject = hit.transform.gameObject;
                    touchOffset = (Vector2)hit.transform.position - inputPosition;
                    hit.transform.GetComponent<Tile>().PickUp();
                }
            }
        }
    }

    void DropItem()
    {
        draggingItem = false;
        draggedObject.transform.localScale = new Vector3(1, 1, 1);
        if (draggedObject.GetComponent<Tile>().movable)
        {
            draggedObject.GetComponent<Tile>().Drop();
        }
    }
}
