﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private GameController gameController;

    private bool draggingItem = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;

    void Update()
    {
        gameController = GetComponent<GameController>();
        if (gameController.isGameOver) return;
        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id)) return;
        }
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            DragOrPickUp();
        }
        else
        {
            if (draggingItem) DropItem();
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
            if (tile.tag == Constants.MOVABLE_TAG)
            {
                draggedObject.transform.position = inputPosition + (new Vector3(0, 1, 1)) * touchOffset;
            }
        }
        else
        {
            // var layerMask = 1 << 0;
            // RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.5f, layerMask);
            // if (touches.Length > 0)
            // {
            //     var hit = touches[0];
            //     if (hit.transform != null && hit.transform.tag == Constants.MOVABLE_TAG)
            //     {
            //         draggingItem = true;
            //         draggedObject = hit.transform.gameObject;
            //         draggedObject = GameObject.FindGameObjectWithTag(Constants.MOVABLE_TAG);
            //         touchOffset = (Vector2)hit.transform.position - inputPosition;
            //         hit.transform.GetComponent<Tile>().PickUp();
            //     }
            // }

            draggedObject = GameObject.FindGameObjectWithTag(Constants.MOVABLE_TAG);
            if (draggedObject == null) return;
            draggingItem = true;
            touchOffset = (Vector2)draggedObject.transform.position - inputPosition;
            draggedObject.transform.position = inputPosition + (new Vector3(0, 1, 1)) * touchOffset;
            draggedObject.transform.GetComponent<Tile>().PickUp();
        }
    }

    public void CancelDrag()
    {
        draggingItem = false;
    }

    void DropItem()
    {
        draggingItem = false;
        draggedObject.transform.localScale = new Vector3(1, 1, 1);
        if (draggedObject.tag == Constants.MOVABLE_TAG)
        {
            draggedObject.GetComponent<Tile>().Drop();
        }
    }
}
