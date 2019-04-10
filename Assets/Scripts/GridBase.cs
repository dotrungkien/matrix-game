using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using DG.Tweening;

public class GridBase : MonoBehaviour
{
    public Tile renderTile;
    [HideInInspector]
    public Color color;
    public float cellSize = 0.4f;
    int[,] grid = new int[9, 9];
    private int dim = 9;
    private Tile lastTile = null;

    public int[] KeyToGrid(string key)
    {
        int y = '8' - key[0];
        int x = key[1] - '0';
        return new int[2] { x, y };
    }

    public int[] PosToGrid(Vector2 pos)
    {
        Vector3 gridPos = transform.position;
        int x = Mathf.RoundToInt((pos.x - gridPos.x) / cellSize) + 4;
        int y = Mathf.RoundToInt((pos.y - gridPos.y) / cellSize) + 4;
        int[] res = new int[2] { x, y };
        return res;
    }

    public Vector3 GridToPos(int[] cell)
    {
        Vector3 gridPos = transform.position;
        float x = cell[0] * cellSize - cellSize * 4 + gridPos.x;
        float y = cell[1] * cellSize - cellSize * 4 + gridPos.y;
        return new Vector3(x, y, 0);
    }

    public void UpdateGridVal(int x, int y, int top, int mid, int bot)
    {
        grid[x, y] = mid;
        grid[x, y - 1] = bot;
        grid[x, y + 1] = top;
        UpdateStatus();
    }

    public void UpdateStatus()
    {
        // Columns
        for (int i = 0; i < dim; i++)
        {
            int[] tempList = new int[dim];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();
            for (int j = 0; j < dim; j++)
            {
                tempList[j] = grid[i, j];
                map[j] = new int[2] { i, j };
            }
            searchAndMatch(tempList, map);
        }
        // Rows
        for (int j = 0; j < dim; j++)
        {
            int[] tempList = new int[dim];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();
            for (int i = 0; i < dim; i++)
            {
                tempList[i] = grid[i, j];
                map[i] = new int[2] { i, j };
            }
            searchAndMatch(tempList, map);
        }
        // lower \ diagonal
        for (int k = 2; k < dim; k++)
        {
            int[] tempList = new int[k + 1];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();
            for (int j = 0; j <= k; j++)
            {
                int i = k - j;
                tempList[j] = grid[i, j];
                map[j] = new int[2] { i, j };
            }
            searchAndMatch(tempList, map);
        }
        // upper \ diagonal
        for (int k = dim - 2; k >= 2; k--)
        {
            int[] tempList = new int[k + 1];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();

            for (int j = 0; j <= k; j++)
            {
                int i = k - j;
                tempList[j] = grid[dim - j - 1, dim - i - 1];
                map[j] = new int[2] { dim - j - 1, dim - i - 1 };
            }
            searchAndMatch(tempList, map);
        }
        // lower / diagonal
        for (int k = dim - 2; k >= 2; k--)
        {
            int[] tempList = new int[k + 1];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();

            for (int j = 0; j <= k; j++)
            {
                int i = k - j;
                tempList[j] = grid[dim - j - 1, i];
                map[j] = new int[2] { dim - j - 1, i };
            }
            searchAndMatch(tempList, map);
        }
        // upper / diagonal
        for (int k = 2; k < dim; k++)
        {
            int[] tempList = new int[k + 1];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();
            for (int i = 0; i <= k; i++)
            {
                int j = dim - k - 1 + i;
                tempList[i] = grid[i, j];
                map[i] = new int[2] { i, j };
            }
            searchAndMatch(tempList, map);
        }
    }

    void searchAndMatch(int[] tempList, Dictionary<int, int[]> map)
    {
        int u = 0;
        int len = tempList.Length;
        while (u < len - 1)
        {
            if (tempList[u] == 0 || tempList[u] != tempList[u + 1])
            {
                u++;
                if (u > len - 1) break;
            }
            else
            {
                int v = u;
                while (v < len && tempList[u] == tempList[v]) v++;
                if (v - u >= 3)
                {
                    Vector3 from = GridToPos(map[u]);
                    Vector3 to = GridToPos(map[v - 1]);
                    DrawLine(from, to);
                }
                u = v;
            }
        }
    }

    public void UpdateStateFirstTime(GridState props)
    {
        var gridData = props.grid;
        foreach (KeyValuePair<string, int> cell in gridData)
        {
            if (cell.Value == -1) continue;
            var pos = KeyToGrid(cell.Key);
            grid[pos[0], pos[1]] = cell.Value;
        }
        UpdatePieces();
        UpdateStatus();
    }

