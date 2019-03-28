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
    List<GameObject> gridObjs = new List<GameObject>();

    private string myID;
    public Dictionary<string, GridBase> grids = new Dictionary<string, GridBase>();


    void Start()
    {
        myID = PlayerPrefs.GetString("myID", "");
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.NEW_PIECE, this);
    }

    public void UpdateGrid(string player_id, GridState state)
    {
        bool isMe = (player_id == myID);
        Transform spawnPosition = isMe ? grid1SpawnPos : grid2SpawnPos;
        GameObject target = GameObject.Find(player_id);
        if (target == null)
        {
            GridBase grid = Instantiate(gridPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);
            grid.gameObject.name = player_id;
            if (isMe) grid.transform.tag = Constants.PLACEABLE_TAG;
            grids[player_id] = grid;
            gridObjs.Add(grid.gameObject);
        }
        grids[player_id].UpdateState(state);
    }

    public void ActiveGrid(string player_id)
    {
        foreach (KeyValuePair<string, GridBase> item in grids)
        {
            string key = item.Key;
            if (key == player_id)
            {
                item.Value.transform.GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
            else
            {
                if (key != myID) item.Value.transform.GetComponent<SpriteRenderer>().sortingOrder = -5;
            }
        }
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
