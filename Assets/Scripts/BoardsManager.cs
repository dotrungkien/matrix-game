using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public struct GridState
{
    public int points;
    public string player_id;
    public string nick;
    public Dictionary<string, int> grid;
    public string game_id;
}

public class BoardsManager : MonoBehaviour
{
    public GameObject gridPrefab;
    public Transform grid1SpawnPos;
    public Transform grid2SpawnPos;


    Dictionary<string, GridState> boards = new Dictionary<string, GridState>();

    void Start()
    {

    }
    void SpawnNewGrid(GridState state)
    {
        Transform spawnPosition = grid1SpawnPos.childCount == 0 ? grid1SpawnPos : grid2SpawnPos;
        var gridObj = Instantiate(gridPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);
        var grid = gridObj.GetComponent<GridBase>();
        grid.UpdateState(state);
    }
}