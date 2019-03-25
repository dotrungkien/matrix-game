using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class BoardsManager : MonoBehaviour
{
    public GameObject gridPrefab;
    public Transform grid1SpawnPos;
    public Transform grid2SpawnPos;

    private Dictionary<int, GridBase> grids = new Dictionary<int, GridBase>();

    public void UpdateGrid(int id, GridState state)
    {
        Transform spawnPosition = id == 0 ? grid1SpawnPos : grid2SpawnPos;
        if (spawnPosition.childCount == 0)
        {
            var gridObj = Instantiate(gridPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);
            gridObj.tag = id == 0 ? Constants.GRID1_TAG : Constants.GRID2_TAG;
            GridBase grid = gridObj.GetComponent<GridBase>();
            grids[id] = grid;
        }
        grids[id].UpdateState(state);
    }

    public void UpdateGridData(int id, Dictionary<string, int> gridData)
    {
        grids[id].UpdateData(gridData);
    }
}