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

public class GameManager : MonoBehaviour, IListener
{
    public Tile tilePrefab;
    public Transform tileSpawnPos;

    public GridBase gridPrefab;
    public Transform grid1SpawnPos;
    public Transform grid2SpawnPos;

    public bool isGameOver = false;

    private string myID;
    private Dictionary<string, GridBase> grids = new Dictionary<string, GridBase>();

    void Start()
    {
        myID = PlayerPrefs.GetString("myID", "");
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.NEW_PIECE, this);
    }

    public void UpdateGrid(string player_id, GridState state)
    {
        Transform spawnPosition = player_id == myID ? grid1SpawnPos : grid2SpawnPos;
        if (spawnPosition.childCount == 0)
        {
            GridBase grid = Instantiate(gridPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);
            if (player_id == myID) grid.transform.tag = Constants.PLACEABLE_TAG;
            grids[player_id] = grid;
        }
        grids[player_id].UpdateState(state);
    }

    public void UpdateGridData(string player_id, Dictionary<string, int> gridData, JToken piece)
    {
        if (myID != player_id)
        {
            grids[player_id].PlacePiece(piece);
        }
        grids[player_id].UpdateData(gridData);
    }

    public void SpawnNewTiles(int[] piece)
    {
        Tile tile = Instantiate(tilePrefab, tileSpawnPos.position, Quaternion.identity, tileSpawnPos);
        tile.transform.tag = Constants.MOVABLE_TAG;
        tile.SetVal(piece);
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.GAMEOVER:
                GameOver();
                break;
            case EVENT_TYPE.PLAYER_LEFT:
                GameOver();
                break;
            case EVENT_TYPE.RESTART:
                Restart();
                break;
            case EVENT_TYPE.NEW_PIECE:
                int[] pieceVal = (int[])param;
                SpawnNewTiles(pieceVal);
                break;
            default:
                break;
        }
    }
}