    public void UpdatePieces()
    {
        for (int i = 0; i < dim; i++)
        {
            for (int j = 1; j < dim; j = j + 3)
            {
                if (grid[i, j] == 0) continue;
                int top = grid[i, j + 1];
                int mid = grid[i, j];
                int bot = grid[i, j - 1];
                Vector3 placePos = GridToPos(new int[2] { i, j });
                Tile newTile = Instantiate(renderTile, placePos, Quaternion.identity, transform);
                newTile.GetComponent<SpriteRenderer>().color = color;
                newTile.SetVal(new int[] { top, mid, bot });
            }
        }
    }

    public void UpdateState(GridState props)
    {
        var gridData = props.grid;
        foreach (KeyValuePair<string, int> cell in gridData)
        {
            if (cell.Value == -1) continue;
            var pos = KeyToGrid(cell.Key);
            grid[pos[0], pos[1]] = cell.Value;
        }
        UpdateStatus();
    }

    public void PlacePiece(JToken piece, Transform spawnPos)
    {

        int x = (int)piece["x"];
        int y = dim - 2 - (int)piece["y"];
        Vector3 placePos = GridToPos(new int[2] { x, y });
        int[] pieceVal = piece["values"].ToObject<int[]>();
        Tile newTile = GameManager.GetInstance().SpawnNewTile(pieceVal, spawnPos.position, transform);
        newTile.SetColor(color);
        if (gameObject.name != PlayerPrefs.GetString("username"))
        {
            newTile.transform.DOMove(placePos, 1.0f);
        }
        else
        {
            newTile.transform.position = placePos;
        }
        UpdateLastTile(newTile);

    }

    public void UpdateLastTile(Tile newTile)
    {
        if (lastTile != null) lastTile.DisableFrame();
        newTile.EnableFrame();
        lastTile = newTile;
    }

    public void UpdateData(Dictionary<string, int> gridData)
    {
        foreach (KeyValuePair<string, int> cell in gridData)
        {
            if (cell.Value == -1) continue;
            var pos = KeyToGrid(cell.Key);
            grid[pos[0], pos[1]] = cell.Value;
        }
        UpdateStatus();
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        GameObject newLine = new GameObject("new line");
        newLine.transform.parent = transform;
        LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        Vector3 newFrom = from;
        Vector3 newTo = to;

        float extra = cellSize * 0.5f;

        if (from.x == to.x)
        {
            if (from.y < to.y)
            {
                newFrom = new Vector3(from.x, from.y - extra, from.z);
                newTo = new Vector3(to.x, to.y + extra, to.z);
            }
            else
            {
                newFrom = new Vector3(from.x, from.y + extra, from.z);
                newTo = new Vector3(to.x, to.y - extra, to.z);
            }
        }
        else if (from.y == to.y)
        {
            if (from.x < to.x)
            {
                newFrom = new Vector3(from.x - extra, from.y, from.z);
                newTo = new Vector3(to.x + extra, to.y, to.z);
            }
            else
            {
                newFrom = new Vector3(from.x + extra, from.y, from.z);
                newTo = new Vector3(to.x - extra, to.y, to.z);
            }
        }
        else
        {
            if (from.x < to.x && from.y > to.y)
            {
                newFrom = new Vector3(from.x - extra, from.y + extra, from.z);
                newTo = new Vector3(to.x + extra, to.y - extra, to.z);
            }
            else if (from.x < to.x && from.y < to.y)
            {
                newFrom = new Vector3(from.x - extra, from.y - extra, from.z);
                newTo = new Vector3(to.x + extra, to.y + extra, to.z);
            }
            else if (from.x > to.x && from.y > to.y)
            {
                newFrom = new Vector3(from.x + extra, from.y + extra, from.z);
                newTo = new Vector3(to.x - extra, to.y - extra, to.z);
            }
            else if (from.x > to.x && from.y < to.y)
            {
                newFrom = new Vector3(from.x + extra, from.y - extra, from.z);
                newTo = new Vector3(to.x - extra, to.y + extra, to.z);
            }
        }

        lineRenderer.SetPosition(0, newFrom);
        lineRenderer.SetPosition(1, newTo);
        lineRenderer.sortingLayerName = "Tile";
        lineRenderer.sortingOrder = 5;
    }
}
