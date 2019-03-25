using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public struct GridState
{
    public string player_id;
    public string player_nick;
    public int point;
    public string game_id;
    public Dictionary<string, int> grid;

    public GridState(string _player_id, string _player_nick, int _point, string _game_id, Dictionary<string, int> _grid)
    {
        player_id = _player_id;
        player_nick = _player_id;
        point = _point;
        game_id = _game_id;
        grid = _grid;
    }
}

public class BoardsManager : MonoBehaviour
{
    public GameObject gridPrefab;
    public Transform grid1SpawnPos;
    public Transform grid2SpawnPos;

    Dictionary<string, GridState> boards = new Dictionary<string, GridState>();

    public void SpawnGrid(int id, GridState state)
    {
        Transform spawnPosition = id == 0 ? grid1SpawnPos : grid2SpawnPos;
        GridBase grid;
        if (spawnPosition.childCount > 0)
        {
            grid = spawnPosition.GetComponentInChildren<GridBase>();
        }
        else
        {
            var gridObj = Instantiate(gridPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);
            grid = gridObj.GetComponent<GridBase>();
        }
        grid.UpdateState(state);
    }
}