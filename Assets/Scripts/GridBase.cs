using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class GridBase : MonoBehaviour
{
    public Tile renderTile;
    public float cellSize = 0.5f;
    int[,] grid = new int[9, 9];
    private int dim = 9;

    public int[] KeyToGrid(string key)
    {
        int y = '8' - key[0];
        int x = key[1] - '0';
        return new int[2] { x, y };
    }

    public int[] PosToGrid(Vector2 pos)
    {
        Vector3 gridPos = transform.position;
        int x = (int)Mathf.Floor((pos.x - gridPos.x + 2) / cellSize);
        int y = (int)Mathf.Floor((pos.y - gridPos.y + 2) / cellSize);
        int[] res = new int[2] { x, y };
        return res;
    }

    public Vector3 GridToPos(int[] cell)
    {
        Vector3 gridPos = transform.position;
        float x = cell[0] * cellSize - 2 + gridPos.x;
        float y = cell[1] * cellSize - 2 + gridPos.y;
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
        }
    }

    void searchAndMatch(int[] tempList, Dictionary<int, int[]> map)
    {
        int u = 0;
        int len = tempList.Length;
        int newScore = 0;
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
                    newScore += tempList[u] * (v - u);
                    Vector3 from = GridToPos(map[u]);
                    Vector3 to = GridToPos(map[v - 1]);
                    DrawLine(from, to);
                }
                u = v;
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

    public void PlacePiece(JToken piece)
    {
        int x = (int)piece["x"];
        int y = dim - 2 - 3 * (int)piece["y"];
        Vector3 placePos = GridToPos(new int[2] { x, y });
        Tile newTile = Instantiate(renderTile, placePos, Quaternion.identity, transform);
        newTile.SetVal(piece["values"].ToObject<int[]>());
    }

    public void UpdateData(Dictionary<string, int> gridData)
    {
        foreach (KeyValuePair<string, int> cell in gridData)
        {
            if (cell.Value == -1) continue;
            var pos = KeyToGrid(cell.Key);
            Debug.Log(string.Format("x = {0}, y = {1}", pos[0], pos[1], cell.Value));
            grid[pos[0], pos[1]] = cell.Value;
        }
        UpdateStatus();
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        GameObject newLine = new GameObject("new line");
        LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        lineRenderer.sortingLayerName = "Tile";
        lineRenderer.sortingOrder = 5;
    }
}
