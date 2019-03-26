using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;

    private bool draggingItem = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;

    void Update()
    {
        gameManager = GetComponent<GameManager>();
        if (gameManager.isGameOver) return;
        if (Input.GetMouseButton(0))
        {
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
            if (tile.tag == Constants.MOVABLE_TAG) draggedObject.transform.position = inputPosition + touchOffset;
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
            draggedObject.transform.GetComponent<Tile>().PickUp();
        }
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
