using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public float cellSize = 0.5f;
    int[,] grid = new int[9, 9];
    public int totalScore = 0;

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
        // GridLog();
    }



    public void UpdateStatus()
    {
        int newScore = 0;
        int dim = 9;

        // Columns
        for (int i = 0; i < dim; i++)
        {
            int[] tempList = new int[dim];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();
            for (int j = 0; j < dim; j++)
            {
                tempList[j] = grid[i, j];
                map[j] = new int[] { i, j };
            }
            newScore += searchAndMatch(tempList, map);
            // int j = 0;
            // while (j < 8)
            // {
            //     if (grid[i, j] == 0 || grid[i, j] != grid[i, j + 1])
            //     {
            //         j++;
            //         if (j > 8) break;
            //     }
            //     else
            //     {
            //         int k = j;
            //         while (k < 9 && grid[i, j] == grid[i, k]) k++;
            //         if (k - j >= 3)
            //         {
            //             newScore += grid[i, j] * (k - j);
            //             Vector3 from = GridToPos(new int[2] { i, j });
            //             Vector3 to = GridToPos(new int[2] { i, k - 1 });
            //             DrawLine(from, to);
            //         }
            //         j = k;
            //     }
            // }
        }
        // Rows
        for (int j = 0; j < dim; j++)
        {
            int[] tempList = new int[dim];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();
            for (int i = 0; i < dim; i++)
            {
                tempList[i] = grid[i, j];
                map[i] = new int[] { i, j };
            }
            newScore += searchAndMatch(tempList, map);
            // int i = 0;
            // while (i < 8)
            // {
            //     if (grid[i, j] == 0 || grid[i, j] != grid[i + 1, j])
            //     {
            //         i++;
            //         if (i > 8) break;
            //     }
            //     else
            //     {
            //         int k = i + 1;
            //         while (k < 9 && grid[i, j] == grid[k, j]) k++;
            //         if (k - i >= 3)
            //         {
            //             newScore += grid[i, j] * (k - i);
            //             Vector3 from = GridToPos(new int[2] { i, j });
            //             Vector3 to = GridToPos(new int[2] { k - 1, j });
            //             DrawLine(from, to);
            //         }
            //         i = k;
            //     }
            // }
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
                map[j] = new int[] { i, j };
            }
            newScore += searchAndMatch(tempList, map);
            // int u = 0;
            // while (u < k)
            // {
            //     if (tempList[u] == 0 || tempList[u] != tempList[u + 1])
            //     {
            //         u++;
            //         if (u > k) break;
            //     }
            //     else
            //     {
            //         int v = u;
            //         while (v < k + 1 && tempList[u] == tempList[v]) v++;
            //         if (v - u >= 3)
            //         {
            //             newScore += tempList[u] * (v - u);
            //             Vector3 from = GridToPos(map[u]);
            //             Vector3 to = GridToPos(map[v - 1]);
            //             DrawLine(from, to);
            //         }
            //         u = v;
            //     }
            // }
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
                map[j] = new int[] { dim - j - 1, dim - i - 1 };
            }
            newScore += searchAndMatch(tempList, map);

            // int u = 0;
            // while (u < k)
            // {
            //     if (tempList[u] == 0 || tempList[u] != tempList[u + 1])
            //     {
            //         u++;
            //         if (u > k) break;
            //     }
            //     else
            //     {
            //         int v = u;
            //         while (v < k + 1 && tempList[u] == tempList[v]) v++;
            //         if (v - u >= 3)
            //         {
            //             newScore += tempList[u] * (v - u);
            //             Vector3 from = GridToPos(map[u]);
            //             Vector3 to = GridToPos(map[v - 1]);
            //             DrawLine(from, to);
            //         }
            //         u = v;
            //     }
            // }
        }

        // lower / diagonal
        for (int k = dim - 2; k >= 2; k--)
        {
            int[] tempList = new int[k + 1];
            Dictionary<int, int[]> map = new Dictionary<int, int[]>();

            for (int j = 0; j <= k; j++)
            {
                int i = k - j;
                tempList[j] = grid[dim - i - 1, dim - j - 1];
                map[j] = new int[] { dim - i - 1, dim - j - 1 };
            }
            newScore += searchAndMatch(tempList, map);

            // Debug.Log(string.Join(" ", tempList));

            // int u = 0;
            // while (u < k)
            // {
            //     if (tempList[u] == 0 || tempList[u] != tempList[u + 1])
            //     {
            //         u++;
            //         if (u > k) break;
            //     }
            //     else
            //     {
            //         int v = u;
            //         while (v < k + 1 && tempList[u] == tempList[v]) v++;
            //         if (v - u >= 3)
            //         {
            //             newScore += tempList[u] * (v - u);
            //             Vector3 from = GridToPos(map[u]);
            //             Vector3 to = GridToPos(map[v - 1]);
            //             DrawLine(from, to);
            //         }
            //         u = v;
            //     }
            // }
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
                map[i] = new int[] { i, j };
            }
            newScore += searchAndMatch(tempList, map);
            // int u = 0;
            // while (u < k)
            // {
            //     if (tempList[u] == 0 || tempList[u] != tempList[u + 1])
            //     {
            //         u++;
            //         if (u > k) break;
            //     }
            //     else
            //     {
            //         int v = u;
            //         while (v < k + 1 && tempList[u] == tempList[v]) v++;
            //         if (v - u >= 3)
            //         {
            //             newScore += tempList[u] * (v - u);
            //             Vector3 from = GridToPos(map[u]);
            //             Vector3 to = GridToPos(map[v - 1]);
            //             DrawLine(from, to);
            //         }
            //         u = v;
            //     }
            // }
        }

        totalScore = newScore;
    }

    int searchAndMatch(int[] tempList, Dictionary<int, int[]> map)
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
        return newScore;
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        GameObject newLine = new GameObject("new line");
        LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.04f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        lineRenderer.sortingLayerName = "Tile";
        lineRenderer.sortingOrder = 5;
    }

    private void GridLog()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Debug.Log("i = " + i + " j = " + j + " grid[i,j] = " + grid[i, j] + " ");
            }
            Debug.Log("\n");
        }
    }
}
