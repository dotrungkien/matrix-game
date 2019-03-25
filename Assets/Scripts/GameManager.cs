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

public class GameManager : Singleton<GameManager>, IListener
{
    public Tile tilePrefab;
    public Transform tileSpawnPos1;
    public Transform tileSpawnPos2;

    public GridBase gridPrefab;
    public Transform grid1SpawnPos;
    public Transform grid2SpawnPos;

    public bool isGameOver = false;

    public int turn;

    private Dictionary<string, GridBase> grids = new Dictionary<string, GridBase>();

    void Start()
    {
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.NEW_PIECE, this);
    }

    public void UpdateGrid(string player_id, GridState state)
    {
        string myID = PlayerPrefs.GetString("myID", "");
        Debug.Log(string.Format("playerID {0} myID {1}", player_id, myID));
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
        grids[player_id].PlacePiece(piece);
        grids[player_id].UpdateData(gridData);
    }

    public int NextTurn()
    {
        turn = turn == 0 ? 1 : 0;
        return turn;
    }

    public void SpawnNewTiles(int[] piece)
    {
        Tile tile1 = Instantiate(tilePrefab, tileSpawnPos1.position, Quaternion.identity, tileSpawnPos1);
        tile1.transform.tag = Constants.MOVABLE_TAG;
        tile1.SetVal(piece);
        Tile tile2 = Instantiate(tilePrefab, tileSpawnPos2.position, Quaternion.identity, tileSpawnPos2);
        tile2.SetVal(piece);
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void Restart()
    {
        turn = 0;
        isGameOver = false;
        SceneManager.LoadScene("Game");
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.GAMEOVER:
                GameOver();
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
