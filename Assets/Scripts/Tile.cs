using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{

    private Vector2 startingPosition;
    private List<Transform> touchingTiles;
    private Transform myParent;
    private SpriteRenderer tile;
    private int topVal, midVal, botVal;
    private GridManager gridManager;

    public bool movable;
    public Sprite[] renders;
    public SpriteRenderer top;
    public SpriteRenderer mid;
    public SpriteRenderer bot;

    public enum TAG
    {
        player1,
        player2
    }
    public TAG tag;

    void Start()
    {
        gridManager = GameObject.Find("Player1").GetComponent<GridManager>();
        movable = true;
        tile = GetComponent<SpriteRenderer>();
        topVal = UnityEngine.Random.Range(7, 10);
        // topVal = 9;
        top.sprite = renders[topVal - 7];
        midVal = UnityEngine.Random.Range(7, 10);
        // midVal = 9;
        mid.sprite = renders[midVal - 7];
        botVal = UnityEngine.Random.Range(7, 10);
        // botVal = 9;
        bot.sprite = renders[botVal - 7];
        startingPosition = transform.position;
        touchingTiles = new List<Transform>();
        myParent = transform.parent;
    }

    public void PickUp()
    {
        if (!movable) return;
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        tile.sortingOrder = 2;
        top.sortingOrder = 3;
        mid.sortingOrder = 3;
        bot.sortingOrder = 3;
    }

    public void Drop()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tile.sortingOrder = 0;
        top.sortingOrder = 1;
        mid.sortingOrder = 1;
        bot.sortingOrder = 1;
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
        // if (!currentCell.CompareTag(transform.tag)) return;
        if (currentCell.childCount != 0)
        {
            transform.position = startingPosition;
            transform.parent = myParent;
            return;
        }
        else
        {
            transform.parent = currentCell;
            StartCoroutine(SlotIntoPlace(transform.position, newPosition));
        }
        movable = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Cell") return;
        if (other.transform.parent.parent.name != transform.tag) return;
        if (!touchingTiles.Contains(other.transform))
        {
            touchingTiles.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Cell") return;
        if (other.transform.parent.parent.name != transform.tag) return;
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
        int[] cell = gridManager.PosToGrid(endingPos);
        // Debug.Log("Place on x= " + cell[0] + ", y= " + cell[1]);
        gridManager.UpdateGridVal(cell[0], cell[1], topVal, midVal, botVal);
        GameManager.GetInstance().SpawnNewTile();
    }
}
