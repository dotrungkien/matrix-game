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
    public GameObject tile1;
    public GameObject tile2;
    public Transform tileSpawnPos1;
    public Transform tileSpawnPos2;

    public GameObject gridPrefab;
    public Transform grid1SpawnPos;
    public Transform grid2SpawnPos;

    public int player1Score;
    public int player2Score;

    public int turnCount = 0;
    public bool isGameOver = false;

    public int turn;

    private Dictionary<int, GridBase> grids = new Dictionary<int, GridBase>();

    void Start()
    {
        // Instantiate(tile1, tileSpawnPos1.position, Quaternion.identity, tileSpawnPos1);
        // Instantiate(tile2, tileSpawnPos2.position, Quaternion.identity, tileSpawnPos2);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.NEW_PIECE, this);
    }

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

    public int NextTurn()
    {
        turn = turn == 0 ? 1 : 0;
        return turn;
    }

    public void SpawnNewTile(int[] piece)
    {
        turnCount += 1;
        GameObject tileObj;
        if (turn == 0)
        {
            tileObj = Instantiate(tile1, tileSpawnPos1.position, Quaternion.identity, tileSpawnPos1);

        }
        else
        {
            tileObj = Instantiate(tile2, tileSpawnPos2.position, Quaternion.identity, tileSpawnPos2);
        }
        Tile tile = tileObj.GetComponent<Tile>();
        tile.SetVal(piece);
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void Restart()
    {
        turn = 0;
        turnCount = 0;
        isGameOver = false;
        SceneManager.LoadScene("Game");
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.SCORE_CHANGE:
                if (sender.tag == Constants.GRID1_TAG) player1Score = (int)param;
                if (sender.tag == Constants.GRID2_TAG) player2Score = (int)param;
                break;
            case EVENT_TYPE.GAMEOVER:
                GameOver();
                break;
            case EVENT_TYPE.NEW_PIECE:
                int[] pieceVal = (int[])param;
                // Debug.Log(string.Format("New Piece {0} {1} {2}", pieceVal[0], pieceVal[1], pieceVal[2]));
                SpawnNewTile(pieceVal);
                break;
            default:
                break;
        }
    }
}
