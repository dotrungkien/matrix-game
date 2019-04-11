using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{


    private List<Transform> touchingTiles;
    private Transform myParent;
    private SpriteRenderer render;
    private int topVal, midVal, botVal;
    [HideInInspector]
    public Vector2 startingPosition;
    public GameObject frame;
    public Sprite[] renders;
    public SpriteRenderer top;
    public SpriteRenderer mid;
    public SpriteRenderer bot;

    void Awake()
    {
        touchingTiles = new List<Transform>();
        myParent = transform.parent;
        render = GetComponent<SpriteRenderer>();
        DisableFrame();
    }

    public void SetVal(int[] val)
    {
        topVal = val[0];
        top.sprite = renders[topVal];
        midVal = val[1];
        mid.sprite = renders[midVal];
        botVal = val[2];
        bot.sprite = renders[botVal];
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void DisableFrame()
    {
        frame.SetActive(false);
    }

    public void EnableFrame()
    {
        frame.SetActive(true);
    }

    public void PickUp()
    {
        if (transform.tag != Constants.MOVABLE_TAG) return;
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        render.sortingOrder = 2;
        top.sortingOrder = 3;
        mid.sortingOrder = 3;
        bot.sortingOrder = 3;
    }

    public void Drop()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        render.sortingOrder = -1;
        top.sortingOrder = 0;
        mid.sortingOrder = 0;
        bot.sortingOrder = 0;
        Vector2 newPosition;
        if (touchingTiles.Count == 0)
        {
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
        if (currentCell.childCount != 0)
        {
            transform.position = startingPosition;
            transform.parent = myParent;
            return;
        }
        else
        {
            transform.parent = currentCell;
            render.color = currentCell.parent.parent.GetComponent<GridBase>().color;
            // render.enabled = false;
            StartCoroutine(SlotIntoPlace(transform.position, newPosition));
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Cell") return;
        if (other.transform.parent.parent.tag != Constants.PLACEABLE_TAG) return;
        if (!touchingTiles.Contains(other.transform))
        {
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
        // SoundManager.GetInstance().MakeTileSound();
        transform.position = endingPos;

        GridBase gridBase = transform.parent.parent.parent.GetComponent<GridBase>();
        gridBase.UpdateLastTile(this);
        int[] cell = gridBase.PosToGrid(endingPos);
        gridBase.UpdateGridVal(cell[0], cell[1], topVal, midVal, botVal);
        cell[1] = (8 - cell[1]) / 3;
        EventManager.GetInstance().PostNotification(EVENT_TYPE.PLACE_PIECE, this, cell);
        transform.tag = Constants.UNTAGGED;

    }
}
