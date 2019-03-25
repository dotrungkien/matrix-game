using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{

    private Vector2 startingPosition;
    private List<Transform> touchingTiles;
    private Transform myParent;
    private SpriteRenderer render;
    private int topVal, midVal, botVal;

    public Sprite[] renders;
    public SpriteRenderer top;
    public SpriteRenderer mid;
    public SpriteRenderer bot;

    void Start()
    {
        startingPosition = transform.position;
        touchingTiles = new List<Transform>();
        myParent = transform.parent;
    }

    public void SetVal(int[] val)
    {
        topVal = val[0];
        top.sprite = renders[topVal - 7];
        midVal = val[1];
        mid.sprite = renders[midVal - 7];
        botVal = val[2];
        bot.sprite = renders[botVal - 7];
        render = GetComponent<SpriteRenderer>();
        render.color = (transform.tag == Constants.MOVABLE_TAG) ? new Color(0f, 0.6f, 0f) : new Color(0f, 0f, 0.6f);
    }

    public void PickUp()
    {
        if (transform.tag != Constants.MOVABLE_TAG) return;
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        render.sortingOrder = 2;
        top.sortingOrder = 3;
        mid.sortingOrder = 3;
        bot.sortingOrder = 3;
    }

    public void Drop()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        render.sortingOrder = 0;
        top.sortingOrder = 1;
        mid.sortingOrder = 1;
        bot.sortingOrder = 1;
        Vector2 newPosition;
        if (touchingTiles.Count == 0)
        {
            Debug.Log("no touching tiles found");
            transform.position = startingPosition;
            transform.parent = myParent;
            return;
        }

        var currentCell = touchingTiles[0];
        if (touchingTiles.Count == 1)
        {
            newPosition = currentCell.position;
        }
        else
        {
            var distance = Vector2.Distance(transform.position, touchingTiles[0].position);

            foreach (Transform cell in touchingTiles)
            {
                if (Vector2.Distance(transform.position, cell.position) < distance)
                {
                    currentCell = cell;
                    distance = Vector2.Distance(transform.position, cell.position);
                }
            }
            newPosition = currentCell.position;
        }
        // if (!currentCell.CompareTag(transform.tag)) return;
        Debug.Log("current cell " + currentCell.name);
        if (currentCell.childCount != 0)
        {
            Debug.Log("back to start");
            transform.position = startingPosition;
            transform.parent = myParent;
            return;
        }
        else
        {
            Debug.Log("place in slot");
            transform.parent = currentCell;
            StartCoroutine(SlotIntoPlace(transform.position, newPosition));
        }
        transform.tag = Constants.UNTAGGED;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Cell") return;
        if (other.transform.parent.parent.tag != Constants.PLACEABLE_TAG) return;
        Debug.Log("check touching tile contain");
        if (!touchingTiles.Contains(other.transform))
        {
            Debug.Log("contained");
            touchingTiles.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Cell") return;
        if (other.transform.parent.parent.tag != Constants.PLACEABLE_TAG) return;
        if (touchingTiles.Contains(other.transform))
        {
            touchingTiles.Remove(other.transform);
        }
    }

    IEnumerator SlotIntoPlace(Vector2 startingPos, Vector2 endingPos)
    {
        float duration = 0.1f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startingPos, endingPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endingPos;
        // GridManager gridManager = transform.parent.parent.parent.GetComponent<GridManager>();
        // int[] cell = gridManager.PosToGrid(endingPos);
        // Debug.Log("Place on x= " + cell[0] + ", y= " + cell[1]);
        // GameManager.GetInstance().SpawnNewTile();
        // gridManager.UpdateGridVal(cell[0], cell[1], topVal, midVal, botVal);
    }
}
